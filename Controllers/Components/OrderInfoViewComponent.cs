using Microsoft.AspNetCore.Mvc;
using MvcTest.Controllers;

namespace HotelAdmin.Controllers.Components
{
    public class OrderInfoViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(OrderModel model)
        {
            return View(model);
        }
    }
}
