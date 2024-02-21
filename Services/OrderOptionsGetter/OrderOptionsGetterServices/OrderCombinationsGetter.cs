using MvcTest.Controllers.ClientsControllers;
using MvcTest.Data;
using MvcTest.Controllers;
using HotelAdmin.Data.Models.BookableObjects;

namespace MvcTest.Sevices
{
    public class OrderCombinationsGetter
    {
        private readonly MvcTestContext _context;
        private BookingOptionsRequestModel parameters;
        private HashSet<List<OrderOptionCategoryRoomWithDates>> combinations = new HashSet<List<OrderOptionCategoryRoomWithDates>>(new CombinationEqualityComparer());

        public OrderCombinationsGetter(MvcTestContext context, BookingOptionsRequestModel parameters)
        {
            this._context = context;
            this.parameters = parameters;
        }

        public async Task<HashSet<List<OrderOptionCategoryRoomWithDates>>> GetCombinations()
        {
            SetOptions(parameters, 0, await BookableObjectsGetter.GetBookableObjects(_context, parameters.HotelId), new List<OrderOptionCategoryRoomWithDates>());
            return combinations;
        }

        private void SetOptions(BookingOptionsRequestModel parameters, int i, List<BookableObject> rooms, List<OrderOptionCategoryRoomWithDates> option)
        {
            if (parameters.Sections.Count == i)
            {
                combinations.Add(option);
                return;
            }

            bool hasDog = parameters.Sections[i].HasDog;
            bool hasKitchen = parameters.Sections[i].HasKitchen;
            int bedsNumber = parameters.Sections[i].Adults + parameters.Sections[i].ChildrenBeds;

            for (int j = 0; j < rooms.Count; j++)
            {
                if (!CheckForDog(rooms[j], hasDog))
                {
                    continue;
                }
                if (!CheckForKitchen(rooms[j], hasKitchen))
                {
                    continue;
                }

                OrderOptionCategoryRoomWithDates roomWithDates = CreateOrderOptionCategoryRoomWithDates(parameters.Sections[i], rooms[j]);
                
                if (!PopulateRoom(roomWithDates.OrderOptionCategoryRoom, bedsNumber))
                {
                    continue;
                }

                List<BookableObject> filteredRooms = GetFilteredRooms(rooms, j);
                List<OrderOptionCategoryRoomWithDates> updatedOption = CloneOptionList(option);
                updatedOption.Add(roomWithDates);

                SetOptions(parameters, i + 1, filteredRooms, updatedOption);
            }
        }
       
        private bool PopulateRoom(OrderOptionCategoryRoom room, int bedsNumber)
        {
            if (IsEnoughBeds(room.BookableObject.Category.BasePriceNumberOfPeople, bedsNumber))
            {
                room.BaseNumberOfPeople = bedsNumber;
                return true;
            }

            room.BaseNumberOfPeople = room.BookableObject.Category.BasePriceNumberOfPeople;
            AddExtraPeopleToRoom(room, bedsNumber);
            if (IsEnoughBeds(room.TotalNumberOfPeople, bedsNumber))
            {                
                return true;
            }

            AddExtraBedsToRoom(room, bedsNumber);
            if (IsEnoughBeds(room.TotalNumberOfPeople, bedsNumber))
            {                
                return true;
            }

            return false;
        }

        private bool CheckForDog(BookableObject room, bool hasDog)
        {
            if (hasDog && !room.IsDogAllowed)
            {
                return false;
            }
            return true;
        }

        private bool CheckForKitchen(BookableObject room, bool hasKitchen)
        {
            if (hasKitchen && !room.Category.IsOwnKitchen)
            {
                return false;
            }
            return true;
        }

        private OrderOptionCategoryRoomWithDates CreateOrderOptionCategoryRoomWithDates(AccommodationSectionModel section, BookableObject room)
        {
            return new OrderOptionCategoryRoomWithDates
            {
                Start = section.Start,
                End = section.End,
                OrderOptionCategoryRoom = new OrderOptionCategoryRoom { BookableObject = room }
            };
        }

        private bool IsEnoughBeds(int occupiedBeds, int requiredBedsNumber)
        {
            return requiredBedsNumber<=occupiedBeds;
        }

        private void AddExtraPeopleToRoom(OrderOptionCategoryRoom room, int bedsNumber)
        {
            for (int y = 1; y <= room.BookableObject.Category.NumberOfAdditionalBeds; y++)
            {
                if (room.TotalNumberOfPeople + y == bedsNumber)
                {
                    room.ExtraNumberOfPeople = y;
                    return;
                }
            }
            room.ExtraNumberOfPeople = room.BookableObject.Category.NumberOfAdditionalBeds;
            return;
        }

        private void AddExtraBedsToRoom(OrderOptionCategoryRoom room, int bedsNumber)
        {
            for (int y = 1; y <= room.BookableObject.NumberOfAdditionalBeds; y++)
            {
                if (room.TotalNumberOfPeople + y == bedsNumber)
                {
                    room.ExtraNumberOfBeds = y;
                    return;
                }
            }
            room.ExtraNumberOfBeds = room.BookableObject.NumberOfAdditionalBeds;
            return;
        }

        private List<BookableObject> GetFilteredRooms(List<BookableObject> rooms, int currentIndex)
        {
            List<BookableObject> filteredRooms = new List<BookableObject>();

            for (int y = 0; y < rooms.Count; y++)
            {
                if (y != currentIndex)
                {
                    BookableObject room = rooms[y];

                    if ((room is Room && (room as Room).RoomCombination != null && (room as Room).RoomCombination.Id == rooms[currentIndex].Id) ||
                        (room is RoomCombination && rooms[currentIndex] is Room && (rooms[currentIndex] as Room).RoomCombination != null && (rooms[currentIndex] as Room).RoomCombination.Id == room.Id))
                    {
                        continue;
                    }

                    filteredRooms.Add(room);
                }
            }

            return filteredRooms;
        }

        private List<OrderOptionCategoryRoomWithDates> CloneOptionList(List<OrderOptionCategoryRoomWithDates> option)
        {
            return new List<OrderOptionCategoryRoomWithDates>(option);
        }
    }

    public class CombinationEqualityComparer : IEqualityComparer<List<OrderOptionCategoryRoomWithDates>>
    {
        //If comparison of hash codes returned true, this method is executed to ensure equality (hash codes can be the same for diferent objects)
        public bool Equals(List<OrderOptionCategoryRoomWithDates> x, List<OrderOptionCategoryRoomWithDates> y)
        {
            // Check if both lists are null or empty
            if ((x == null || x.Count == 0) && (y == null || y.Count == 0))
            {
                return true;
            }

            // Check if either list is null or empty
            if ((x == null || x.Count == 0) || (y == null || y.Count == 0))
            {
                return false;
            }

            // Check if the lists have the same count
            if (x.Count != y.Count)
            {
                return false;
            }

            // Create a dictionary to count the occurrences of each element in both lists
            Dictionary<string, int> occurrences = new Dictionary<string, int>();

            foreach (var element in x)
            {
                if (occurrences.ContainsKey(element.Identifier))
                {
                    occurrences[element.Identifier]++;
                }
                else
                {
                    occurrences[element.Identifier] = 1;
                }
            }

            foreach (var element in y)
            {
                if (occurrences.ContainsKey(element.Identifier))
                {
                    occurrences[element.Identifier]--;
                    if (occurrences[element.Identifier] == 0)
                    {
                        occurrences.Remove(element.Identifier);
                    }
                }
                else
                {
                    return false;
                }
            }

            // If the dictionary is empty, it means all elements matched
            return occurrences.Count == 0;
        }
        
        //The function is used to determine the identity of List<OrderOptionCategoryRoomWithDates> objects.
        //If two different instances return the same HashCode that means that they are equal
        public int GetHashCode(List<OrderOptionCategoryRoomWithDates> obj)
        {
            if (obj == null || obj.Count == 0)
            {
                return 0;
            }


            int hashCode = 0;

            foreach (var element in obj)
            {
                hashCode += element.Identifier.GetHashCode();
            }

            return hashCode;
        }
    }
}
