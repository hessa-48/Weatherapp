using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace WeatherApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly DbService _dbService;

        public HomeController(DbService dbService)
        {
            _dbService = dbService;
        }

        // الصفحة الرئيسية (Login Page)
        public IActionResult Index()
        {
            return View();
        }

        // POST method لتسجيل الدخول
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // التحقق من أن الحقول غير فارغة
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please fill both fields";
                return View("Index");
            }

            // التحقق من قاعدة البيانات
            var user = await _dbService.ValidateUserAsync(username, password);
            if (user != null)
            {
                // حفظ بيانات المستخدم في الجلسة
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("Username", user.Username);

                // إعادة التوجيه لصفحة الطقس
                return RedirectToAction("WeatherPage");
            }

            ViewBag.Error = "Invalid username or password";
            return View("Index");
        }

        // صفحة الطقس (سنكمل الكود لاحقًا)
        public IActionResult WeatherPage()
        {
            return View();
        }
    }
}
