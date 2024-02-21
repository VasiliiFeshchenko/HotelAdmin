using Microsoft.AspNetCore.Mvc;

namespace MvcTest.Controllers.Components
{
    public class RoomsReservationsTableViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
