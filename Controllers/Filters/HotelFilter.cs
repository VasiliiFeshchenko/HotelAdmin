using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using MvcTest.Data;
using Microsoft.EntityFrameworkCore;

namespace MvcTest.Controllers.Filters
{
    public class HotelFilter : IAsyncActionFilter
    {
        private readonly MvcTestContext _context;

        public HotelFilter(MvcTestContext context)
        {
            _context = context;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionArguments.TryGetValue("hotelId", out var hotelId)){}
            else
            {
                hotelId = 1;
            }
            var hotel = await _context.Hotel.Where(h => h.Id == (int)hotelId).FirstOrDefaultAsync();
            if (hotel == null)
            {
                context.Result = new NotFoundResult();
                return;
            }

            context.HttpContext.Items["Hotel"] = hotel;
            context.HttpContext.Items["Hotels"] = await _context.Hotel.ToListAsync();
            await next();
        }
    }
}
