namespace UrbanSolution.Web.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Threading.Tasks;
    using UrbanSolution.Models;
    using static UrbanSolutionUtilities.WebConstants;

    public static class ApplicationBuilderExtensions
    {
        private static readonly IdentityRole[] roles =
        {
            new IdentityRole(AdminRole),
            new IdentityRole(ManagerRole),
            new IdentityRole(BlogAuthorRole),
            new IdentityRole(EventCreatorRole)
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

                //create admin
                await CreateUser(userManager, AdminUserName, AdminEmail, AdminFullName, UserDefaultAge, DefaultAdminPassword, AdminRole);

                //create main manager (for all regions)
                await CreateUser(userManager, ManagerAllUserName, ManagerEmail, ManagerFullName, UserDefaultAge, string.Format(DefaultManagerPassword, ""), ManagerRole);

                await CreateRegionalManagers(userManager);

                //create blog author
                await CreateUser(userManager, BlogAuthorUserName, BlogAuthorEmail, BlogAuthorFullName, UserDefaultAge, DefaultBlogAuthorPassword, BlogAuthorRole);

                //create event creator
                await CreateUser(userManager, EventCreatorUserName, EventCreatorEmail, EventCreatorFullName, UserDefaultAge, DefaultEventCreatorPassword, EventCreatorRole);

            }
        }

        private static async Task CreateUser(UserManager<User> userManager, string userName, string email, string fullName, int age, string defaultPassword, string role)
        {
            var user = await userManager.FindByNameAsync(userName);

            if (user == null)
            {
                user = new User()
                {
                    UserName = userName,
                    Email = email,
                    FullName = fullName,
                    Age = age
                };

                if (role == ManagerRole)
                {
                    user.ManagedRegion = RegionType.All;
                }

                await userManager.CreateAsync(user, defaultPassword);
                await userManager.AddToRoleAsync(user, role);
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


