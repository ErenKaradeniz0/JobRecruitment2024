using JobRecruitment2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
                    var appliedJobsForUser = _context.Applications
                        .Where(a => a.tc == currentUser.tc)
                        .Select(a => a.job_id)
                        .ToList();

                    var availableJobs = _context.Jobs
                        .Where(j => !appliedJobsForUser.Contains(j.job_id) && j.vacancy > 0)
                        .ToList();

                    return View(availableJobs);

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
                ViewBag.ErrorMessage = "You've already applied for this job.";
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
                                                            where app.tc == currentUser.tc && job.vacancy > 0
                                                            select new JobViewModel
                                                            {
                                                                job_id = job.job_id,
                                                                job_name = job.job_name,
                                                                job_description = job.job_description,
                                                                employee_limit = job.employee_limit,
                                                                vacancy = job.vacancy,
                                                                dep_id = job.dep_id,
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
            TempData["SuccessMessage"] = "You've deleted the selected job application.";


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

                var applications = (from app in _context.Applications
                                    join job in _context.Jobs on app.job_id equals job.job_id
                                    where job.dep_id == manager.dep_id
                                    select app).ToList();

                return View(applications); // Pass the list of applications to the view
            }
            ViewBag.ErrorMessage = "Manager not found. Redirecting Main Page...";

            return View();
        }

        // Action method to accept an application
        [HttpPost]
        public ActionResult AcceptApplication(int application_id)
        {   
            var application = _context.Applications.FirstOrDefault(a => a.application_id == application_id);
            if (application != null)
            {
                application.app_status = "Accepted";
                _context.SaveChanges();
            }
            return RedirectToAction("ManageApplications");
        }

        [HttpPost]
        public ActionResult RejectApplication(int application_id)
        {
            var application = _context.Applications.Find(application_id);
            if (application != null)
            {
                application.app_status = "Rejected";
                _context.SaveChanges();
            }
            return RedirectToAction("ManageApplications");
        }
    }
}
