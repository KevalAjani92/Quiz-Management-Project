using Microsoft.AspNetCore.Mvc;

namespace QuizManagement.Controllers
{
    public class QuestionLevelController : Controller
    {
        public IActionResult AddEdit_QuestionLevel()
        {
            return View();
        }
    }
}
