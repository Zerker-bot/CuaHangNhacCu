using Microsoft.AspNetCore.Mvc;

namespace CuaHangNhacCu.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
