using HotelAdmin.Data.Models.BookableObjects;
using Microsoft.EntityFrameworkCore;
using MvcTest.Data;

namespace MvcTest.Sevices
{
    public class BookableObjectsGetter
    {
        public static async Task<List<BookableObject>> GetBookableObjects(MvcTestContext _context, int hotelId, int? categoryId = null)
        {
            List<Room> rooms = new List<Room>();
            List<RoomCombination> roomCombinations = new List<RoomCombination>();
            if (categoryId==null)
            {
                rooms = await _context.Room.Where(r => r.HotelId == hotelId).Include(r => r.Category).ToListAsync();
                roomCombinations = await _context.RoomCombination.Where(r => r.HotelId == hotelId).Include(r => r.Category).ToListAsync();
            }
            else
            {
                rooms = await _context.Room.Where(r => r.HotelId == hotelId && r.CategoryId == categoryId).Include(r => r.Category).ToListAsync();
                roomCombinations = await _context.RoomCombination.Where(r => r.HotelId == hotelId && r.CategoryId == categoryId).Include(r => r.Category).ToListAsync();
            }
            List<BookableObject> allRooms = new List<BookableObject>();            
            allRooms.AddRange(rooms);
            allRooms.AddRange(roomCombinations);
            return allRooms;
        }
        
    }
}
