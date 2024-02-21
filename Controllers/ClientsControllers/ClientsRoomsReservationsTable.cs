using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore;
using MvcTest.Controllers.Components;
using MvcTest.Controllers.Filters;
using MvcTest.Data;
using MvcTest.Models;
using MvcTest.Sevices;
using MvcTest.Sevices.MoscowTimeGetter;
using NuGet.Protocol;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MvcTest.Controllers.ClientsControllers
{
    [ServiceFilter(typeof(HotelFilter))]
    public class ClientsRoomsReservationsTable : Controller
    {
        private readonly MvcTestContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        DateTime start;
        DateTime end;
        int totalDays;
        private Hotel hotel;
        public ClientsRoomsReservationsTable(MvcTestContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IActionResult> Index([FromQuery] int hotelId)
        {
            start = MoscowTime.Time.Date;
            end = start.AddMonths(1);
            totalDays = (int)(this.end - this.start).TotalDays;
            hotel = (Hotel)HttpContext.Items["Hotel"];
            HttpContext.Items["start"] = this.start;
            HttpContext.Items["end"] = this.end;
            await SetHttpContextItems();
            return View();
        }
        public async Task<IActionResult> Table([FromQuery] int hotelId,[FromBody] GetRowsModel model)
        {
            hotel = (Hotel)HttpContext.Items["Hotel"];
            this.start = model.Start;
            this.end = model.End;
            HttpContext.Items["start"] = this.start;
            HttpContext.Items["end"] = this.end;
            totalDays = (int)(this.end - this.start).TotalDays;
            await SetHttpContextItems();
            HttpContext.Items["IsClientTable"] = true;
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
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
