
using HotelAdmin.Services.IsProductionChecker;
using MvcTest.Controllers.ClientsControllers;
using MvcTest.Data;
using MvcTest.Sevices.OrderOptionsGetterServices;

namespace MvcTest.Sevices
{
    public class OrderOptionsGetter
    {
        private readonly MvcTestContext _context;
        private BookingOptionsRequestModel parameters;
        private HashSet<List<OrderOptionCategoryRoomWithDates>> combinations = new HashSet<List<OrderOptionCategoryRoomWithDates>>();
        
        public OrderOptionsGetter (MvcTestContext context, BookingOptionsRequestModel parameters)
        {
            this._context= context;
            this.parameters= parameters;
        }

        public async Task<List<OrderOption>> GetOptions()
        {
            //Set combinations (without repetition of items)
            OrderCombinationsGetter orderCombinationsGetter = new OrderCombinationsGetter(_context,this.parameters);
            combinations = await orderCombinationsGetter.GetCombinations();

            //Check for overlapping reservations
            OrderCombinationsReservationsChecker checker = new OrderCombinationsReservationsChecker(_context, combinations);
            if (AppState.IsProduction)
            {
                combinations = await checker.CheckConcurrently();
            }
            else
            {
                combinations = checker.Check();
            }

            //Accemble category combinations
            CategoryCombinationsBuilder categoryCombinationsBuilder = new CategoryCombinationsBuilder(combinations);
            List<List<OrderOptionCategory>> combinationsOfCategories = categoryCombinationsBuilder.Build();

            //Set prices for category combinations
            OrderCombinationsPricesSetter orderCombinationsPricesSetter = new OrderCombinationsPricesSetter(_context, combinationsOfCategories);
            await orderCombinationsPricesSetter.Set();

            //Create and order Order Options
            List<OrderOption> orderOptions = new List<OrderOption>();
            foreach (var categoriesCombination in combinationsOfCategories)
            {
                orderOptions.Add(new OrderOption { OrderOptionCategories = categoriesCombination });
            }
            return orderOptions.OrderBy(option=>option.Price).ToList();            
        }
    }
}
