using HotelAdmin.Data.Models.BookableObjects;
using HotelAdmin.Data.Models.RoomStateSetters;
using Microsoft.EntityFrameworkCore;
using MvcTest.Controllers.ClientsControllers;
using MvcTest.Data;
using System.Collections.Concurrent;

namespace MvcTest.Sevices
{
    public class OrderCombinationsReservationsChecker
    {
        private readonly MvcTestContext _context;
        private HashSet<List<OrderOptionCategoryRoomWithDates>> combinations = new HashSet<List<OrderOptionCategoryRoomWithDates>>();
        public OrderCombinationsReservationsChecker(MvcTestContext context, HashSet<List<OrderOptionCategoryRoomWithDates>> combinations)
        {
            this._context = context;
            this.combinations = combinations;
        }

        public async Task<HashSet<List<OrderOptionCategoryRoomWithDates>>> CheckConcurrently()
        {
            List<(List<OrderOptionCategoryRoomWithDates> Combination, Task<bool> Task)> tasks = combinations.Select(combination =>
                (Combination: combination, Task: IsCombinationOverlappingWithReservations(combination))).ToList();

            await Task.WhenAll(tasks.Select(task => task.Task));

            HashSet<List<OrderOptionCategoryRoomWithDates>> _combinations = new HashSet<List<OrderOptionCategoryRoomWithDates>>(
                tasks.Where(task => task.Task.Result == false).Select(task => task.Combination));
            return _combinations;
        }
        public HashSet<List<OrderOptionCategoryRoomWithDates>> Check()
        {
            List<(List<OrderOptionCategoryRoomWithDates> Combination, bool Result)> results = new List<(List<OrderOptionCategoryRoomWithDates>, bool)>();

            foreach (var combination in combinations)
            {
                bool isOverlapping = IsCombinationOverlappingWithReservations(combination).Result;
                results.Add((combination, isOverlapping));
            }

            HashSet<List<OrderOptionCategoryRoomWithDates>> _combinations = new HashSet<List<OrderOptionCategoryRoomWithDates>>(
                results.Where(result => !result.Result).Select(result => result.Combination));

            return _combinations;
        }
        private async Task<bool> IsCombinationOverlappingWithReservations(List<OrderOptionCategoryRoomWithDates> combination)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            string connectionString = configuration.GetConnectionString("Main");
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 32));
            using (var context = new MvcTestContext(new DbContextOptionsBuilder<MvcTestContext>()
                .UseMySql(connectionString, serverVersion)
                .Options))
            {
                foreach (var roomWithDates in combination)
                {
                    bool isOverlapping = await context.BookableObjectStateSetter.AnyAsync(stateSetter =>
                        !(stateSetter is PriceSet) &&
                        (!(stateSetter is Reservation) || !((stateSetter as Reservation).Order.IsCanceled)) &&

                        //If both stateSetter and rooWith dates are Rooms or RoomCombinations
                        (stateSetter.BookableObject.Id == roomWithDates.OrderOptionCategoryRoom.BookableObject.Id ||

                         //If stateSetter is RoomCombination and roomWithDates is Room
                         (stateSetter.BookableObject is RoomCombination &&
                          roomWithDates.OrderOptionCategoryRoom.BookableObject is Room &&
                          (roomWithDates.OrderOptionCategoryRoom.BookableObject as Room).RoomCombination != null &&
                          (roomWithDates.OrderOptionCategoryRoom.BookableObject as Room).RoomCombination.Id ==
                          stateSetter.BookableObject.Id &&
                          stateSetter is Reservation) ||

                        //If stateSetter is Room and roomWithDates is RoomCombination
                        (stateSetter.BookableObject is Room &&
                         (stateSetter.BookableObject as Room).RoomCombination!=null &&
                         (stateSetter.BookableObject as Room).RoomCombination.Id == roomWithDates.OrderOptionCategoryRoom.BookableObject.Id)) &&

                        stateSetter.Start <= roomWithDates.End &&
                        stateSetter.End >= roomWithDates.Start);

                    if (isOverlapping)
                    {
                        return true;
                    }
                }
            }

            return false; // Combination is not overlapping
        }

        //public async Task Check()
        //{            
        //    foreach (var combination in combinations)
        //    {
        //        bool isOverlapping = await IsCombinationOverlappingWithReservations(combination);
        //        if (isOverlapping)
        //        {
        //            combinations.Remove(combination);
        //        }
        //    }
        //}

        //private async Task<bool> IsCombinationOverlappingWithReservations(List<OrderOptionCategoryRoomWithDates> combination)
        //{
        //    foreach (var roomWithDates in combination)
        //    {
        //        bool isOverlapping = await _context.BookableObjectStateSetter.AnyAsync(stateSetter =>
        //            !(stateSetter is PriceSet) &&
        //            (stateSetter.BookableObject.Id == roomWithDates.OrderOptionCategoryRoom.BookableObject.Id ||
        //             (stateSetter.BookableObject is RoomCombination &&
        //              roomWithDates.OrderOptionCategoryRoom.BookableObject is Room &&
        //              (roomWithDates.OrderOptionCategoryRoom.BookableObject as Room).RoomCombination != null &&
        //              (roomWithDates.OrderOptionCategoryRoom.BookableObject as Room).RoomCombination.Id ==
        //              stateSetter.BookableObject.Id)) &&
        //            stateSetter.Start <= roomWithDates.End &&
        //            stateSetter.End >= roomWithDates.Start);

        //        if (isOverlapping)
        //        {
        //            return true;
        //        }
        //    }

        //    return false; // Combination is not overlapping
        //}
    }
}
