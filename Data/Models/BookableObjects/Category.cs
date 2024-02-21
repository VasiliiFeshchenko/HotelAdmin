using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MvcTest.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelAdmin.Data.Models.BookableObjects
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfPeople { get; set; }
        public int BasePriceNumberOfPeople { get; set; }
        [Column(TypeName = "decimal(10, 2)")] public decimal BasePrice { get; set; }
        [Column(TypeName = "decimal(10, 2)")] public decimal AdditionalPrice { get; set; }
        public bool IsOwnKitchen { get; set; }
        public Hotel Hotel { get; set; }

        [NotMapped] public bool HasAdditionalPaymentBeds
        {
            get
            {
                if (BasePriceNumberOfPeople != NumberOfPeople)
                {
                    return true;
                }
                return false;
            }
        }
        
        [NotMapped] public int NumberOfAdditionalBeds
        {
            get
            {
                return NumberOfPeople - BasePriceNumberOfPeople;
            }
        }
    }
}
