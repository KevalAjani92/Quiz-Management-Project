using Microsoft.AspNetCore.Mvc;

namespace QuizManagement.Controllers
{
    [CheckAccess]
    public class ProfileController : Controller
    {        
        public IActionResult Index()
        {
            return View();
        }
    }
}
