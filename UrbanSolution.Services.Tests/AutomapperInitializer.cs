namespace UrbanSolution.Services.Tests
{
    using Mapping;
    using Models;
    using UrbanSolution.Models;

    public class AutomapperInitializer
    {
        private static bool isInitialized = false;

        public static void Initialize()
        {
            if (!isInitialized)
            {
                AutoMapperConfig.RegisterMappings(
                    typeof(UrbanIssuesListingServiceModel).Assembly,
                    typeof(UrbanIssue).Assembly);

                isInitialized = true;
            }
        }
    }
}
