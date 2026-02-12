using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TournamentManager.Controllers
{
    public class RefereeController : Controller
    {
        [Authorize(Roles = "Admin, Referee")]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return RedirectToAction("AccessDenied", "Home");
        }
    }
}
