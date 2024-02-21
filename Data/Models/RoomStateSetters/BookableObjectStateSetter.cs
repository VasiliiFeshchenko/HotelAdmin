using System.ComponentModel.DataAnnotations;
using HotelAdmin.Data.Models.BookableObjects;
using MvcTest.Models;

namespace HotelAdmin.Data.Models.RoomStateSetters
{
    public abstract class BookableObjectStateSetter 
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public BookableObject BookableObject { get; set; }
        public string? Comment { get; set; }
    }
}
