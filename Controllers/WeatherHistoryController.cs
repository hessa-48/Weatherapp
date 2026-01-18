using Microsoft.AspNetCore.Mvc;
using WEATHERAPP.Models;
using WEATHERAPP.Services;

namespace WEATHERAPP.Controllers
{
    public class WeatherHistoryController : Controller
    {
        private readonly DatabaseService _dbService;

        public WeatherHistoryController(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        // =========================
        // GET: عرض سجل الطقس للمستخدم
        // =========================
        [HttpGet]
        public IActionResult Index()
        {
            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userId == 0)
                return RedirectToAction("Login", "Account");

            var results = _dbService.GetUserWeatherResults(userId);

            // لأن اسم الفولدر عندك NewFolder
            return View("~/Views/NewFolder/Index.cshtml", results);
        }

        // =========================
        // POST: حفظ التعديلات (AJAX)
        // =========================
        [HttpPost]
        public IActionResult SaveChanges([FromBody] List<WeatherResult> updatedResults)
        {
            if (updatedResults == null || updatedResults.Count == 0)
                return BadRequest("No data to save");

            int userId = HttpContext.Session.GetInt32("UserId") ?? 0;

            if (userId == 0)
                return Unauthorized("User not logged in");

            foreach (var result in updatedResults)
            {
                result.UserId = userId;
                result.DateRecorded = DateTime.Now;

                _dbService.UpdateWeatherResult(result);
            }

            return Ok("All changes saved successfully");
        }
    }
}
