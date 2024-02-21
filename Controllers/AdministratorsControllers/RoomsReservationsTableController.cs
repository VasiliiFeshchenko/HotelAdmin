using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcTest.Data;
using MvcTest.Models;
using System.Linq;
using MvcTest.Controllers.Filters;
using MvcTest.Sevices;
using Microsoft.AspNetCore.Http;
using MvcTest.Sevices.MoscowTimeGetter;

namespace MvcTest.Controllers
{
    [ServiceFilter(typeof(AuthenticationFilter))]
    [ServiceFilter(typeof(HotelFilter))]
    public class RoomsReservationsTableController : Controller
    {
        private readonly MvcTestContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private DateTime start;
        private DateTime end;
        private Hotel hotel;
        private int totalDays;

        public RoomsReservationsTableController(MvcTestContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        //The hotel Id is not used, but it still necessary for the HotelFilter
        public async Task<IActionResult> Index([FromQuery] int hotelId, DateTime? start = null, DateTime? end = null)
        {
            if (start != null)
            {
                _httpContextAccessor.HttpContext.Session.SetString("startTableDate", start.ToString());
            }
            if (end != null)
            {
                _httpContextAccessor.HttpContext.Session.SetString("endTableDate", end.ToString());
            }
            string _start = _httpContextAccessor.HttpContext.Session.GetString("startTableDate");
            string _end = _httpContextAccessor.HttpContext.Session.GetString("endTableDate");
            if (_start != null)
            {
                start = DateTime.Parse(_start);
            }
            if (_end != null)
            {
                end = DateTime.Parse(_end);
            }

            hotel = (Hotel)HttpContext.Items["Hotel"];
            this.start = (start == null ? MoscowTime.Time.Date : start.Value );
            this.end = (end == null ? this.start.AddMonths(1).Date : (DateTime)end);
            HttpContext.Items["start"] = this.start; 
            HttpContext.Items["end"] = this.end;
            totalDays = (int)(this.end - this.start).TotalDays;            
            await SetHttpContextItems();

            return View();
        }
        public async Task<IActionResult> Table([FromQuery] int hotelId,[FromBody] GetRowsModel model)
        {
            //_httpContextAccessor.HttpContext.Session.SetString("startTableDate", model.Start.ToString());
            //_httpContextAccessor.HttpContext.Session.SetString("endTableDate", model.End.ToString());
            hotel = (Hotel)HttpContext.Items["Hotel"];
            this.start = model.Start;
            this.end = model.End;
            HttpContext.Items["start"] = this.start;
            HttpContext.Items["end"] = this.end;
            totalDays = (int)(this.end - this.start).TotalDays;
            await SetHttpContextItems();
            HttpContext.Items["IsClientTable"] = false;
            return ViewComponent("RoomsReservationsTable");            
        }
        private async Task SetHttpContextItems()
        {
            RoomsReservationsTableDataGetter getter = new RoomsReservationsTableDataGetter(hotel, _context, start, end);
            (List<TableRowData>, List<DateTime>) result = await getter.Get();
            HttpContext.Items["RoomsWithDates"] = result.Item1;
            HttpContext.Items["Dates"] = result.Item2;               
        }
    }

    public class GetRowsModel
    {
        public int HotelId { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
