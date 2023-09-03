using Microsoft.AspNetCore.Mvc;

namespace WEBAPP.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
