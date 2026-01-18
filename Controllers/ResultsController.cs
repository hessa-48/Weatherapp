using Microsoft.AspNetCore.Mvc;

namespace WEATHERAPP.Controllers
{
    public class ResultsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
