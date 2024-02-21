using HotelAdmin.Data.Models.BookableObjects;
using HotelAdmin.Data.Models.RoomStateSetters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcTest.Controllers.Filters;
using MvcTest.Data;
using MvcTest.Models;
using MvcTest.Sevices;


namespace MvcTest.Controllers
{
    [ServiceFilter(typeof(AuthenticationFilter))]
    [ServiceFilter(typeof(HotelFilter))]
    public class SetPrice : Controller
    {
        Hotel hotel = null;
        private readonly MvcTestContext _context;
        private string redirectString = "/RoomsReservationsTable/Index?hotelId=";
        public SetPrice(MvcTestContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Create(int hotelId)
        {            
            List<Category> categories = await _context.Category.Where(c=>c.Hotel.Id== hotelId).ToListAsync();
            HttpContext.Items["Categories"] = categories;
            return View();
        }

        public async Task<string> CreateRoot([FromBody] PriceSetModel model)
        {
            if (model == null)
            {
                return "model is null!";
            }
            string isValidInputs = await IsValidInputs(model);
            if (isValidInputs == null)
            {
                List<PriceSet> priceSets = await AssamblePriceSets(model);
                for (int i = 0; i < priceSets.Count; i++)
                {
                    await AdjustPriceSets(priceSets[i]);
                    await _context.BookableObjectStateSetter.AddAsync(priceSets[i]);
                    await _context.SaveChangesAsync();
                }
                return redirectString + model.HotelId;
            }
            return isValidInputs;

        }

        //resolve return null issue
        private async Task<string> IsValidInputs(PriceSetModel model)
        {
            if (model.Start.Year == 1 || model.End.Year == 1)
            {
                return "Установите конечную и начальную даты!";
            }
            if (model.Start > model.End)
            {
                return "Начальная дата больше конечной!";
            }
            if (model.Price < 0)
            {
                return "Вы пытаетесь установить отрицательную цену!";
            }
            if (model.Start.Year==1|| model.End.Year == 1)
            {
                return "Укажите все даты";
            }
            return null;
        }
        private async Task<List<PriceSet>> AssamblePriceSets(PriceSetModel model)
        {
            Category category = await _context.Category.Where(c => c.Name.Trim() == model.Category && c.Hotel.Id == model.HotelId).FirstOrDefaultAsync();
            List<BookableObject> bookableObjects = await BookableObjectsGetter.GetBookableObjects(_context, model.HotelId, category.Id);
            List<PriceSet> priceSets = new List<PriceSet>();
            for (int i = 0; i < bookableObjects.Count; i++)
            {
                PriceSet priceSet = new PriceSet()
                {
                    BookableObject = bookableObjects[i],
                    Price = model.Price,
                    Start = model.Start,
                    End = model.End,
                    Comment = model.Comment
                };
                priceSets.Add(priceSet);
            }

            return priceSets;
        }
        private async Task AdjustPriceSets(PriceSet priceSet)
        {
            #region Find overlaps with PriceSets
            //The priceSet is deviding already existing by two parts
            List<BookableObjectStateSetter> innerOverlaps = await _context.BookableObjectStateSetter
                .Where(o => o is PriceSet
                && (o.BookableObject.Name == priceSet.BookableObject.Name)
                && (o.Start.Date < priceSet.Start.Date && o.End.Date > priceSet.End.Date))
                .ToListAsync();

            //The priceSet includes already existing one
            List<BookableObjectStateSetter> fullOuterOverlaps = await _context.BookableObjectStateSetter
                .Where(o => o is PriceSet
                && (o.BookableObject.Name == priceSet.BookableObject.Name)
                && (o.Start.Date >= priceSet.Start.Date && o.End.Date <= priceSet.End.Date))
                .ToListAsync();

            //The priseSet takes first dates of already existing one
            List<BookableObjectStateSetter> firstOuterOverlaps = await _context.BookableObjectStateSetter
                .Where(o => o is PriceSet
                && (o.BookableObject.Name == priceSet.BookableObject.Name)
                && (o.Start.Date >= priceSet.Start.Date && o.Start.Date <= priceSet.End.Date)
                && !(o.Start.Date >= priceSet.Start.Date && o.End.Date <= priceSet.End.Date))
                .ToListAsync();

            //The priseSet takes last dates of already existing one
            List<BookableObjectStateSetter> lastOuterOverlaps = await _context.BookableObjectStateSetter
                .Where(o => o is PriceSet
                && (o.BookableObject.Name == priceSet.BookableObject.Name)
                && (o.End.Date <= priceSet.End.Date && o.End.Date >= priceSet.Start.Date)
                && !(o.Start.Date >= priceSet.Start.Date && o.End.Date <= priceSet.End.Date))
                .ToListAsync();
            #endregion
            #region Anjust overlaps
            //Handle innerOverlaps
            foreach (var item in innerOverlaps)
            {
                PriceSet secondPriceSet = new PriceSet
                {
                    Start = priceSet.End.AddDays(1),
                    End = item.End,
                    BookableObject = item.BookableObject,
                    Comment= item.Comment,
                    Price = (item as PriceSet).Price,
                    Currency = (item as PriceSet).Currency                    
                };                
                item.End = priceSet.Start.AddDays(-1);
                await _context.BookableObjectStateSetter.AddAsync(secondPriceSet);
            }
            //Handle fullOuterOverlaps
            _context.BookableObjectStateSetter.RemoveRange(fullOuterOverlaps);

            //Handle firstOuterOverlaps
            foreach (var item in firstOuterOverlaps)
            {
                item.Start = priceSet.End.Date.AddDays(1);
            }
            //Handle lastOuterOverlaps
            foreach (var item in lastOuterOverlaps)
            {
                item.End = priceSet.Start.Date.AddDays(-1);
            }
            #endregion
        }
    }
    public class PriceSetModel
    {
        public string Category { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Price { get; set; }
        public string Comment { get; set; }
        public int HotelId { get; set; }
    }
}
