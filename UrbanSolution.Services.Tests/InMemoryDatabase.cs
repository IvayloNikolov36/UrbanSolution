namespace UrbanSolution.Services.Tests
{
    using Data;
    using Microsoft.EntityFrameworkCore;
    using System;
    
    public class InMemoryDatabase
    {
        public static UrbanSolutionDbContext Get()
        {
            var dbOptions = new DbContextOptionsBuilder<UrbanSolutionDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new UrbanSolutionDbContext(dbOptions);
        }
    }
}
