using Microsoft.AspNetCore.Identity;

namespace ASPIdentityApp.Areas.Identity.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Create roles
            string[] roleNames = { "Admin", "Librarian", "Member" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create default admin user
            var adminUser = new ApplicationUser
            {
                UserName = "admin@elibrary.com",
                Email = "admin@elibrary.com",
                FirstName = "System",
                LastName = "Admin",
                EmailConfirmed = true
            };

            string adminPassword = "Admin@123";
            var _adminUser = await userManager.FindByEmailAsync(adminUser.Email);

            if (_adminUser == null)
            {
                var createAdmin = await userManager.CreateAsync(adminUser, adminPassword);
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    await userManager.AddToRoleAsync(adminUser, "Librarian");
                }
            }
        }
    }
}
