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
                    var appliedJobsForUser = _context.Applications
                       .Where(a => a.tc == currentUser.tc)
                       .Select(a => a.job_id)
                       .ToList();

                    var availableJobs = _context.Jobs.Where(j => !appliedJobsForUser.Contains(j.job_id)).ToList();
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
                    List<JobsWithAppId> userApplications = (from app in _context.Applications
                                                            join job in _context.Jobs on app.job_id equals job.job_id
                                                            where app.tc == currentUser.tc
                                                            select new JobsWithAppId
                                                            {
                                                                job_id = job.job_id,
                                                                job_name = job.job_name,
                                                                job_description = job.job_description,
                                                                employee_limit = job.employee_limit,
                                                                vacancy = job.vacancy,
                                                                dep_id = job.dep_id,
                                                                application_id = app.application_id
                                                            }).ToList();
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

            // Redirect to the action that displays the updated application list
            return RedirectToAction("MyApplications");
        }
    }
}
