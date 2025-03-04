using Microsoft.AspNetCore.Identity;

namespace ArvidsonFoto.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ArvidsonFotoUser class
public class ArvidsonFotoUser : IdentityUser
{
    public bool ShowAllLogs { get; set; } = false;
}
