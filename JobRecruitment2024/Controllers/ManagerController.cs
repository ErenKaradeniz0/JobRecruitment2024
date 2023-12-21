using JobRecruitment2024.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;

namespace JobRecruitment2024.Controllers
{
    public class ManagerController : Controller
    {
        private readonly DbContextViewModel _context;

        public ManagerController()
        {
            _context = new DbContextViewModel();
        }

        [HttpGet]
        public ActionResult ManagerLoginPage()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ManagerLoginPage(string username, string password)
        {
            Session["ManagerUsername"] = username;
            var manager = _context.Managers.FirstOrDefault(u => u.username == username && u.password == password);


            if (manager != null)
            {
                // Redirect to the main page if authentication is successful
                return RedirectToAction("ManagerMainPage");
            }

            string errorMessage = "Invalid username or password.";
            ViewData["ErrorMessage"] = errorMessage;
            return View();
        }


        public ActionResult ManagerMainPage()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ManagerUpdateAccount()
        {
            string ManagerUsername = Session["ManagerUsername"] as string;
            try
            {
                Managers manager = _context.Managers.FirstOrDefault(m => m.username == ManagerUsername);


                if (manager != null)
                {
                    return View(manager);
                }
                else
                {
                    ViewBag.ErrorMessage = "Manager not found. Redirecting Main Page";
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
        public ActionResult ManagerUpdateAccount(Managers updatedManager)
        {
            string ManagerUsername = Session["ManagerUsername"] as string;
            if (ModelState.IsValid)
            {
                try
                {
                    Managers currentManager = _context.Managers.FirstOrDefault(m => m.username == ManagerUsername);

                    currentManager.username = updatedManager.username;
                    currentManager.name = updatedManager.name;
                    currentManager.surname = updatedManager.surname;
                    currentManager.email = updatedManager.email;
                    currentManager.phone_num = updatedManager.phone_num;

                    if (updatedManager.password != null) currentManager.password = updatedManager.password;
               
           
                    _context.SaveChanges();

                    ViewBag.SuccessMessage = "Account information updated successfully. Redirecting mainpage...";
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

            return View(updatedManager);
        }


        
        private readonly JobController _jobController = new JobController();

        [HttpGet]
        public ActionResult ManageJobPosting()
        {
            string ManagerUsername = Session["ManagerUsername"] as string;
            Managers currentManager = _context.Managers.FirstOrDefault(m => m.username == ManagerUsername);
            if (currentManager == null)
            {
                ViewBag.ErrorMessage = "Manager not found. Redirecting Login Page...";
                return View();
            }
            var jobs = _context.Jobs.ToList();

            if (jobs == null || !jobs.Any())
            {
                ViewBag.ErrorMessage = "No Jobs Found. Redirecting mainpage...";
            }

            var jobViewModel = new JobViewModel
            {
                JobsList = jobs,
                Job = null
            };

            return View(jobViewModel); // Pass the 'jobViewModel' to the view
        }
 

        [HttpPost]
        public ActionResult ManageJobPosting(JobViewModel model)
        {
            string ManagerUsername = Session["ManagerUsername"] as string;
            Managers currentManager = _context.Managers.FirstOrDefault(m => m.username == ManagerUsername);


            try
            { var job = new Jobs {
                job_name = model.Job.job_name,
                employee_limit = model.Job.employee_limit,
                vacancy = model.Job.employee_limit,
                dep_id = currentManager.dep_id,
                //fix vacany
            };
                _jobController.Create(job);
                var jobs = _context.Jobs.ToList();
                model.JobsList = jobs;
                ViewBag.SuccessMessage = "Registration successful.";
                return View("ManageJobPosting", model);
            }
            catch (Exception ex)
            {

                ViewBag.ErrorMessage = "An error occurred while processing your request.";

                return View(model);


            }
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