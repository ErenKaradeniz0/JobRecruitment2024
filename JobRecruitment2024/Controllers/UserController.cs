using JobRecruitment2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobRecruitment2024.Controllers
{
    public class UserController : Controller
    {
        private readonly DbContextModel _context;

        public UserController()
        {
            _context = new DbContextModel();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {

            var user = _context.Users.FirstOrDefault(u => u.email == email && u.password == password);




            if (user != null)
            {
                // Redirect to the main page if authentication is successful
                return RedirectToAction("UserMainPage");
            }

            string errorMessage = "Invalid email or password.";
            ViewData["ErrorMessage"] = errorMessage;
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult UserMainPage()
        {
            return View();
        }

        public ActionResult UpdateAccount()
        {
            return View();
        }

        public ActionResult ApplyJob()
        {
            return View();
        }

        public ActionResult MyApplications()
        {
            return View();
        }

    }
}