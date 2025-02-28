using Microsoft.AspNetCore.Mvc;

namespace QuizManagement.Controllers
{
    //[CheckAccess]
    public class QuizwiseQuestionController : Controller
    {
        public IActionResult AddEdit_QuizwiseQuestion()
        {
            return View();
        }
        public IActionResult QuizwiseQuestionList()
        {
            return View();
        }
        public IActionResult QuizwiseQuestionDetails()
        {
            return View();
        }
    }
}
