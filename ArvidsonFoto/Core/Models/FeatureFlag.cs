namespace ArvidsonFoto.Core.Models;

/// <summary>
/// Represents a feature flag used to enable or disable specific functionality in an application.
/// </summary>
/// <remarks>Feature flags are commonly used to control the availability of features at runtime without requiring
/// code changes. This class provides properties to determine whether a feature is enabled and to specify an optional
/// end date for the feature.</remarks>
public class FeatureFlag()
{
    /// <summary>
    /// Gets or sets a value indicating whether the feature is enabled.
    /// </summary>
    public bool Enabled { get; init; } = false;

    /// <summary>
    /// Gets or sets the end date of the feature to see if it should be removed in unit-tests.
    /// </summary>
    public DateTime EndDate { get; init; }
}
