using System.ComponentModel.DataAnnotations;

namespace HotelAdmin.Data.Models.BookableObjects
{
    public class Room : BookableObject
    {
        public RoomCombination? RoomCombination { get; set; }
    }
}
