using ApiRefr;
using ApiRefr.Class;
using ApiRefr.Models;
using Client.Models;
using Client.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private CallAuthApi _callAuthApi;
        private IHttpContextAccessor _httpContextAccessor;
        private TokenApiModel tokens;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _callAuthApi = new CallAuthApi(httpContextAccessor);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Index(LoginModel model)
        {
            TokenApiModel token = await _callAuthApi.Auth(model);

            if (token == null || token.AccessToken == "")
            {
                return View();
            } else
            {
                CookieOptions options = new CookieOptions();
                options.IsEssential = true;
                options.HttpOnly = true;
                options.Secure = true;
                Response.Cookies.Append("AccessToken", token.AccessToken, options);
                Response.Cookies.Append("RefreshToken", token.RefreshToken, options);
                return RedirectToAction("WeatherForecast");
            }

            
        }

        public async Task<IActionResult> WeatherForecast()
        {
            TokenApiModel token = await _callAuthApi.ValidateToken(HttpContext);
            CookieOptions options = new CookieOptions();
            options.IsEssential = true;
            options.HttpOnly = true;
            options.Secure = true;
            //options.SameSite = SameSiteMode.None;
            
            Response.Cookies.Append("AccessToken", token.AccessToken, options);
            Response.Cookies.Append("RefreshToken", token.RefreshToken, options);
            var model = await _callAuthApi.Forecast(token);

            if (model != null)
            {
                return View(model);
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
