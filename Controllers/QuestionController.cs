using Microsoft.AspNetCore.Mvc;

namespace QuizManagement.Controllers
{
    public class QuestionController : Controller
    {
        public IActionResult AddEdit_Question()
        {
            return View();
        }
    }
}
