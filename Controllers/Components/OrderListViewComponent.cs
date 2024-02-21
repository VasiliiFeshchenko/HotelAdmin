using HotelAdmin.Data.Models.Order;
using Microsoft.AspNetCore.Mvc;
using MvcTest.Controllers;

namespace HotelAdmin.Controllers.Components
{
    public class OrderListViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IEnumerable<Order> model)
        {
            return View(model);
        }
    }
}
