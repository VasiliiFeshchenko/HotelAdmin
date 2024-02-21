using MvcTest.Controllers.ClientsControllers;
using System.Collections.Generic;

namespace MvcTest.Sevices
{
    public class CategoryCombinationsBuilder
    {
        private HashSet<List<OrderOptionCategoryRoomWithDates>> combinations = new HashSet<List<OrderOptionCategoryRoomWithDates>>();
        public CategoryCombinationsBuilder(HashSet<List<OrderOptionCategoryRoomWithDates>> combinations) 
        {
            this.combinations = combinations;
        }

        //изменить с асинхронного на обычный
        public List<List<OrderOptionCategory>> Build()
        {
            List<List<OrderOptionCategory>> combinationsOfCategories = new List<List<OrderOptionCategory>>();

            foreach (var roomsCombination in combinations)
            {
                List<OrderOptionCategory> categoriesCombination = CombinationExists(roomsCombination, combinationsOfCategories);
                if (categoriesCombination != null)
                {
                    AddRoomsToCategories(roomsCombination, categoriesCombination);
                }
                else
                {
                    combinationsOfCategories.Add(BuildCategoriesCombination(roomsCombination));
                }
            }
            return combinationsOfCategories;
        }

        private List<OrderOptionCategory> CombinationExists(List<OrderOptionCategoryRoomWithDates> roomsCombination, List<List<OrderOptionCategory>> combinationsOfCategories)
        {
            foreach (var categoriesCombination in combinationsOfCategories)
            {
                for (int i = 0; i < categoriesCombination.Count; i++)
                {
                    if (!CotegoriesEquals(roomsCombination[i], categoriesCombination[i]))
                    {
                        break;
                    }
                    if (i+1== categoriesCombination.Count)
                    {
                        return categoriesCombination;
                    }
                }
            }
            return null;
        }

        private bool CotegoriesEquals(OrderOptionCategoryRoomWithDates orderOptionCategoryRoomWithDates, OrderOptionCategory category)
        {
            if (orderOptionCategoryRoomWithDates.OrderOptionCategoryRoom.BookableObject.CategoryId != category.RootCategoryId)
            {
                return false;
            }
            if (orderOptionCategoryRoomWithDates.OrderOptionCategoryRoom.ExtraNumberOfBeds != category.ExtraNumberOfBeds)
            {
                return false;
            }
            if (orderOptionCategoryRoomWithDates.OrderOptionCategoryRoom.ExtraNumberOfPeople != category.ExtraNumberOfPeople)
            {
                return false;
            }
            return true;
        }

        private void AddRoomsToCategories(List<OrderOptionCategoryRoomWithDates> roomsCombination, List<OrderOptionCategory> categoriesCombination)
        {
            for (int i = 0; i < roomsCombination.Count; i++)
            {
                if (!categoriesCombination[i].OrderOptionCategoryRoomsWithDates.Any(room=>
                room.OrderOptionCategoryRoom.BookableObject.Id == roomsCombination[i].OrderOptionCategoryRoom.BookableObject.Id))
                {
                    categoriesCombination[i].OrderOptionCategoryRoomsWithDates.Add(roomsCombination[i]);
                }
                //if (!categoriesCombination[i].OrderOptionCategoryRoomsWithDates.Contains(roomsCombination[i]))
                //{
                //    categoriesCombination[i].OrderOptionCategoryRoomsWithDates.Add(roomsCombination[i]);
                //}
            }
        }

        private List<OrderOptionCategory> BuildCategoriesCombination(List<OrderOptionCategoryRoomWithDates> roomsCombination)
        {
            List<OrderOptionCategory> categoriesCombination = new List<OrderOptionCategory>();
            foreach (var room in roomsCombination)
            {
                OrderOptionCategory orderOptionCategory = new OrderOptionCategory();
                orderOptionCategory.OrderOptionCategoryRoomsWithDates.Add(room);
                categoriesCombination.Add(orderOptionCategory);
            }
            return categoriesCombination;
        }

    }
}
