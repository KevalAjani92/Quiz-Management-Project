using Microsoft.AspNetCore.Mvc;

namespace QuizManagement.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
