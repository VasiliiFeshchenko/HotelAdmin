using HotelAdmin.Data.Models.BookableObjects;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcTest.Controllers.Filters;
using MvcTest.Data;
using MvcTest.Models;
using MvcTest.Sevices;
using MvcTest.Sevices.MoscowTimeGetter;

namespace HotelAdmin.Controllers.Components
{
    [ServiceFilter(typeof(HotelFilter))]
    public class BookableObjectOccupationCalendarViewComponent:ViewComponent
    {
        private readonly MvcTestContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        DateTime month;
        private Hotel hotel;
        private int roomId;
        public BookableObjectOccupationCalendarViewComponent(MvcTestContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public IViewComponentResult Invoke(int roomId, DateTime? month = null)
        {
            month = month ?? new DateTime(MoscowTime.Time.Year, MoscowTime.Time.Month, 1);
            this.roomId = roomId;
            return View();
        }
        private async Task<BookableObjectOccupationCalendarModel> GetModel()
        {
            hotel = (Hotel)HttpContext.Items["Hotel"];
            DateTime start = month;
            DateTime end = month.AddMonths(1).AddDays(-1);
            RoomsReservationsTableDataGetter getter = new RoomsReservationsTableDataGetter(hotel,_context,start,end);
            BookableObject bookableObject = await _context.Room.Where(r => r.Id == roomId).FirstOrDefaultAsync();
            bookableObject = bookableObject ?? await _context.RoomCombination.Where(r => r.Id == roomId).FirstOrDefaultAsync();
            return null;
        }
    }
    public class BookableObjectOccupationCalendarModel
    {
        public string Month { get; set; }
        public string Year { get; set; }
        public TableRowData Data  { get; set; }
    }

}
