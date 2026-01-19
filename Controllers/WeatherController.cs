using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WEATHERAPP.Models;
using WEATHERAPP.Services;

namespace WEATHERAPP.Controllers
{
    public class WeatherController : Controller
    {
        private readonly DatabaseService _dbService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;

        public WeatherController(
            DatabaseService dbService,
            IHttpClientFactory httpClientFactory,
            IConfiguration config)
        {
            _dbService = dbService;
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        // ===========================
        // GET: Weather page
        // ===========================
        [HttpGet]
        public IActionResult Index()
        {
            // 🔒 User must be logged in
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // ===========================
        // POST: Get weather from API
        // ===========================
        [HttpPost]
        public async Task<IActionResult> Index(string city)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(city))
            {
                ViewBag.Error = "Please enter a city";
                return View();
            }

            try
            {
                var apiKey = _config["OpenWeatherMapAPIKey"];
                var client = _httpClientFactory.CreateClient();

                var url =
                    $"https://api.openweathermap.org/data/2.5/weather" +
                    $"?q={city}&appid={apiKey}&units=metric";

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "City not found or API error";
                    return View();
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(json);

                var weatherResult = new WeatherResult
                {
                    UserId = HttpContext.Session.GetInt32("UserId"),
                    City = city,
                    Humidity = data.GetProperty("main").GetProperty("humidity").GetInt32(),
                    TempMin = data.GetProperty("main").GetProperty("temp_min").GetDouble(),
                    TempMax = data.GetProperty("main").GetProperty("temp_max").GetDouble(),
                    DateAdded = DateTime.Now,
                    DateRecorded = DateTime.Now
                };

                ViewBag.WeatherResult = weatherResult;
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Something went wrong: " + ex.Message;
            }

            return View();
        }

        // ===========================
        // POST: Save weather to DB
        // ===========================
        [HttpPost]
        public IActionResult Save(
            string city,
            int? humidity,
            double? tempMin,
            double? tempMax,
            DateTime? dateRecorded)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null || string.IsNullOrEmpty(city) || dateRecorded == null)
            {
                TempData["Error"] = "Invalid data or user not logged in.";
                return RedirectToAction("Index");
            }

            var result = new WeatherResult
            {
                UserId = userId,
                City = city,
                Humidity = humidity,
                TempMin = tempMin,
                TempMax = tempMax,
                DateAdded = DateTime.Now,
                DateRecorded = dateRecorded.Value
            };

            try
            {
                _dbService.InsertWeatherResult(result);
                TempData["Success"] = "Weather saved successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error saving weather: " + ex.Message;
            }

            // ✅ Redirect directly to History page
            return RedirectToAction("Index", "WeatherHistory");
        }
    }
}
