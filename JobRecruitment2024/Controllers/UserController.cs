using JobRecruitment2024.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobRecruitment2024.Controllers
{
    public class UserController : Controller
    {
        private readonly DbContextViewModel _context;

        public UserController()
        {
            _context = new DbContextViewModel();
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

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Users model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new Users
                    {
                        tc = model.tc,
                        name = model.name,
                        surname = model.surname,
                        email = model.email,
                        password = model.password,
                        phone_num = model.phone_num
                    };

                    _context.Users.Add(user);
                    _context.SaveChanges();

                    ViewBag.SuccessMessage = "Registration successful. You are being redirected to the login page.";

                    return View("Register", model);
                }
                catch (DbUpdateException ex)
                {
                    if (ex.InnerException.InnerException is SqlException sqlException && sqlException.Number == 2627)
                    {
                        ViewBag.ErrorMessage = "You are already registered."; // Duplicate key error message
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Please write the data in appropriate format."; // Other database-related error message
                    }

                    return View(model); // Return to the view with the error message
                }
            }
            ViewBag.ErrorMessage = "Please enter your information correctly.";
            return View(model);
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