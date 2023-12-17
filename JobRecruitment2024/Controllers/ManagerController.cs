using JobRecruitment2024.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobRecruitment2024.Controllers
{
    public class ManagerController : Controller
    {
        private readonly DbContextModel _context;

        public ManagerController()
        {
            _context = new DbContextModel();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var manager = _context.Managers.FirstOrDefault(u => u.username == username && u.password == password);


            if (manager != null)
            {
                // Redirect to the main page if authentication is successful
                return RedirectToAction("ManagerMainPage");
            }

            string errorMessage = "Invalid email or password.";
            ViewData["ErrorMessage"] = errorMessage;
            return View();
        }


        public ActionResult Register() {
            return View();
        }

        public ActionResult ManagerMainPage()
        {
            return View();
        }

        public ActionResult UpdateAccount()
        {
            return View();
        }

        public ActionResult ManageJobPosting()
        {
            return View();
        }

        public ActionResult ManageApplications()
        {
            return View();
        }

        public ActionResult ManageEmployees()
        {
            return View();
        }
    }
}