using Microsoft.AspNetCore.Mvc;

namespace HotelAdmin.Controllers.AdministratorsControllers
{
    public class Transactions : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
