using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using MvcTest.Data;
using Microsoft.EntityFrameworkCore;
using MvcTest.Models.HotelManagerModels;
using Microsoft.AspNetCore.Http.Extensions;
using HotelAdmin.Services.IsProductionChecker;

namespace MvcTest.Controllers.Filters
{
    public class AuthenticationFilter : IAuthorizationFilter
    {
        private readonly MvcTestContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationFilter(MvcTestContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            if (AppState.IsProduction)
            {
                string keyCode = _httpContextAccessor.HttpContext.Session.GetString("keyCode");
                string returnUrl = _httpContextAccessor.HttpContext.Request.GetDisplayUrl();
                if (keyCode == null)
                {
                    _httpContextAccessor.HttpContext.Session.SetString("returnUrl", returnUrl);
                    context.Result = new RedirectToActionResult("Index", "AdministratorAuthentification", new { returnUrl });
                }
                else
                {
                    IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                    string connectionString = configuration.GetConnectionString("Main");
                    var serverVersion = new MySqlServerVersion(new Version(8, 0, 32));
                    var newContext = new MvcTestContext(new DbContextOptionsBuilder<MvcTestContext>()
                        .UseMySql(connectionString, serverVersion)
                        .Options);
                    bool found = await newContext.keyCodes.Where(k => k.Code == keyCode).AnyAsync();
                    if (!found)
                    {
                        _httpContextAccessor.HttpContext.Session.SetString("returnUrl", returnUrl);
                        context.Result = new RedirectToActionResult("Index", "AdministratorAuthentification", new { returnUrl });
                    }
                }
            }
        }
    }
}
