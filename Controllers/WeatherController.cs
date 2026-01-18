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

        public WeatherController(DatabaseService dbService, IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _dbService = dbService;
            _httpClientFactory = httpClientFactory;
            _config = config;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                ViewBag.Error = "Please enter a city";
                return View();
            }

            try
            {
                var apiKey = _config["OpenWeatherMapAPIKey"];
                var client = _httpClientFactory.CreateClient();
                var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = "City not found or API error";
                    return View();
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(json);

                int humidity = data.GetProperty("main").GetProperty("humidity").GetInt32();
                double tempMin = data.GetProperty("main").GetProperty("temp_min").GetDouble();
                double tempMax = data.GetProperty("main").GetProperty("temp_max").GetDouble();

                var userId = HttpContext.Session.GetInt32("UserId") ?? 0;

                ViewBag.WeatherResult = new WeatherResult
                {
                    UserId = userId,
                    City = city,
                    Humidity = humidity,
                    TempMin = tempMin,
                    TempMax = tempMax,
                    DateAdded = DateTime.Now,
                    DateRecorded = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Something went wrong: " + ex.Message;
            }

            return View();
        }

        [HttpPost]
        public IActionResult Save(string city, int? humidity, double? tempMin, double? tempMax, DateTime? dateRecorded)
        {
            var userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userId == 0 || string.IsNullOrEmpty(city) || dateRecorded == null)
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

            return RedirectToAction("Index");
        }
    }
}
