using JobRecruitment2024.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity; //entity framework
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNet.Identity;
using System.Web.Helpers;
using System.Data.Entity.Infrastructure;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
using System.Runtime.ConstrainedExecution;

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
        public ActionResult UserLoginPage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UserLoginPage(string email, string password)
        {
            string encryptedPassword = HomeController.PasswordEncrypt(password);
            Session["UserEmail"] = email;
            var user = _context.Users.FirstOrDefault(u => u.email == email && u.password == encryptedPassword);

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
                    if (model.tc != null && _context.Users.Any(u => u.tc == model.tc))
                    {
                        throw new InvalidOperationException("This TC is registered.");
                    }


                    if (model.email != null && _context.Users.Any(u => u.email == model.email))
                    {
                        throw new InvalidOperationException("This email is already registered.");
                    }

                    string encryptedPassword = HomeController.PasswordEncrypt(model.password);
                    var user = new Users
                    {
                        tc = model.tc,
                        name = model.name,
                        surname = model.surname,
                        phone_num = model.phone_num,
                        email = model.email,
                        password = encryptedPassword
                    };

                    _context.Users.Add(user);
                    _context.SaveChanges();

                    ViewBag.SuccessMessage = "Registration successful. You are being redirected to the login page.";

                    return View("Register", model);
                }
                catch (DbUpdateException ex)
                {
                    ViewBag.ErrorMessage = "An error occurred while processing your request." + ex;

                    return View(model);
                }
                catch (InvalidOperationException ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return View(model);
                }
            }

            ViewBag.ErrorMessage = "Please enter your information correctly.";
            return View(model);
        }

        [HttpGet]
        public ActionResult UserMainPage()
        {

            string userEmail = Session["UserEmail"] as string;
            Users user = _context.Users.FirstOrDefault(u => u.email == userEmail);
            if (user == null)
            {
                ViewBag.ErrorMessage = "User not found. Redirecting Login Page...";
            }
            else
            {
                if(user.emp_status != null)
                {
                    ViewBag.StatusMessage = "You have been employed.";
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Remove("UserEmail");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult UserUpdateAccount()
        {
            string userEmail = Session["UserEmail"] as string;
            try
            {
                Users user = _context.Users.FirstOrDefault(u => u.email == userEmail);

                if (user != null)
                {
                    return View(user);
                }
                else
                {
                    ViewBag.ErrorMessage = "User not found. Redirecting Main Page";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while processing your request.";

                if (ex.InnerException != null)
                {
                    ViewBag.InnerErrorMessage = ex.InnerException.Message;
                }

                return View();
            }
        }

        [HttpPost]
        public ActionResult UserUpdateAccount(Users updatedUser)
        {
            string userEmail = Session["UserEmail"] as string;
            if (ModelState.IsValid)
            {
                try
                {
                    Users currentUser = _context.Users.FirstOrDefault(u => u.email == userEmail);
                    currentUser.name = updatedUser.name;
                    currentUser.surname = updatedUser.surname;
                    currentUser.email = updatedUser.email;
                    currentUser.phone_num = updatedUser.phone_num;

                    if (updatedUser.password != null)
                    {
                        string encryptedPassword = HomeController.PasswordEncrypt(updatedUser.password);
                        currentUser.password = encryptedPassword;

                    }


                    _context.SaveChanges();

                    ViewBag.SuccessMessage = "Account information updated successfully.Redirecting mainpage...";
                    Session["UserEmail"] = currentUser.email;
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex + "Failed to update account information. Please try again.";
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    var errorMessage = error.ErrorMessage;

                }

                // Return to the view with the model to display the errors
                ViewBag.ErrorMessage = "Invalid data. Please check your inputs.";
            }

            return View(updatedUser);
        }

        public ActionResult UserDeleteAccount()
        {
            string userEmail = Session["UserEmail"] as string;
            Users user = _context.Users.FirstOrDefault(u => u.email == userEmail);
            // Delete applications associated with the user's tc

            var userApplications = _context.Applications.Where(a => a.tc == user.tc);
                _context.Applications.RemoveRange(userApplications);

                // Delete the user with the specified tc
                var userToDelete = _context.Users.SingleOrDefault(u => u.tc == user.tc);
                if (userToDelete != null)
                {
                _context.Users.Remove(userToDelete);
                }

                _context.SaveChanges();
            TempData["SuccessMessage"] = "You've deleted your account.";
            return RedirectToAction("UserLoginPage", "User");

        }
    }
}


