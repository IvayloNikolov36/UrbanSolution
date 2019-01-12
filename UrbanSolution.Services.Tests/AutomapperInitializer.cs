namespace UrbanSolution.Services.Tests
{
    using Mapping;
    using Models;
    using UrbanSolution.Models;

    public static class AutomapperInitializer
    {
        private static bool isInitialized;

        public static void Initialize()
        {
            if (isInitialized) return;

            AutoMapperConfig.RegisterMappings(
                typeof(UrbanIssuesListingServiceModel).Assembly,
                typeof(UrbanIssue).Assembly,
                typeof(BaseServiceTest).Assembly);

            isInitialized = true;
        }
    }
}
