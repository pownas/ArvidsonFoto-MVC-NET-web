using Microsoft.AspNetCore.Identity;

namespace ArvidsonFoto.Core.Models;

/// <summary>
/// Represents an application user with additional profile data.
/// </summary>
/// <remarks>
/// Extends the standard IdentityUser to include application-specific properties
/// such as log viewing preferences.
/// </remarks>
// Add profile data for application users by adding properties to the ArvidsonFotoUser class
public class ArvidsonFotoIdentityUser : IdentityUser
{
    /// <summary>Indikerar om alla loggar ska visas för denna användare</summary>
    public bool ShowAllLogs { get; set; }
}
