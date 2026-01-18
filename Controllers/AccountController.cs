using WEATHERAPP.Services;
using Microsoft.AspNetCore.Mvc;
using WEATHERAPP.Models;

namespace WEATHERAPP.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseService _dbService;

        public AccountController(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        // ===========================
        // GET Login page
        // ===========================
        [HttpGet]
        public IActionResult Login() => View();

        // ===========================
        // POST Login (validate user)
        // ===========================
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // التحقق من وجود اسم المستخدم وكلمة المرور
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please enter username and password";
                return View();
            }

            // التحقق من قاعدة البيانات
            var user = _dbService.ValidateUser(username, password);

            if (user != null)
            {
                // ✅ حفظ بيانات الجلسة
                HttpContext.Session.SetInt32("UserId", user.UserId);
                HttpContext.Session.SetString("Username", user.Username ?? "");

                // ✅ تحويل المستخدم للصفحة الثانية بعد تسجيل الدخول
                return RedirectToAction("Index", "Weather"); // تأكدي ان الـ WeatherController موجود
            }

            // عرض رسالة خطأ
            ViewBag.Error = "Invalid username or password";
            return View();
        }

        // ===========================
        // Logout
        // ===========================
        public IActionResult Logout()
        {
            // مسح الجلسة كاملة
            HttpContext.Session.Clear();

            // العودة لصفحة تسجيل الدخول
            return RedirectToAction("Login");
        }
    }
}
