using Microsoft.EntityFrameworkCore;
using MvcTest.Data;

namespace MvcTest.Sevices.NewContextGetter
{
    public class ContextGetter
    {
        public static MvcTestContext GetNewContext()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
  .SetBasePath(Directory.GetCurrentDirectory())
  .AddJsonFile("appsettings.json")
  .Build();
            string connectionString = configuration.GetConnectionString("Main");            
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 32));
            var newContext = new MvcTestContext(new DbContextOptionsBuilder<MvcTestContext>()
                .UseMySql(connectionString, serverVersion)
                .Options);
            return newContext;
        }
    }
}
