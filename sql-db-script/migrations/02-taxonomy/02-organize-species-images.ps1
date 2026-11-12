[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$ConnectionString,

    [Parameter(Mandatory = $true)]
    [string]$PicturesRoot,

    [string]$PlanCsvPath,

    [switch]$Apply,

    [switch]$AllowIncomplete
)

$ErrorActionPreference = 'Stop'

$picturesRootItem = Get-Item -LiteralPath $PicturesRoot -ErrorAction Stop
if (-not $picturesRootItem.PSIsContainer) {
    throw "Pictures root does not exist: $PicturesRoot"
}

$picturesRootFull = [System.IO.Path]::GetFullPath($picturesRootItem.FullName)
$picturesRootName = $picturesRootItem.Name
if ($picturesRootName -cnotin @('Bilder', 'bilder')) {
    throw "PicturesRoot must be named 'Bilder' or 'bilder': $picturesRootFull"
}

$renamePicturesRoot = $picturesRootName -ceq 'Bilder'
$picturesRootParent = Split-Path -Parent $picturesRootFull
$lowercasePicturesRoot = Join-Path $picturesRootParent 'bilder'
$temporaryPicturesRoot = Join-Path $picturesRootParent ('.bilder-migration-' + [guid]::NewGuid().ToString('N'))

if ($renamePicturesRoot) {
    $existingLowercaseRoot = Get-ChildItem -LiteralPath $picturesRootParent -Directory -Force |
        Where-Object { $_.Name -ceq 'bilder' }
    if ($null -ne $existingLowercaseRoot) {
        throw "Cannot rename PicturesRoot to lowercase because a separate 'bilder' directory already exists: $($existingLowercaseRoot.FullName)"
    }
}

$query = @'
;WITH CategoryPaths AS
(
    SELECT
        root.menu_ID,
        root.menu_mainID,
        root.menu_text,
        root.menu_URLtext,
        root.menu_ID AS RootMenuId,
        CONVERT(nvarchar(2000), root.menu_URLtext) AS RelativeFolder
    FROM dbo.tbl_menu AS root
    WHERE root.menu_mainID = 0

    UNION ALL

    SELECT
        child.menu_ID,
        child.menu_mainID,
        child.menu_text,
        child.menu_URLtext,
        parent.RootMenuId,
        CONVERT(nvarchar(2000), parent.RelativeFolder + N'\' + child.menu_URLtext)
    FROM dbo.tbl_menu AS child
    INNER JOIN CategoryPaths AS parent ON child.menu_mainID = parent.menu_ID
)
SELECT DISTINCT
    i.image_URL AS ImageUrl,
    i.image_art AS ArtMenuId,
    p.menu_text AS CategoryNameSv,
    p.RelativeFolder
FROM dbo.tbl_images AS i
LEFT JOIN CategoryPaths AS p ON p.menu_ID = i.image_art
WHERE i.image_URL IS NOT NULL
  AND LTRIM(RTRIM(i.image_URL)) <> N''
ORDER BY i.image_URL, p.RelativeFolder
OPTION (MAXRECURSION 100);
'@

$connection = [System.Data.SqlClient.SqlConnection]::new($ConnectionString)
$command = $connection.CreateCommand()
$command.CommandText = $query
$command.CommandTimeout = 120
$reader = $null
$rows = [System.Collections.Generic.List[object]]::new()

try {
    $connection.Open()
    $reader = $command.ExecuteReader()

    while ($reader.Read()) {
        $rows.Add([pscustomobject]@{
            ImageUrl = if ($reader.IsDBNull(0)) { $null } else { $reader.GetString(0) }
            ArtMenuId = if ($reader.IsDBNull(1)) { $null } else { $reader.GetInt32(1) }
            CategoryNameSv = if ($reader.IsDBNull(2)) { $null } else { $reader.GetString(2) }
            RelativeFolder = if ($reader.IsDBNull(3)) { $null } else { $reader.GetString(3) }
        })
    }
}
finally {
    if ($null -ne $reader) {
        $reader.Dispose()
    }
    $command.Dispose()
    $connection.Dispose()
}

if ($rows.Count -eq 0) {
    throw 'The database query returned no image rows. Verify the database and connection string.'
}

$mediaExtensions = @('.jpg', '.jpeg', '.png', '.gif', '.webp', '.tif', '.tiff')
$allMediaFiles = @(
    Get-ChildItem -LiteralPath $picturesRootFull -File -Recurse |
        Where-Object { $mediaExtensions -contains $_.Extension.ToLowerInvariant() }
)

$fileIndex = @{}
foreach ($file in $allMediaFiles) {
    if ($file.BaseName.EndsWith('.thumb', [System.StringComparison]::OrdinalIgnoreCase)) {
        continue
    }

    $key = $file.BaseName.ToLowerInvariant()
    if (-not $fileIndex.ContainsKey($key)) {
        $fileIndex[$key] = [System.Collections.Generic.List[System.IO.FileInfo]]::new()
    }
    $fileIndex[$key].Add($file)
}

$plan = [System.Collections.Generic.List[object]]::new()
$knownMediaPaths = [System.Collections.Generic.HashSet[string]]::new(
    [System.StringComparer]::OrdinalIgnoreCase
)
$rowsByImage = $rows | Group-Object {
    if ($null -eq $_.ImageUrl) { '' } else { $_.ImageUrl.ToLowerInvariant() }
}

foreach ($imageGroup in $rowsByImage) {
    $distinctTargets = @(
        $imageGroup.Group |
            Where-Object { -not [string]::IsNullOrWhiteSpace($_.RelativeFolder) } |
            Select-Object -ExpandProperty RelativeFolder -Unique
    )

    if ($distinctTargets.Count -gt 1) {
        foreach ($row in $imageGroup.Group) {
            $plan.Add([pscustomobject]@{
                ImageUrl = $row.ImageUrl
                CategoryNameSv = $row.CategoryNameSv
                RelativeFolder = $row.RelativeFolder
                SourcePath = $null
                DestinationPath = $null
                ThumbnailSourcePath = $null
                ThumbnailDestinationPath = $null
                Status = 'DuplicateImageAssignment'
            })
        }
        continue
    }

    $row = $imageGroup.Group[0]
    if ([string]::IsNullOrWhiteSpace($row.RelativeFolder)) {
        $plan.Add([pscustomobject]@{
            ImageUrl = $row.ImageUrl
            CategoryNameSv = $row.CategoryNameSv
            RelativeFolder = $null
            SourcePath = $null
            DestinationPath = $null
            ThumbnailSourcePath = $null
            ThumbnailDestinationPath = $null
            Status = 'MissingCategoryPath'
        })
        continue
    }

    $pathSegments = $row.RelativeFolder -split '\\'
    if ($pathSegments.Count -eq 0 -or
        @($pathSegments | Where-Object { $_ -cnotmatch '^[a-z0-9,-]+$' }).Count -gt 0) {
        $plan.Add([pscustomobject]@{
            ImageUrl = $row.ImageUrl
            CategoryNameSv = $row.CategoryNameSv
            RelativeFolder = $row.RelativeFolder
            SourcePath = $null
            DestinationPath = $null
            ThumbnailSourcePath = $null
            ThumbnailDestinationPath = $null
            Status = 'InvalidMenuUrlText'
        })
        continue
    }

    $imageName = [System.IO.Path]::GetFileName($row.ImageUrl)
    $imageBase = [System.IO.Path]::GetFileNameWithoutExtension($imageName)
    if ([string]::IsNullOrWhiteSpace($imageBase)) {
        $plan.Add([pscustomobject]@{
            ImageUrl = $row.ImageUrl
            CategoryNameSv = $row.CategoryNameSv
            RelativeFolder = $row.RelativeFolder
            SourcePath = $null
            DestinationPath = $null
            ThumbnailSourcePath = $null
            ThumbnailDestinationPath = $null
            Status = 'InvalidImageUrl'
        })
        continue
    }

    $key = $imageBase.ToLowerInvariant()
    if (-not $fileIndex.ContainsKey($key)) {
        $plan.Add([pscustomobject]@{
            ImageUrl = $row.ImageUrl
            CategoryNameSv = $row.CategoryNameSv
            RelativeFolder = $row.RelativeFolder
            SourcePath = $null
            DestinationPath = $null
            ThumbnailSourcePath = $null
            ThumbnailDestinationPath = $null
            Status = 'MissingFile'
        })
        continue
    }

    $sources = $fileIndex[$key]
    if ($sources.Count -ne 1) {
        $plan.Add([pscustomobject]@{
            ImageUrl = $row.ImageUrl
            CategoryNameSv = $row.CategoryNameSv
            RelativeFolder = $row.RelativeFolder
            SourcePath = ($sources.FullName -join ' | ')
            DestinationPath = $null
            ThumbnailSourcePath = $null
            ThumbnailDestinationPath = $null
            Status = 'DuplicateFilename'
        })
        continue
    }

    $source = $sources[0]
    [void]$knownMediaPaths.Add([System.IO.Path]::GetFullPath($source.FullName))
    $destinationDirectory = Join-Path $picturesRootFull $row.RelativeFolder
    $destinationPath = Join-Path $destinationDirectory $source.Name
    $sourceFullPath = [System.IO.Path]::GetFullPath($source.FullName)
    $destinationFullPath = [System.IO.Path]::GetFullPath($destinationPath)

    $thumbnail = Get-ChildItem -LiteralPath $source.DirectoryName -File -Filter "$imageBase.thumb.*" |
        Where-Object {
            ($mediaExtensions -contains $_.Extension.ToLowerInvariant()) -and
            $_.Extension -eq $source.Extension
        } |
        Select-Object -First 1

    $thumbnailSourcePath = $null
    $thumbnailDestinationPath = $null
    if ($null -ne $thumbnail) {
        $thumbnailSourcePath = [System.IO.Path]::GetFullPath($thumbnail.FullName)
        $thumbnailDestinationPath = Join-Path $destinationDirectory $thumbnail.Name
        [void]$knownMediaPaths.Add($thumbnailSourcePath)
    }

    $status = 'Ready'
    if ($sourceFullPath.Equals($destinationFullPath, [System.StringComparison]::OrdinalIgnoreCase)) {
        $status = 'AlreadyInPlace'
    }
    elseif (Test-Path -LiteralPath $destinationFullPath) {
        $status = 'DestinationExists'
    }
    elseif ($null -ne $thumbnailDestinationPath -and
        (Test-Path -LiteralPath $thumbnailDestinationPath)) {
        $status = 'ThumbnailDestinationExists'
    }

    $plan.Add([pscustomobject]@{
        ImageUrl = $row.ImageUrl
        CategoryNameSv = $row.CategoryNameSv
        RelativeFolder = $row.RelativeFolder
        SourcePath = $sourceFullPath
        DestinationPath = $destinationFullPath
        ThumbnailSourcePath = $thumbnailSourcePath
        ThumbnailDestinationPath = $thumbnailDestinationPath
        Status = $status
    })
}

foreach ($file in $allMediaFiles) {
    $fullPath = [System.IO.Path]::GetFullPath($file.FullName)
    if (-not $knownMediaPaths.Contains($fullPath)) {
        $plan.Add([pscustomobject]@{
            ImageUrl = $null
            CategoryNameSv = $null
            RelativeFolder = $null
            SourcePath = $fullPath
            DestinationPath = $null
            ThumbnailSourcePath = $null
            ThumbnailDestinationPath = $null
            Status = 'NotInDatabase'
        })
    }
}

if (-not [string]::IsNullOrWhiteSpace($PlanCsvPath)) {
    $plan | Export-Csv -LiteralPath $PlanCsvPath -NoTypeInformation -Encoding UTF8
    Write-Output "Plan exported to: $PlanCsvPath"
}

$blockers = @(
    $plan | Where-Object {
        $_.Status -in @(
            'DuplicateImageAssignment',
            'MissingCategoryPath',
            'InvalidMenuUrlText',
            'InvalidImageUrl',
            'MissingFile',
            'DuplicateFilename',
            'DestinationExists',
            'ThumbnailDestinationExists'
        )
    }
)

if ($Apply -and $blockers.Count -gt 0 -and -not $AllowIncomplete) {
    $blockerSummary = $blockers | Group-Object Status | ForEach-Object {
        '{0}={1}' -f $_.Name, $_.Count
    }
    throw "No files were moved. Resolve all blockers first: $($blockerSummary -join ', '). Use -AllowIncomplete only for an explicitly approved partial move."
}

if (-not $Apply -and $renamePicturesRoot) {
    Write-Output "Applying this plan will rename the root folder to: $lowercasePicturesRoot"
}

if ($Apply) {
    foreach ($item in $plan | Where-Object Status -eq 'Ready') {
        $destinationDirectory = Split-Path -Parent $item.DestinationPath
        New-Item -ItemType Directory -Path $destinationDirectory -Force | Out-Null
        Move-Item -LiteralPath $item.SourcePath -Destination $item.DestinationPath

        if (-not [string]::IsNullOrWhiteSpace($item.ThumbnailSourcePath)) {
            Move-Item -LiteralPath $item.ThumbnailSourcePath -Destination $item.ThumbnailDestinationPath
        }

        $item.Status = 'Moved'
        Write-Output "MOVED: $($item.SourcePath) -> $($item.DestinationPath)"
    }

    if ($renamePicturesRoot) {
        [System.IO.Directory]::Move($picturesRootFull, $temporaryPicturesRoot)
        try {
            [System.IO.Directory]::Move($temporaryPicturesRoot, $lowercasePicturesRoot)
        }
        catch {
            [System.IO.Directory]::Move($temporaryPicturesRoot, $picturesRootFull)
            throw
        }

        Write-Output "RENAMED ROOT: $picturesRootFull -> $lowercasePicturesRoot"
    }
}

$plan |
    Group-Object Status |
    Sort-Object Name |
    ForEach-Object { '{0}: {1}' -f $_.Name, $_.Count }

if (-not $Apply) {
    Write-Output 'Preview only. Folder names come from Swedish menu_URLtext values; re-run with -Apply only after reviewing the plan.'
}
elseif ($blockers.Count -gt 0) {
    Write-Warning 'An approved partial move was requested. Review the exported plan and skipped rows.'
}
else {
    Write-Output 'All database-backed image files were moved to their menu_URLtext folder paths.'
}
