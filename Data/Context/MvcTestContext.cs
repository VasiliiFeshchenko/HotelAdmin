using HotelAdmin.Data.Models.BookableObjects;
using HotelAdmin.Data.Models.Order;
using HotelAdmin.Data.Models.RoomStateSetters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MvcTest.Models;
using MvcTest.Models.HotelManagerModels;
using System.Reflection.Metadata;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using HotelAdmin.Data.Models;

namespace MvcTest.Data
{
    public class MvcTestContext:DbContext
    {
        public MvcTestContext(DbContextOptions<MvcTestContext> options) : base(options)
        {
        }

        public DbSet<Room> Room { get; set; } = default!;
        public DbSet<RoomCombination> RoomCombination { get; set; } = default!;
        public DbSet<Category> Category { get; set; } = default!;

        public DbSet<Hotel> Hotel { get; set; } = default!;
        public DbSet<Client> Client { get; set; } = default!;
        public DbSet<Order> Order { get; set; } = default!;
        public DbSet<OrderSource> OrderSource { get; set; } = default!;

        public DbSet<Reservation> Reservation { get; set; } = default!;
        public DbSet<PriceSet> PriceSet { get; set; } = default!;
        public DbSet<ClosureSet> ClosureSet { get; set; } = default!;
        public DbSet<BookableObjectStateSetter> BookableObjectStateSetter { get; set; } = default!;

        public DbSet<EmailCredentials> EmailCredentials { get; set; } = default!;

        public DbSet<KeyCode> keyCodes { get; set; } = default!;

        public DbSet<MoneyTransaction> Transaction { get; set; } = default!;
        public DbSet<TransactionMethod> TransactionMethod { get; set; } = default!;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

            string connectionString = configuration.GetConnectionString("Main");

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 32));
            optionsBuilder.UseMySql(connectionString, serverVersion);

            // Log connection pool statistics
            //var loggerFactory = LoggerFactory.Create(builder =>
            //{
            //    builder.AddConsole();
            //});

            //optionsBuilder.UseLoggerFactory(loggerFactory);
            //optionsBuilder.EnableDetailedErrors();
        }
    }
}
