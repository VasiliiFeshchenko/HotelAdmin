using HotelAdmin.Data.Models.RoomStateSetters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MvcTest.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HotelAdmin.Data.Models.BookableObjects
{
    public abstract class BookableObject 
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? RoomUrl { get; set; }
        public bool IsDogAllowed { get; set; }
        public int NumberOfAdditionalBeds { get; set; }
        [Column(TypeName = "decimal(10, 2)")]  public decimal AdditionalPrice { get; set; }

        [JsonIgnore] public List<BookableObjectStateSetter>? RoomStateSetters { get; set; }

        public Hotel Hotel { get; set; }
        public int HotelId { get; set; }

        [NotMapped] public bool HasExtraBeds
        {
            get
            {
                if (NumberOfAdditionalBeds != 0)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
