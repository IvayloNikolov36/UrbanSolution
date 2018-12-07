using System.Threading.Tasks;

namespace UrbanSolution.Web.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using UrbanSolution.Models;
    using static WebConstants;

    public static class ApplicationBuilderExtensions
    {

        private static readonly IdentityRole[] roles =
        {
            new IdentityRole(AdminRole),
            new IdentityRole(ManagerRole),
            new IdentityRole(BlogAuthorRole)
        };

        public static async void SeedDatabase(this IApplicationBuilder app)
        {
            var serviceFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            var scope = serviceFactory.CreateScope();

            using (scope)
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                await CreateRoles(roleManager);

                await CreateAdmin(userManager);

                await CreateManager(userManager);

                await CreateRegionalManagers(userManager);

                await CreateBlogAuthor(userManager);

            }
        }

        private static async Task CreateBlogAuthor(UserManager<User> userManager)
        {
            var user = await userManager.FindByNameAsync(BlogAuthorUserName);

            if (user == null)
            {
                user = new User()
                {
                    UserName = BlogAuthorUserName,
                    Email = BlogAuthorEmail,
                    FullName = BlogAuthorFullName,
                    Age = UserDefaultAge
                };

                await userManager.CreateAsync(user, DefaultBlogAuthorPassword);
                await userManager.AddToRoleAsync(user, BlogAuthorRole);
            }
        }

        private static async Task CreateRegionalManagers(UserManager<User> userManager)
        {
            var regionsAsStrings = Enum.GetNames(typeof(RegionType));

            for (int i = 0; i < regionsAsStrings.Length; i++)
            {
                var managerUsername = ManagerUserName + regionsAsStrings[i];
                var user = await userManager.FindByNameAsync(managerUsername);
                if (user == null)
                {
                    user = new User()
                    {
                        UserName = managerUsername,
                        Email = string.Format(ManagerEmail, managerUsername),
                        FullName = $"Manager {regionsAsStrings[i]}",
                        ManagedRegion = Enum.Parse<RegionType>(regionsAsStrings[i]),
                        Age = UserDefaultAge
                    };

                    await userManager.CreateAsync(user, string.Format(DefaultManagerPassword, regionsAsStrings[i]));
                    await userManager.AddToRoleAsync(user, ManagerRole);
                }

            }
        }

        private static async Task CreateManager(UserManager<User> userManager)
        {
            var user = await userManager.FindByNameAsync(ManagerUserName);
            if (user == null)
            {
                user = new User()
                {
                    UserName = ManagerUserName,
                    Email = ManagerEmail,
                    FullName = ManagerFullName,
                    Age = UserDefaultAge
                };

                await userManager.CreateAsync(user, string.Format(DefaultManagerPassword, ""));
                await userManager.AddToRoleAsync(user, ManagerRole);
            }
        }

        private static async Task CreateAdmin(UserManager<User> userManager)
        {
            var user = await userManager.FindByNameAsync(AdminUserName);
            if (user == null)
            {
                user = new User()
                {
                    UserName = AdminUserName,
                    Email = AdminEmail,
                    FullName = AdminFullName,
                    Age = UserDefaultAge
                };

                await userManager.CreateAsync(user, DefaultAdminPassword);
                await userManager.AddToRoleAsync(user, AdminRole);
            }
        }

        private static async Task CreateRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.Name))
                {
                    await roleManager.CreateAsync(role);
                }
            }
        }
    }

}


