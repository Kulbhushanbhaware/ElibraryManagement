using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ASPIdentityApp.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    // Optional: Add a computed property for full name
    public string FullName => $"{FirstName} {LastName}";
}

