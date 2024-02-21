using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Server;
using MvcTest.Controllers.Filters;
using MvcTest.Data;
using MvcTest.Sevices;
using MvcTest.Sevices.EmailService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System;
using Microsoft.CodeAnalysis.Options;
using HotelAdmin.Services.IsProductionChecker;
using HotelAdmin.Data.Models.BookableObjects;
using HotelAdmin.Data.Models.Order;
using HotelAdmin.Data.Models.RoomStateSetters;

namespace MvcTest.Controllers.ClientsControllers
{
    public class BookingOptionsController : Controller
    {
        //Initialization of data
        private readonly MvcTestContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BookingOptionsController(MvcTestContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }



        //HTTP endpoints
        [ServiceFilter(typeof(HotelFilter))]
        public IActionResult BookingOptionsParameters([FromQuery] int hotelId)
        {
            return View();
        }
        [ServiceFilter(typeof(HotelFilter))]
        public async Task<IActionResult> Index([FromQuery] int hotelId = 1, bool? errorAlert = false)
        {
            HttpContext.Items["ErrorAlert"] = errorAlert;

            string json = TempData["parameters"] as string;
            BookingOptionsRequestModel parameters = JsonConvert.DeserializeObject<BookingOptionsRequestModel>(json);
            TempData["parameters"] = json;

            OrderOptionsGetter orderOptionsGetter = new OrderOptionsGetter(_context, parameters);
            List<OrderOption> options = await orderOptionsGetter.GetOptions();

            JsonSerializerSettings settings = new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore};
            string _json = JsonConvert.SerializeObject(options, settings);
            _httpContextAccessor.HttpContext.Session.SetString("options", _json);
            return View(options);
        }
        [ServiceFilter(typeof(HotelFilter))]
        public async Task<IActionResult> BookingOptionsItem(int BookingOptionIndex, [FromQuery] int hotelId = 1)
        {
            string json = _httpContextAccessor.HttpContext.Session.GetString("options");
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new BookableObjectConverter());
            settings.Converters.Add(new StateSetterConverter());
            List<OrderOption> options = JsonConvert.DeserializeObject<List<OrderOption>>(json, settings);

            HttpContext.Items["BookingOptionIndex"] = BookingOptionIndex;
            OrderOption option = options[BookingOptionIndex];
            await option.SetPrices(_context);
            await option.SetPrepayments(_context);

            JsonSerializerSettings _settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            string _json = JsonConvert.SerializeObject(option, _settings);
            _httpContextAccessor.HttpContext.Session.SetString("option", _json);
            return View(option);
        }
        [ServiceFilter(typeof(HotelFilter))]
        public IActionResult Confirmation([FromQuery] int hotelId = 1)
        {
            return View();
        }

        //HTTP Post requests
        public async Task<string> SendQuery([FromBody] BookingOptionsRequestModel parameters)
        {
            string isValidInputs = IsValidInputs(parameters);
            if (isValidInputs!=null)
            {
                return isValidInputs;
            }
            string json = JsonConvert.SerializeObject(parameters);
            TempData["parameters"] = json;
            string redirectString = "/BookingOptions/Index?hotelId="+parameters.HotelId;
            return redirectString;

        }
        public async Task<string> MakeOrder([FromBody] ReservationData reservationData)
        {
            string json = _httpContextAccessor.HttpContext.Session.GetString("option");
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new BookableObjectConverter());
            settings.Converters.Add(new StateSetterConverter());
            OrderOption option = JsonConvert.DeserializeObject<OrderOption>(json, settings);

            Order order = await BuildOrder(reservationData, option);
            order.SourceId = 1;

            Orders orders = new Orders(_context);
            OrderModel orderModel = await orders.AssambleOrderModel(order);
            string isOverlapping = await orders.IsOverlaping(orderModel);
            if (isOverlapping!=null )
            {
                return "/BookingOptions/Index?errorAlert=true";
            }
            if (await DidPriceOrPrepaymentChanged(option))
            {
                return "Error: price changed";
            }

            _context.Add(order);
            await _context.SaveChangesAsync();
            order = await _context.Order.OrderBy(order => order.Id)
                .Include(o=>o.Client)
                .Include(o=>o.Reservations)
                .ThenInclude(r=>r.BookableObject.Category).LastAsync();


            EmailBuilder builder = new EmailBuilder(order);
            string administratorEmail = builder.BuildAdministratorEmail();
            EmailSender emailSender = new EmailSender(_context);
            if (AppState.IsProduction)
            {
                emailSender.SendEmailToMyself("Новое бронирование с сайта!", administratorEmail, order.HotelId);
                emailSender.SendEmail(order);
            }


            string redirectString = "/BookingOptions/Confirmation?hotelId="+order.HotelId;
            return redirectString; 
        }

        private async Task<bool> DidPriceOrPrepaymentChanged(OrderOption option)
        {
            decimal oldPrice = option.Price;
            decimal oldPrepayment = option.Prepayment;
            await option.SetPrices(_context);
            await option.SetPrepayments(_context);
            if (oldPrice != option.Price || oldPrepayment != option.Prepayment)
            {
                return true;
            }
            return false;
        }

        private string IsValidInputs(BookingOptionsRequestModel parameters)
        {
            if (parameters == null)
            {
                return "parameters are null!";
            }
            DateTime minDate = (new DateTime()).AddYears(9998);
            DateTime maxDate = new DateTime();
            int i = 0;
            foreach (var item in parameters.Sections)
            {
                if (item.Start.Year == 1 || item.End.Year == 1)
                {
                    return "Пожалуйста, введите все даты для номеров.";
                }
                DateTimeOffset serverTime = DateTimeOffset.Now;
                TimeZoneInfo moscowTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
                DateTimeOffset moscowTime = TimeZoneInfo.ConvertTime(serverTime, moscowTimeZone);
                DateTime moscowDateTime = moscowTime.DateTime;
                if (item.Start.Date < moscowDateTime.Date)
                {
                    return "Вы ввели дату меньше сегодняшней!";
                }
                if (item.Start>item.End)
                {
                    return "Пожалуйста, введите правильные даты. Сечас конечная дата одного из номеров меньше начальной.";
                }
                if (item.Adults == 0 && item.ChildrenBeds==0)
                {
                    return "Пожалуйста, укажите правильное количесво людей. В одном из номеров вы не указали этот пункт.";
                }
                if ((item.Start>maxDate||item.End<minDate)&&i!=0)
                {
                    return "На данный момент поиск вариантов размещения из номеров с непересекающимися датами прожевания не поддерживается. Вы можете указать разные даты для каждого номера, но они должны пересекаться друг с другом.";
                }

                if (item.Start<minDate)
                {
                    minDate = item.Start;
                }
                if (item.End>maxDate)
                {
                    maxDate = item.End;
                }
                i++;
            }
            return null;
        }
        private async Task<Order> BuildOrder(ReservationData data, OrderOption option)
        {
            List<Reservation> reservations = new List<Reservation>();
            for (int i = 0; i < option.OrderOptionCategories.Count; i++)
            {
                BookableObject room = await _context.Room.Where(r => r.Name == data.SelectedOptions[i]).FirstOrDefaultAsync();
                if (room == null)
                {
                    room = await _context.RoomCombination.Where(r => r.Name == data.SelectedOptions[i]).FirstOrDefaultAsync();
                }
                if (room==null)
                {
                    if (data.SelectedOptions[i] != "Случайный номер")
                    {
                        throw new InvalidOperationException();
                    }
                    room = await _context.Room
                        .Where(r => r.Id == option.OrderOptionCategories[i].OrderOptionCategoryRoomsWithDates[0].OrderOptionCategoryRoom.BookableObject.Id)
                        .FirstOrDefaultAsync();
                    if (room == null)
                    {
                        room = await _context.RoomCombination
                         .Where(r => r.Id == option.OrderOptionCategories[i].OrderOptionCategoryRoomsWithDates[0].OrderOptionCategoryRoom.BookableObject.Id)
                         .FirstOrDefaultAsync();
                    }
                }

                reservations.Add(new Reservation
                {
                    NumberofPeople = option.OrderOptionCategories[i].TotalNumberOfPeople,
                    Start = option.OrderOptionCategories[i].Start,
                    End = option.OrderOptionCategories[i].End,
                    BookableObject= room
                });
            }
            Order order = new Order
            {
                Client = new Client
                {
                    Name = data.FullName,
                    PhoneNumber = data.Phone,
                    EMail = data.Email
                },
                Reservations= reservations,
                Price = option.Price,
                PayedPrice = 0,
                Prepayment = option.Prepayment,
                PrepaymentDeadline = option.PrepaymentDeadline,
                HotelId = option.HotelId 
            };
            order.SetCreationDate();

            return order;
        }
    } 



    //Data models for elements on a page
    public class OrderOption
    {
        public List<OrderOptionCategory> OrderOptionCategories { get; set; }
        public decimal Price
        {
            get
            {
                decimal price = 0;
                foreach (var item in OrderOptionCategories)
                {
                    price += item.Price;
                }
                return price;
            }
        }
        public decimal Prepayment
        {
            get
            {
                decimal prepayment = 0;
                foreach (var item in OrderOptionCategories)
                {
                    prepayment += item.Prepayment;
                }
                return prepayment;
            }
        }
        public string Name
        {
            get
            {
                string name = "";
                int i = 0;
                foreach (var item in OrderOptionCategories)
                {
                    name += item.Name;
                    if (i!=OrderOptionCategories.Count-1)
                    {
                        name+= ", ";
                    }
                }
                return name;
            }
        }
        public DateTime PrepaymentDeadline { get; set; }
        public int HotelId 
        {
            get
            {
                if (OrderOptionCategories.IsNullOrEmpty())
                {
                    throw new InvalidOperationException();
                }
                return OrderOptionCategories[0].HotelId;
            }
        }

        public async Task SetPrices(MvcTestContext _context)
        {
            if (OrderOptionCategories.IsNullOrEmpty())
            {
                throw new InvalidOperationException();
            }
            foreach (var item in OrderOptionCategories)
            {
                await item.SetPrice(_context);
            }
        }
        public async Task SetPrepayments(MvcTestContext _context)
        {
            if (OrderOptionCategories.IsNullOrEmpty())
            {
                throw new InvalidOperationException();
            }
            foreach (var item in OrderOptionCategories)
            {
                await item.SetPrepayment(_context);
            }
        }

    }
    public class OrderOptionCategory
    {
        public decimal Price { get; set; }
        public async Task SetPrice(MvcTestContext _context)
        {
            if (OrderOptionCategoryRoomsWithDates.IsNullOrEmpty())
            {
                throw new InvalidOperationException();
            }
            List<BookableObjectStateSetter> priceSets = await _context.BookableObjectStateSetter.Where(setter => setter is PriceSet &&
            OrderOptionCategoryRoomsWithDates[0].Start <= setter.End &&
            OrderOptionCategoryRoomsWithDates[0].End >= setter.Start &&
            setter.BookableObject == OrderOptionCategoryRoomsWithDates[0].OrderOptionCategoryRoom.BookableObject).ToListAsync();

            DateTime startDate = OrderOptionCategoryRoomsWithDates[0].Start;
            DateTime endDate = OrderOptionCategoryRoomsWithDates[0].End;

            DateTime currentDate = startDate;
            decimal _price = 0;

            while (currentDate <= endDate)
            {
                var priceSet = priceSets.FirstOrDefault(setter => setter.Start <= currentDate && setter.End >= currentDate);

                if (priceSet != null)
                {
                    _price += (priceSet as PriceSet).Price;
                }
                else
                {
                    _price += rootCategory.BasePrice;
                }
                for (int i = 0; i < ExtraNumberOfBeds; i++)
                {
                    _price += OrderOptionCategoryRoomsWithDates[0].OrderOptionCategoryRoom.BookableObject.AdditionalPrice;
                }
                for (int i = 0; i < ExtraNumberOfPeople; i++)
                {
                    _price += rootCategory.AdditionalPrice;
                }
                currentDate = currentDate.AddDays(1);
            }
            Price = _price;
        }

        public decimal Prepayment { get; set; }
        public async Task SetPrepayment(MvcTestContext _context)
        {
            if (OrderOptionCategoryRoomsWithDates.IsNullOrEmpty())
            {
                throw new InvalidOperationException();
            }
            BookableObjectStateSetter priceSet = await _context.BookableObjectStateSetter.Where(setter=>setter is PriceSet &&
            setter.BookableObject == OrderOptionCategoryRoomsWithDates[0].OrderOptionCategoryRoom.BookableObject &&
            setter.Start<= OrderOptionCategoryRoomsWithDates[0].Start &&
            setter.End >= OrderOptionCategoryRoomsWithDates[0].Start).FirstOrDefaultAsync();
            if (priceSet != null)
            {
                Prepayment = (priceSet as PriceSet).Price;
            }
            else
            {
                Prepayment = rootCategory.BasePrice;
            }
            for (int i = 0; i < ExtraNumberOfBeds; i++)
            {
                Prepayment += OrderOptionCategoryRoomsWithDates[0].OrderOptionCategoryRoom.BookableObject.AdditionalPrice;
            }
            for (int i = 0; i < ExtraNumberOfPeople; i++)
            {
                Prepayment += rootCategory.AdditionalPrice;
            }
        }

        public List<OrderOptionCategoryRoomWithDates> OrderOptionCategoryRoomsWithDates { get; set; } = new List<OrderOptionCategoryRoomWithDates>();
        public int RootCategoryId
        {
            get
            {
                if (OrderOptionCategoryRoomsWithDates.IsNullOrEmpty())
                {
                    throw new InvalidOperationException();
                }
                return OrderOptionCategoryRoomsWithDates[0].OrderOptionCategoryRoom.BookableObject.CategoryId;
            }
        }
        public Category rootCategory
        {
            get
            {
                return OrderOptionCategoryRoomsWithDates[0].OrderOptionCategoryRoom.BookableObject.Category;
            }
        }
        public int ExtraNumberOfBeds
        {
            get
            {
                if (OrderOptionCategoryRoomsWithDates.IsNullOrEmpty())
                {
                    throw new InvalidOperationException();
                }
                return OrderOptionCategoryRoomsWithDates[0].OrderOptionCategoryRoom.ExtraNumberOfBeds;
            }
        }
        public int ExtraNumberOfPeople
        {
            get
            {
                if (OrderOptionCategoryRoomsWithDates.IsNullOrEmpty())
                {
                    throw new InvalidOperationException();
                }
                return OrderOptionCategoryRoomsWithDates[0].OrderOptionCategoryRoom.ExtraNumberOfPeople;
            }
        }
        public int TotalNumberOfPeople
        {
            get
            {
                if (OrderOptionCategoryRoomsWithDates.IsNullOrEmpty())
                {
                    throw new InvalidOperationException();
                }
                return OrderOptionCategoryRoomsWithDates[0].OrderOptionCategoryRoom.TotalNumberOfPeople;
            }
        }                
        public string Name
        {
            get
            {
                if (OrderOptionCategoryRoomsWithDates.IsNullOrEmpty())
                {
                    throw new InvalidOperationException();
                    return "No rooms in category";
                }
                string name = OrderOptionCategoryRoomsWithDates[0].OrderOptionCategoryRoom.BookableObject.Category.Name;
                if (OrderOptionCategoryRoomsWithDates[0].OrderOptionCategoryRoom.ExtraNumberOfBeds!=0)
                {
                    name += " + ";
                    if (OrderOptionCategoryRoomsWithDates[0].OrderOptionCategoryRoom.ExtraNumberOfBeds==1)
                    {
                        name+= "1 дополнительное спальное место";
                    }
                    else
                    {
                        name += OrderOptionCategoryRoomsWithDates[0].OrderOptionCategoryRoom.ExtraNumberOfBeds + " дополнительных спальных места";
                    }
                }
                return name;
            }
        }
        public DateTime Start
        {
            get
            {
                if (OrderOptionCategoryRoomsWithDates.IsNullOrEmpty())
                {
                    throw new InvalidOperationException();
                }
                return OrderOptionCategoryRoomsWithDates[0].Start;
            }
        }
        public DateTime End
        {
            get
            {
                if (OrderOptionCategoryRoomsWithDates.IsNullOrEmpty())
                {
                    throw new InvalidOperationException();
                }
                return OrderOptionCategoryRoomsWithDates[0].End;
            }
        }
        public DateTime CheckIn
        {
            get
            {
                if (OrderOptionCategoryRoomsWithDates.IsNullOrEmpty())
                {
                    throw new InvalidOperationException();
                }
                return OrderOptionCategoryRoomsWithDates[0].Start;
            }
        }
        public DateTime CheckOut
        {
            get
            {
                if (OrderOptionCategoryRoomsWithDates.IsNullOrEmpty())
                {
                    throw new InvalidOperationException();
                }
                return OrderOptionCategoryRoomsWithDates[0].End.AddDays(1);
            }
        }
        public string NightsNumber
        {
            get
            {
                int nightsNumber = ((int)(CheckOut.Date - CheckIn.Date).TotalDays);
                if (nightsNumber == 1)
                {
                    return "1 ночь";
                }
                if (nightsNumber == 2)
                {
                    return "2-е ночи";
                }
                return nightsNumber.ToString() + " ночи";
            }
        }        
        public int HotelId
        {
            get
            {
                if (OrderOptionCategoryRoomsWithDates.IsNullOrEmpty())
                {
                    throw new InvalidOperationException();
                }
                return OrderOptionCategoryRoomsWithDates[0].OrderOptionCategoryRoom.BookableObject.HotelId;
            }
        }
    }
    public class OrderOptionCategoryRoomWithDates
    {
        public OrderOptionCategoryRoom OrderOptionCategoryRoom { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        //Used for equality comparison
        public string Identifier
        {
            get
            {
                string identifier = "";
                identifier += OrderOptionCategoryRoom.BookableObject.Id;
                identifier += "_" + OrderOptionCategoryRoom.ExtraNumberOfPeople;
                identifier += "_" + OrderOptionCategoryRoom.ExtraNumberOfPeople;
                identifier += "_" + Start;
                identifier += "_" + End;
                return identifier;
            }
        }
    }
    public class OrderOptionCategoryRoom
    {
        public BookableObject BookableObject { get; set; }
        public int BaseNumberOfPeople { get; set; } = 0;
        public int ExtraNumberOfPeople { get; set; } = 0;
        public int ExtraNumberOfBeds { get;set; } = 0;
        public int TotalNumberOfPeople
        {
            get
            {
                return BaseNumberOfPeople + ExtraNumberOfPeople + ExtraNumberOfBeds;
            }
        }
    }



    //Data models for HTTP Post requests
    public class BookingOptionsRequestModel
    {
        public int HotelId { get; set; }
        public List<AccommodationSectionModel> Sections { get; set; }
    }
    public class AccommodationSectionModel
    {
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public DateTime Start 
        {
            get
            {
                return CheckIn;
            }
        }
        public DateTime End
        {
            get
            {
                return CheckOut.AddDays(-1);
            }
        }
        public int Adults { get; set; }
        public bool HasChildren { get; set; }
        public int ChildrenBeds { get; set; }
        public bool HasKitchen { get; set; }
        public bool HasDog { get; set; }
    }
    public class ReservationData
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string[] SelectedOptions { get; set; }
    }



    //Json converter
    public class StateSetterConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BookableObjectStateSetter);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            JObject jsonObject = JObject.Load(reader);

            if (jsonObject["Price"] != null)
            {
                // It's a PriceSet
                return jsonObject.ToObject<PriceSet>(serializer);
            }
            else if (jsonObject["NumberofPeople"] != null)
            {
                // It's a Reservation
                return jsonObject.ToObject<Reservation>(serializer);
            }
            else if (jsonObject["Start"] != null && jsonObject["End"] != null)
            {
                // It's a ClosureSet
                return jsonObject.ToObject<ClosureSet>(serializer);
            }
            throw new JsonSerializationException("Unable to deserialize BookableObjectStateSetter.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
    public class BookableObjectConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BookableObject);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            JObject jsonObject = JObject.Load(reader);

            // If the JSON contains a property named "Rooms," it indicates a RoomCombination object.
            // Otherwise, it represents a Room object.
            if (jsonObject["Rooms"] != null)
            {
                if (reader.TokenType == JsonToken.Null)
                    return null;
                // It's a RoomCombination
                return jsonObject.ToObject<RoomCombination>(serializer);
            }
            else
            {
                // It's a Room
                return jsonObject.ToObject<Room>(serializer);
            }

        }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
