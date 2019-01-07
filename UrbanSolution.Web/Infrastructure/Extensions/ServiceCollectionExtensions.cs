namespace UrbanSolution.Web.Infrastructure.Extensions
{
    using Filters;
    using Microsoft.Extensions.DependencyInjection;
    using System.Linq;
    using System.Reflection;
    using Services;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainServices(
            this IServiceCollection services)
        {
            Assembly
                .GetAssembly(typeof(IService))
                .GetTypes()
                .Where(t => t.IsClass && t.GetInterfaces().Any(i => i.Name == $"I{t.Name}"))
                .Select(t => new
                {
                    Interface = t.GetInterface($"I{t.Name}"),
                    Implementation = t
                })
                .ToList()
                .ForEach(s => services.AddScoped(s.Interface, s.Implementation));

            services.AddTransient<ValidateIssueIdExistsAttribute>();
            services.AddTransient<ValidateManagerIsMainManagerAttribute>();
            services.AddTransient<ValidateUserAndRoleExistsAttribute>();
            services.AddTransient<ValidateArticleIdExistsAttribute>();
            services.AddTransient<ValidateEventIdExistsAttribute>();

            return services;
        }
    }
}
