using Microsoft.AspNetCore.Mvc;

namespace QuizManagement.Controllers
{
    public class QuizController : Controller
    {
        public IActionResult AddEdit_Quiz()
        {
            return View();
        }
        public IActionResult QuizList()
        {
            return View();
        }
    }
}
