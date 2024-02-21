using Microsoft.EntityFrameworkCore;
using MvcTest.Data;
using MvcTest.Sevices.NewContextGetter;

namespace HotelAdmin.Data.Models.Order
{
    public class OrderSource
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public static async Task<List<string>> GetAllSources(MvcTestContext _context = null)
        {
            if (_context == null)
            {
                _context = ContextGetter.GetNewContext();
            }
            List<OrderSource> orderSources = await _context.OrderSource.ToListAsync();
            List<string> names = orderSources.Select(source => source.Name).ToList();
            return names;
        }
    }
}
