using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobRecruitment2024.Controllers
{
    public class ManagerController : Controller
    {
        // GET: Manager
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            // Your authentication logic here...
            // Assuming the authentication is successful, you can redirect to the main page.
            string usernameAuth = "eren";
            string passwordAuth = "test";

            if (usernameAuth == username && passwordAuth == password)
            {
                // Redirect to the main page (assuming "Index" is your main page)
                return RedirectToAction("ManagerMainPage");
            }

            string errorMessage = "Invalid email or password.";
            ViewData["ErrorMessage"] = errorMessage;
            return View("Login");
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