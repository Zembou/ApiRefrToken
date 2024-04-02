using ApiRefr;
using ApiRefr.Class;
using ApiRefr.Models;
using Client.Models;
using Client.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private CallAuthApi _callAuthApi;
        private IHttpContextAccessor _httpContextAccessor;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _callAuthApi = new CallAuthApi();
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
            Response.Cookies.Append("AccessToken", token.AccessToken);
            Response.Cookies.Append("RefreshToken", token.RefreshToken);
            return RedirectToAction("WeatherForecast");
        }

        public async Task<IActionResult> WeatherForecast()
        {
            TokenApiModel token = new TokenApiModel()
            {
                AccessToken = Request.Cookies["AccessToken"],
                RefreshToken = Request.Cookies["RefreshToken"]
            };
            IEnumerable<WeatherForecast> model = await _callAuthApi.Forecast(token);
            Console.WriteLine(model);
            return View(model);
            //return View(model);
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
