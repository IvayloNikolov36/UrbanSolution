using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using UrbanSolution.Models;
using static UrbanSolution.Web.Infrastructure.WebConstants;

namespace UrbanSolution.Web.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {

        private static readonly IdentityRole[] roles =
        {
            new IdentityRole(AdminRole),
            new IdentityRole(ManagerRole)
        };

        public static async void SeedDatabase(this IApplicationBuilder app)
        {
            var serviceFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            var scope = serviceFactory.CreateScope();
            using (scope)
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role.Name))
                    {
                        await roleManager.CreateAsync(role);
                    }
                }

                var user = await userManager.FindByNameAsync(AdminUserName);
                if (user == null)
                {
                    user = new User()
                    {
                        UserName = AdminUserName,
                        Email = "admin@example.com",
                        FullName = "System Administrator",
                        Age = 20
                    };

                    await userManager.CreateAsync(user, DefaultAdminPassword);
                    await userManager.AddToRoleAsync(user, AdminRole);
                }

                user = await userManager.FindByNameAsync(ManagerUserName);
                if (user == null)
                {
                    user = new User()
                    {
                        UserName = ManagerUserName, 
                        Email = "manager@example.com",
                        FullName = "Regional Manager",
                        Age = 20
                    };

                    await userManager.CreateAsync(user, DefaultManagerPassword);
                    await userManager.AddToRoleAsync(user, ManagerRole);
                }
            }
        }
    }

}


