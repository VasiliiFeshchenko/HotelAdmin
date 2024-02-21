using HotelAdmin.Data.Models.BookableObjects;

namespace MvcTest.Models
{
    public class Hotel  
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool ShowPetCheckbox { get; set; }
        public string ImageUrl { get; set; }
        public string ParametersImageUrl { get; set; }
        public List<Room> Rooms { get; set; }
    }
}
