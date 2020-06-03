namespace UrbanSolution.Services.Tests
{
    using Mapping;
    using Models;
    using UrbanSolution.Web.Models.Issues;

    public static class AutomapperInitializer
    {
        private static bool isInitialized;

        public static void Initialize()
        {
            if (isInitialized) return;

            AutoMapperConfig.RegisterMappings(
                typeof(IssuesListingModel).Assembly,
                typeof(UrbanIssue).Assembly,
                typeof(BaseServiceTest).Assembly);

            isInitialized = true;
        }
    }
}
