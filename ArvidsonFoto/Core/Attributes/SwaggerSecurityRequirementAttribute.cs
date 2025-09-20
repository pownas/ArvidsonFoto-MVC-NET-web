namespace ArvidsonFoto.Core.Attributes;

/// <summary>
/// Represents an attribute used to specify a security requirement for a Swagger operation.
/// </summary>
/// <remarks>This attribute is applied to methods to indicate that a specific security scheme is required for
/// accessing the operation. The specified scheme should match one defined in the Swagger documentation.</remarks>
/// <param name="scheme">The name of the security scheme required for the operation. This value cannot be null or empty.</param>
public class SwaggerSecurityRequirementAttribute(string scheme) : Attribute
{
    /// <summary>
    /// Gets the URI scheme used by the current instance.
    /// </summary>
    public string Scheme { get; } = scheme;
}