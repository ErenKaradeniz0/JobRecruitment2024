using JobRecruitment2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace JobRecruitment2024.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly DbContextViewModel _context;

        public ApplicationController()
        {
            _context = new DbContextViewModel();
        }
        [HttpGet]
        public ActionResult ApplyJob()
        {
            try
            {
                string userEmail = Session["UserEmail"] as string;
                var currentUser = _context.Users.FirstOrDefault(u => u.email == userEmail);

                if (currentUser != null)
                {
                    if (currentUser.job_id != null)
                    {
                        ViewBag.StatusMessage = "You have been employed.";
                        return View();
                    }

                    List<JobViewModel> jobsNotApplied = (from job in _context.Jobs
                                                         join dep in _context.Departments on job.dep_id equals dep.dep_id
                                                         where !_context.Applications.Any(app => app.job_id == job.job_id && app.tc == currentUser.tc)
                                                            && job.vacancy > 0
                                                         select new JobViewModel
                                                         {
                                                             job_id = job.job_id,
                                                             job_name = job.job_name,
                                                             job_description = job.job_description,
                                                             employee_limit = job.employee_limit,
                                                             vacancy = job.vacancy,
                                                             dep_id = job.dep_id,
                                                             dep_name=dep.dep_name,
                                                             application_id = null, // No application ID for jobs not applied
                                                             app_status = null,    // No application status for jobs not applied
                                                             tc = null             // No user TC for jobs not applied
                                                         }).ToList();

                    return View(jobsNotApplied);

                }
                else
                {
                    ViewBag.ErrorMessage = "User not found. Redirecting Log in...";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while processing your request. " + ex;

                return View();
            }
        }


        [HttpPost]
        public ActionResult ApplyJob(int job_id_apply)
        {
            string userEmail = Session["UserEmail"] as string;
            var user = _context.Users.FirstOrDefault(u => u.email == userEmail);

            var existingApplication = _context.Applications
                    .FirstOrDefault(a => a.tc == user.tc && a.job_id == job_id_apply);


            if (existingApplication != null)
            {
                ViewBag.ErrorMessage = "You have already applied for this job.";
                return View();
            }

            var application = new Applications
            {
                tc = user.tc,
                job_id = job_id_apply,
                app_status = "Pending",

            };

            _context.Applications.Add(application);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "You have successfully applied for the selected job";

            return RedirectToAction("ApplyJob");
        }

        public ActionResult MyApplications()
        {
            try
            {
                string userEmail = Session["UserEmail"] as string;
                var currentUser = _context.Users.FirstOrDefault(u => u.email == userEmail);

                if (currentUser != null)
                {
                    List<JobViewModel> userApplications = (from app in _context.Applications
                                                            join job in _context.Jobs on app.job_id equals job.job_id
                                                            join dep in _context.Departments on job.dep_id equals dep.dep_id
                                                            where app.tc == currentUser.tc && job.vacancy > 0
                                                            select new JobViewModel
                                                            {
                                                                job_id = job.job_id,
                                                                job_name = job.job_name,
                                                                job_description = job.job_description,
                                                                employee_limit = job.employee_limit,
                                                                vacancy = job.vacancy,
                                                                dep_id = job.dep_id,
                                                                dep_name=dep.dep_name,
                                                                application_id = app.application_id,
                                                                app_status = app.app_status,
                                                                tc = app.tc,
                                                            }).ToList();
                    if (TempData["SuccessMessage"] != null)
                    {
                        ViewBag.SuccessMessage = TempData["SuccessMessage"];
                    }

                    return View(userApplications);
                }
                else
                {
                    ViewBag.ErrorMessage = "User not found. Redirecting Login Page...";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while processing your request. " + ex;

                return View();
            }
        }
        [HttpPost]
        public ActionResult DeleteApplication(int applicationId)
        {
            var application = _context.Applications.Find(applicationId);
            if (application == null)
            {
                return HttpNotFound(); // Handle not found
            }

            _context.Applications.Remove(application);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "You have deleted the selected job application.";


            // Redirect to the action that displays the updated application list
            return RedirectToAction("MyApplications");
        }
        [HttpGet]
        public ActionResult ManageApplications()
        {
            string ManagerUsername = Session["ManagerUsername"] as string;
            var manager = _context.Managers.FirstOrDefault(m => m.username == ManagerUsername);
            if (manager != null)
            {

                List<JobViewModel> applications = (from app in _context.Applications
                                                     join job in _context.Jobs on app.job_id equals job.job_id
                                                     join user in _context.Users on app.tc equals user.tc
                                                     where job.dep_id == manager.dep_id
                                                     select new JobViewModel
                                                     {
                                                         application_id = app.application_id,
                                                         job_id = job.job_id,
                                                         name = user.name,
                                                         surname = user.surname,
                                                         job_name = job.job_name,
                                                         app_status = app.app_status,
                                                     }).ToList();


                return View(applications); // Pass the list of applications to the view
            }
            ViewBag.ErrorMessage = "Manager not found. Redirecting Main Page...";

            return View();
        }

        // Action method to accept an application
        [HttpPost]
        public ActionResult AcceptApplication(int application_id)
        {
            try
            {
                var application = _context.Applications.FirstOrDefault(a => a.application_id == application_id);

                if (application != null)
                {
                    application.app_status = "Accepted";
                    _context.SaveChanges();

                    // Set success message
                    TempData["SuccessMessage"] = "Application accepted successfully!";
                }
                else
                {
                    // Set error message if the application is not found
                    TempData["ErrorMessage"] = "An error occurred. Application not found.";
                }
            }
            catch (Exception ex)
            {
                // Set error message if an exception occurs during the update
                TempData["ErrorMessage"] = "An error occurred while accepting the application." + ex;
            }

            return RedirectToAction("ManageApplications");
        }


        [HttpPost]
        public ActionResult RejectApplication(int application_id)
        {
            try
            {
                var application = _context.Applications.Find(application_id);

                if (application != null)
                {
                    application.app_status = "Rejected";
                    _context.SaveChanges();

                    // Set success message
                    TempData["SuccessMessage"] = "Application rejected successfully!";
                }
                else
                {
                    // Set error message if the application is not found
                    TempData["ErrorMessage"] = "An error occurred. Application not found.";
                }
            }
            catch (Exception ex)
            {
                // Set error message if an exception occurs during the update
                TempData["ErrorMessage"] = "An error occurred while rejecting the application."+ex;
            }

            return RedirectToAction("ManageApplications");
        }

    }
}
