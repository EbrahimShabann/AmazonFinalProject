using Microsoft.AspNetCore.Identity;

namespace Final_project.Models
{
    public class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // Define the roles to be created
                string[] roleNames = { "admin", "seller", "customer", "customerService" };

                // Create roles if they don't exist
                foreach (var roleName in roleNames)
                {
                    var roleExist = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExist)
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }

                // Create admin user if no users exist
                var usersCount = userManager.Users.Count();
                if (usersCount == 0)
                {
                    var adminUser = new ApplicationUser
                    {
                        UserName = "Admin@amazon.com",
                        Email = "Admin@amazon.com",
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(adminUser, "Admin1234");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }
            }
        }
    }
}