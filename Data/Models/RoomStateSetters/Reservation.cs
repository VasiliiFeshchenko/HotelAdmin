
namespace HotelAdmin.Data.Models.RoomStateSetters
{
    public class Reservation : BookableObjectStateSetter
    {
        public Order.Order Order { get; set; }
        public int NumberofPeople { get; set; }
    }
}
