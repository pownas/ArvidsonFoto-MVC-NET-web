namespace ArvidsonFoto.Core.ApiResponses;

/// <summary>
/// Represents the response for path resolution in a category structure.
/// </summary>
/// <remarks>
/// Contains information about the resolved path including the category path,
/// number of resolved segments, and the final category ID.
/// </remarks>
public class PathResolutionResponse
{
    /// <summary>Den fullständiga kategorisökvägen</summary>
    public string CategoryPath { get; set; } = string.Empty;
    
    /// <summary>Antal segment som har lösts upp</summary>
    public int ResolvedSegments { get; set; }
    
    /// <summary>Det slutliga kategori-ID:t för den lösta sökvägen</summary>
    public int? FinalCategoryId { get; set; }
    
    /// <summary>Lista över sökvägsegment som ingår i lösningen</summary>
    public List<PathSegment> PathResolution { get; set; } = new List<PathSegment>();
}

/// <summary>
/// Represents a segment in a path resolution process, containing details such as level, name, ID, and URL segment.
/// </summary>
/// <remarks>This class is typically used to model individual segments of a hierarchical path, such as in
/// routing or navigation scenarios. Each segment includes a level identifier, a name, a unique ID, and a
/// URL-friendly representation.</remarks>
public class PathSegment
{
    /// <summary>Nivå i hierarkin för detta segment</summary>
    public string Level { get; set; } = string.Empty;
    
    /// <summary>Visningsnamn för segmentet</summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>Unikt ID för segmentet</summary>
    public int Id { get; set; }
    
    /// <summary>URL-vänligt segment för routning</summary>
    public string UrlSegment { get; set; } = string.Empty;
}
