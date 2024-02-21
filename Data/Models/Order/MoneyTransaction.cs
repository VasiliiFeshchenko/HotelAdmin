using System.ComponentModel.DataAnnotations.Schema;

namespace HotelAdmin.Data.Models.Order
{
    public class MoneyTransaction
    {
        public int Id { get; set; }
        public Order Order { get; set; }       
        [Column(TypeName = "decimal(10, 2)")] public decimal Amount { get; set; }
        public TransactionMethod TransactionMethod { get; set; }
        public bool IsRefund { get; set; } = false;
        public DateTime Date { get; set; }
        public string Comment { get; set; }
    }
}
