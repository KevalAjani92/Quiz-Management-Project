using Microsoft.AspNetCore.Mvc;

namespace QuizManagement.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
