using HotelAdmin.Data.Models.Order;
using Microsoft.AspNetCore.Mvc;

namespace HotelAdmin.Controllers.Components
{
    public class TransactionListViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(IEnumerable<Order> model)
        {
            return View(model);
        }
    }
}
