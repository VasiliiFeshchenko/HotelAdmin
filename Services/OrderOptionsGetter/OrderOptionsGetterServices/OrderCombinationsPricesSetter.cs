using Microsoft.EntityFrameworkCore;
using MvcTest.Controllers.ClientsControllers;
using MvcTest.Data;
using System.Threading.Tasks;

namespace MvcTest.Sevices.OrderOptionsGetterServices
{
    public class OrderCombinationsPricesSetter
    {
        private MvcTestContext _context;
        private List<List<OrderOptionCategory>> combinationsOfCategories;

        public OrderCombinationsPricesSetter(MvcTestContext context, List<List<OrderOptionCategory>> combinationsOfCategories)
        {
            this._context = context;
            this.combinationsOfCategories = combinationsOfCategories;
        }

        public async Task Set()
        {
            //List<Task> tasks = new List<Task>();
            //Parallel.ForEach(this.combinationsOfCategories, async (categoriesCombination) =>
            //{
            //    IConfigurationRoot configuration = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json")
            //    .Build();
            //    string connectionString = configuration.GetConnectionString("Main");
            //    var serverVersion = new MySqlServerVersion(new Version(8, 0, 32));
            //    using (var context = new MvcTestContext(new DbContextOptionsBuilder<MvcTestContext>()
            //        .UseMySql(connectionString, serverVersion)
            //        .Options))
            //    {
            //        foreach (var category in categoriesCombination)
            //        {
            //            Task task = category.SetPrice(context);
            //            tasks.Add(task);
            //        }
            //    }
            //});
            //await Task.WhenAll(tasks);
            foreach (var categoriesCombination in this.combinationsOfCategories)
            {
                foreach (var category in categoriesCombination)
                {
                    await category.SetPrice(_context);
                }
            }
        }
    }
}
