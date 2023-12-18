﻿using JobRecruitment2024.Models;
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
            Session["UserEmail"] = email;
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
                        phone_num = model.phone_num,
                        email = model.email,
                    };
                    if (model.password != null)
                        user.password = model.password;

                    // Check if the email is already registered

                    if (model.tc != null && _context.Users.Any(u => u.tc == model.tc))
                    {
                        throw new InvalidOperationException("This TC is registered before.");
                    }


                    if (model.email != null && _context.Users.Any(u => u.email == model.email))
                    {
                        throw new InvalidOperationException("This email is already registered.");
                    }
                    _context.Users.Add(user);
                    _context.SaveChanges();

                    ViewBag.SuccessMessage = "Registration successful. You are being redirected to the login page.";

                    return View("Register", model);
                }
                catch (DbUpdateException ex)
                {
                        ViewBag.ErrorMessage = "An error occurred while processing your request.";

                    return View(model); // Return to the view with the error message
                }
                catch (InvalidOperationException ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return View(model); // Return to the view with the error message
                }
            }

            ViewBag.ErrorMessage = "Please enter your information correctly.";
            return View(model);
        }

        // Helper method to check if it's a unique constraint violation
        private bool IsUniqueConstraintViolation(SqlException ex)
        {
            // Unique constraint violation error numbers can vary based on the database
            // You might need to replace 2601 and 2627 with the specific error numbers related to unique constraint violations in your database
            return ex.Number == 2601 || ex.Number == 2627;
        }

    public ActionResult UserMainPage()
        {
            return View();
        }

        [HttpGet]
        public ActionResult UpdateAccount()
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
        public ActionResult UpdateAccount(Users updatedUser)
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

                    _context.SaveChanges();

                    ViewBag.SuccessMessage = "Account information updated successfully.Redirecting mainpage...";
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

                    // Log or debug these error messages to understand the validation failures
                }

                // Return to the view with the model to display the errors
                ViewBag.ErrorMessage = "Invalid data. Please check your inputs.";
            }

            return View(updatedUser);
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