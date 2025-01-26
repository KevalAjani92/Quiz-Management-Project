using Microsoft.AspNetCore.Mvc;

namespace QuizManagement.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
