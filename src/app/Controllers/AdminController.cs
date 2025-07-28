using app.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace app.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult User()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Report()
        {
            return View();
        }

        public IActionResult Story()
        {
            return View();
        }

        public IActionResult Transaction()
        {
            return View();
        }

        public IActionResult Category()
        {
            return View();
        }

        public IActionResult Ticket()
        {
            return View();
        }

        public IActionResult Review()
        {
            return View();
        }
    }
}
