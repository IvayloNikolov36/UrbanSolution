namespace UrbanSolution.Services.Tests
{
    using Data;

    public class BaseServiceTest
    {
        protected BaseServiceTest()
        {
            this.Db = InMemoryDatabase.Get();

            AutomapperInitializer.Initialize();
        }

        protected UrbanSolutionDbContext Db;
    }
}
