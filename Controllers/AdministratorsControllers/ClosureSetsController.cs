using HotelAdmin.Data.Models.BookableObjects;
using HotelAdmin.Data.Models.RoomStateSetters;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcTest.Controllers.Filters;
using MvcTest.Data;
using MvcTest.Models;
using MvcTest.Sevices;

namespace MvcTest.Controllers
{
    //This is a controller made just for testing of different features
    [ServiceFilter(typeof(AuthenticationFilter))]
    [ServiceFilter(typeof(HotelFilter))]
    public class ClosureSetsController : Controller
    {
        Hotel hotel = null;
        private readonly MvcTestContext _context;
        public ClosureSetsController(MvcTestContext context)
        {
            _context = context;
        }
        private string redirectString = "/RoomsReservationsTable/Index?hotelId=";

        public async Task<IActionResult> Create(int hotelId)
        {
            List<BookableObject> bookableObjects = await BookableObjectsGetter.GetBookableObjects(_context, hotelId);
            HttpContext.Items["Rooms"] = bookableObjects;
            return View();
        }
        public async Task<IActionResult> Delete(int closureSetId, int hotelId)
        {
            ClosureSet closureSet = (ClosureSet)await _context.BookableObjectStateSetter.FindAsync(closureSetId);
            if (closureSet != null)
            {
                _context.BookableObjectStateSetter.Remove(closureSet);
                _context.SaveChanges();
            }
            return RedirectToAction("Index", "RoomsReservationsTable", new { hotelId = hotelId });
        }

        public async Task<string> CreateRoot([FromBody] ClosureSetModel model)
        {
            if (model == null)
            {
                return "model is null!";
            }
            string isValidInputs = await IsValidInputs(model);
            if (isValidInputs == null)
            {
                ClosureSet closureSet = await AssambleClosureSet(model);
                string isOverlapping = await IsOverlapping(closureSet);
                if (isOverlapping == null)
                {
                    await AdjustClosureSets(closureSet);
                    await _context.BookableObjectStateSetter.AddAsync(closureSet);
                    await _context.SaveChangesAsync();
                    return redirectString + model.HotelId;
                }
                return isOverlapping;
            }
            return isValidInputs;
        }

        private async Task<string> IsOverlapping(ClosureSet set)
        {
            //Find reservations that take this room for these days
            List<BookableObjectStateSetter> overlaps = await _context.BookableObjectStateSetter
                .Where(stateSetter => stateSetter is Reservation
                && !(stateSetter as Reservation).Order.IsCanceled
                && stateSetter.BookableObject == set.BookableObject
                && (stateSetter.Start <= set.End.Date && stateSetter.End >= set.Start.Date))
                .ToListAsync();
            if (overlaps.Count!=0)
            {
                return "Вы пытаетесь закрыть номер заронированный на эти даты";
            }
            return null;
        }
        private async Task<string> IsValidInputs(ClosureSetModel model)
        {
            if (model.Start.Year == 1 || model.End.Year == 1)
            {
                return "Установите конечную и начальную даты!";
            }
            if (model.Start > model.End)
            {
                return "Начальная дата больше конечной!";
            }
            if (model.Start.Year == 1 || model.End.Year == 1)
            {
                return "Укажите все даты";
            }
            return null;
        }
        private async Task<ClosureSet> AssambleClosureSet(ClosureSetModel model)
        {
            BookableObject bookableObject = await _context.Room.Where(r => r.Name == model.BookableObject).FirstOrDefaultAsync();
            if (bookableObject == null)
            {
                bookableObject = await _context.RoomCombination.Where(r => r.Name == model.BookableObject).FirstOrDefaultAsync();
            }
            ClosureSet closureSet = new ClosureSet()
            {
                BookableObject = bookableObject,
                Start = model.Start,
                End = model.End,
                Comment = model.Comment
            };
            return closureSet;
        }
        private async Task AdjustClosureSets(ClosureSet closureSet)
        {
            #region Find overlaps with ClosureSets
            //The closureSet is deviding already existing by two parts
            List<BookableObjectStateSetter> innerOverlaps = await _context.BookableObjectStateSetter
                .Where(o => o is ClosureSet
                && (o.BookableObject.Name == closureSet.BookableObject.Name)
                && (o.Start.Date < closureSet.Start.Date && o.End.Date > closureSet.End.Date))
                .ToListAsync();

            //The closureSet includes already existing one
            List<BookableObjectStateSetter> fullOuterOverlaps = await _context.BookableObjectStateSetter
                .Where(o => o is ClosureSet
                && (o.BookableObject.Name == closureSet.BookableObject.Name)
                && (o.Start.Date >= closureSet.Start.Date && o.End.Date <= closureSet.End.Date))
                .ToListAsync();

            //The closureSet takes first dates of already existing one
            List<BookableObjectStateSetter> firstOuterOverlaps = await _context.BookableObjectStateSetter
                .Where(o => o is ClosureSet
                && (o.BookableObject.Name == closureSet.BookableObject.Name)
                && (o.Start.Date >= closureSet.Start.Date && o.Start.Date <= closureSet.End.Date)
                && !(o.Start.Date >= closureSet.Start.Date && o.End.Date <= closureSet.End.Date))
                .ToListAsync();

            //The closureSet takes last dates of already existing one
            List<BookableObjectStateSetter> lastOuterOverlaps = await _context.BookableObjectStateSetter
                .Where(o => o is ClosureSet
                && (o.BookableObject.Name == closureSet.BookableObject.Name)
                && (o.End.Date <= closureSet.End.Date && o.End.Date >= closureSet.Start.Date)
                && !(o.Start.Date >= closureSet.Start.Date && o.End.Date <= closureSet.End.Date))
                .ToListAsync();
            #endregion
            #region Adjust overlaps
            //Handle innerOverlaps
            foreach (var item in innerOverlaps)
            {
                ClosureSet secondClosureSet = new ClosureSet
                {
                    Start = closureSet.End.AddDays(1),
                    End = item.End,
                    BookableObject = item.BookableObject,
                    Comment = item.Comment
                };
                item.End = closureSet.Start.AddDays(-1);
                await _context.BookableObjectStateSetter.AddAsync(secondClosureSet);
            }
            //Handle fullOuterOverlaps
            _context.BookableObjectStateSetter.RemoveRange(fullOuterOverlaps);

            //Handle firstOuterOverlaps
            foreach (var item in firstOuterOverlaps)
            {
                item.Start = closureSet.End.Date.AddDays(1);
            }
            //Handle lastOuterOverlaps
            foreach (var item in lastOuterOverlaps)
            {
                item.End = closureSet.Start.Date.AddDays(-1);
            }
            #endregion
        }
    }
    public class ClosureSetModel
    {
        public string BookableObject { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Comment { get; set; }
        public int HotelId { get; set; }
    }
}
