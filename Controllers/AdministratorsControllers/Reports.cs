using HotelAdmin.Services.Reports;
using Microsoft.AspNetCore.Mvc;
using MvcTest.Controllers.Filters;
using MvcTest.Data;
using MvcTest.Models;
using OfficeOpenXml;

namespace HotelAdmin.Controllers.AdministratorsControllers
{
    [ServiceFilter(typeof(HotelFilter))]
    public class Reports : Controller
    {
        private readonly MvcTestContext _context;
        public Reports(MvcTestContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index( int hotelId)
        {
            Hotel hotel = (Hotel)HttpContext.Items["Hotel"];
            return View();
        }
        public async Task<IActionResult> GeneralFinantialReport([FromQuery] int hotelId, DateTime? date = null)
        {
            Hotel hotel = (Hotel)HttpContext.Items["Hotel"];
            byte[] excelFileContents;
            date = date ?? DateTime.Now.Date;
            GeneralFinancialReport report = new GeneralFinancialReport((DateTime)date, hotel, _context);
            ExcelPackage package = await report.GenerateExcelReportAsync();
            //ExcelPackage package = new ExcelPackage();
            //ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");
            excelFileContents = package.GetAsByteArray();

            string name = "Отчет " + ((DateTime)date).ToString("dd.MM.yy")+" "+hotel.Name+ ".xlsx";
            return File(excelFileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", name);
        }
    }
}
