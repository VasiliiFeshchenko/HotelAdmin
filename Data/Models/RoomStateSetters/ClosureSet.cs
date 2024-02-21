using System.ComponentModel.DataAnnotations.Schema;

namespace HotelAdmin.Data.Models.RoomStateSetters
{
    public enum ClosureType
    {
        normal,
        combined
    }
    public class ClosureSet : BookableObjectStateSetter
    {
        [NotMapped] public ClosureType Type { get; set; } = ClosureType.normal;
    }
}
