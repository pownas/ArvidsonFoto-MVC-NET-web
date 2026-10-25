# Copilot Instructions

## Project Guidelines
- For this NuGet upgrade, use exactly .NET 11 RC2 package version `11.0.0-rc.2.26504.105` for applicable framework packages; do not downgrade to stable 10.0.12. Report in Swedish.

## Code Quality
- For build and code-style warning cleanup, fix warnings without suppressing them and reduce the total below 50.

## CSS and SCSS workflow
- Edit `ArvidsonFoto/wwwroot/css/site.scss` as the CSS source of truth; do not hand-edit generated CSS.
- `ArvidsonFoto/compilerconfig.json` defines the generated output: `wwwroot/css/site.css` and its source map `wwwroot/css/site.css.map`. Regenerate and commit both outputs whenever `site.scss` changes.
- `ArvidsonFoto/Views/Shared/_Layout.cshtml` should reference `/css/site.css`. Production serves this precompiled file as a static asset; do not add a runtime Sass/WebOptimizer bundle for it.
- `site.min.css` is a separate legacy asset. Do not switch the layout to it unless it is regenerated from the current `site.scss` first.
- WebOptimizer remains for JavaScript bundles only. The Sass/V8 packages were removed because runtime Sass compilation depends on platform-specific ClearScript native binaries and caused CSS requests to fail with HTTP 500.

## Game Image API Guidelines
- Include an `IsGameSelectable` flag on each image.
- A disabled category must make all its images effectively unavailable to games, even when an individual image flag is true.
- Keep individual image flags manageable in admin.

## Category DTO Guidelines
- For `CategoryDto`, localized category names must be named `NameSv` and `NameEn` (not `CategoryNameSv`/`CategoryNameEn`); category URL fields remain explicitly language-suffixed.
- För `ImageDto` ska det inte finnas en neutral `Name`-property. Referenser till den ska i stället använda `CategoryNameSv` eller `CategoryNameEn`. Rapportera på svenska.