using System;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using code_hunter.Models.Account;

namespace code_hunter.Context
{
    public static class ContextHelper
    {
        public static async Task Seeding(CodeHunterContext context, UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            //create role Admin if not exists
            if (!roleManager.Roles.Any(p => p.Name.Equals("Admin")))
            {
                var adminRole = new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                };

                await roleManager.CreateAsync(adminRole);
            }

            //create role User if not exists
            if (!roleManager.Roles.Any(p => p.Name.Equals("User")))
            {
                var adminRole = new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER"
                };

                await roleManager.CreateAsync(adminRole);
            }

            //create role Organization if not exists
            if (!roleManager.Roles.Any(p => p.Name.Equals("Organization")))
            {
                var organizationRole = new IdentityRole
                {
                    Name = "Organization",
                    NormalizedName = "ORGANIZATION"
                };

                await roleManager.CreateAsync(organizationRole);
            }

            if (!userManager.Users.Any(p => p.Email.Equals("admin@codehunter.com")))
            {
                var adminUser = new User
                {
                    UserName = "admin",
                    Email = "admin@codehunter.com",
                    Removed = false,
                    CreatedAt = DateTime.Now
                };
                var result = await userManager.CreateAsync(adminUser, "secret");

                if (result.Succeeded)
                {
                    var role = await roleManager.FindByNameAsync("Admin");

                    await userManager.AddToRoleAsync(await userManager.FindByNameAsync("Admin"), role.Name);
                }
            }
        }
    }
}