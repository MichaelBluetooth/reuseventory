using Microsoft.EntityFrameworkCore;
using ReuseventoryApi.Models;

namespace ReuseventoryApi.Test.TestUtils
{
    public class TestDbContextFactory
    {
        public static ReuseventoryDbContext GetContext()
        {
            var options = new DbContextOptionsBuilder<ReuseventoryDbContext>()
                     .UseInMemoryDatabase(databaseName: "ReuseventoryDbContextTest")
                     .Options;
            return new ReuseventoryDbContext(options);
        }
    }
}