using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcTest.Data;

namespace MvcTest.Controllers.AdministratorsControllers
{
    public class AdministratorAuthentificationController : Controller
    {
        private readonly MvcTestContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AdministratorAuthentificationController(MvcTestContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<string> Index([FromQuery] string accessCode)
        {
            bool found = await _context.keyCodes.Where(k => k.Code == accessCode).AnyAsync();
            if (found)
            {
                _httpContextAccessor.HttpContext.Session.SetString("keyCode", accessCode);
            }
            else
            {
                return "Неправльный код!";
            }
            string returnUrl = _httpContextAccessor.HttpContext.Session.GetString("returnUrl");
            if (returnUrl==null)
            {
                return "/";
            }
            return returnUrl;
        }
    }
}
