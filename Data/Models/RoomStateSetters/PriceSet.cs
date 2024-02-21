using HotelAdmin.Data.Models.Unused;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelAdmin.Data.Models.RoomStateSetters
{
    public class PriceSet : BookableObjectStateSetter
    {
        [Column(TypeName = "decimal(10, 2)")] public decimal Price { get; set; }
        public Currency Currency { get; set; }
    }
}
