using MvcTest.Models;

namespace HotelAdmin.Data.Models.Order
{
    public class Client  
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EMail { get; set; }
        public List<Order>? Orders { get; set; }
    }
}
