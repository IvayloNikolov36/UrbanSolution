namespace UrbanSolution.Services.Tests.Mocks
{
    using Microsoft.Extensions.Configuration;

    public class ConfigurationMock
    {
        public static IConfiguration New()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.AddJsonFile(
                @"C:\Users\Ivaylo\source\repos\UrbanSolution\UrbanSolution.Services.Tests\Mocks\MockEntities\mockAppsettings.json");

            IConfiguration configuration = configurationBuilder.Build();

            return configuration;
        } 
        
    }
}
