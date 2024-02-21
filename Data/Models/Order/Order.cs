using HotelAdmin.Data.Models.RoomStateSetters;
using HotelAdmin.Data.Models.Unused;
using Microsoft.EntityFrameworkCore;
using MvcTest.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.IdentityModel.Tokens.Jwt;
using System.Transactions;
using System.Xml.Schema;

namespace HotelAdmin.Data.Models.Order
{
    public enum PaymentStatus
    {
        payed,
        prepayed,
        partiallyPrepayed,
        notPayed
    }
    public class Order  
    {
        public int Id { get; set; }
        public Client Client { get; set; }
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
        public Currency Currency { get; set; }
        [Column(TypeName = "decimal(10, 2)")] public decimal Price { get; set; }
        [Column(TypeName = "decimal(10, 2)")] public decimal PayedPrice { get; set; }
        public string? Comment { get; set; }
        [Column(TypeName = "decimal(10, 2)")] public decimal Prepayment { get; set; }
        public DateTime PrepaymentDeadline { get; set; }
        public int HotelId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public OrderSource? Source { get; set; }
        public int? SourceId { get; set; }
        public bool IsCanceled { get; set; } = false;
        public DateTime? CancelationDate { get; set; } = null;
        public List<MoneyTransaction> MoneyTransactions { get; set; } = new List<MoneyTransaction>();


        [NotMapped] public PaymentStatus PaymentStatus
        {
            get
            {
                if (Price <= PaymentSum)
                {
                    return PaymentStatus.payed;
                }
                if (PaymentSum >= Prepayment)
                {
                    return PaymentStatus.prepayed;
                }
                if (PaymentSum < Prepayment && PaymentSum != 0)
                {
                    return PaymentStatus.partiallyPrepayed;
                }
                return PaymentStatus.notPayed;
            }
        }
        [NotMapped] public string Information //returns html for the email
        {
            get
            {
                string result = "";
                int i = 0;
                foreach (var item in Reservations)
                {
                    result += "<li>";
                    result += item.BookableObject.Category.Name;
                    if (item.BookableObject.Category.NumberOfPeople < item.NumberofPeople)
                    {
                        int addBeds = item.NumberofPeople - item.BookableObject.Category.NumberOfPeople;
                        result += " + ";
                        if (addBeds == 1)
                        {
                            result += "1 дополнительное спальное место";
                        }
                        else
                        {
                            result += addBeds + " дополнительных спальных места";
                        }
                    }
                    result += " (";

                    result += $"<a href=\"{item.BookableObject.RoomUrl}\">";
                    result += item.BookableObject.Name;
                    result += "</a>, ";

                    result += $"заезд {item.Start.ToString("dd.MM.yyyy")}, выезд {item.End.AddDays(1).ToString("dd.MM.yyyy")}, ";
                    result += $"количество спальных мест: {item.NumberofPeople}";
                    result += ")";
                    if (i != Reservations.Count - 1)
                    {
                        result += ",";
                    }
                    else
                    {
                        result += ".";
                    }
                    result += "</li>";
                    i++;
                }
                return result;
            }
        }
        //public async Task<bool> IsOverlapping(MvcTestContext context)
        //{
        //    foreach (var item in Reservations)
        //    {
        //        bool overlaps = await context.BookableObjectStateSetter.AnyAsync
        //            (setter => !
        //                (setter is PriceSet) &&
        //                setter.BookableObject.Id == item.BookableObject.Id &&
        //                setter.Start <= item.End && setter.End >= item.Start
        //            );
        //        if (overlaps)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        [NotMapped]
        public decimal PaymentSum
        {
            get
            {
                return MoneyTransactions.Where(t=>!t.IsRefund).Sum(t => t.Amount)- MoneyTransactions.Where(t => t.IsRefund).Sum(t => t.Amount)+ PayedPrice;
            }
        }

        public void SetCreationDate()
        {
            DateTimeOffset serverTime = DateTimeOffset.Now;
            TimeZoneInfo moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            DateTimeOffset moscowTime = TimeZoneInfo.ConvertTime(serverTime, moscowTimeZone);
            DateTime moscowDateTime = moscowTime.DateTime;
            CreationDate = moscowDateTime;
        }
        public void SetLastUpdateDate()
        {
            DateTimeOffset serverTime = DateTimeOffset.Now;
            TimeZoneInfo moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            DateTimeOffset moscowTime = TimeZoneInfo.ConvertTime(serverTime, moscowTimeZone);
            DateTime moscowDateTime = moscowTime.DateTime;
            LastUpdateDate = moscowDateTime;
        }
    }
}
