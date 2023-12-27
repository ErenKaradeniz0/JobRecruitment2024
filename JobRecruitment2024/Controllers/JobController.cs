using JobRecruitment2024.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobRecruitment2024.Controllers
{
    public class JobController : Controller
    {
        private readonly DbContextViewModel _context;

        public JobController()
        {
            _context = new DbContextViewModel();
        }
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
            int managerDepartmentId = currentManager.dep_id;
            var jobs = _context.Jobs
                .Where(job => job.dep_id == managerDepartmentId)
                .ToList();


            if (jobs == null || !jobs.Any())
            {
                ViewBag.ErrorMessage = "No Jobs Found. Redirecting mainpage...";
            }

            var jobViewModel = new JobViewModel
            {
                JobsList = jobs
            };

            return View(jobViewModel); // Pass the 'jobViewModel' to the view
        }


        [HttpPost]
        public ActionResult ManageJobPosting(JobViewModel model)
        {
            string ManagerUsername = Session["ManagerUsername"] as string;
            Managers currentManager = _context.Managers.FirstOrDefault(m => m.username == ManagerUsername);

            try
            {
                var job = new Jobs
                {
                    job_name = model.job_name,
                    job_description = model.job_description,
                    employee_limit = model.employee_limit,
                    vacancy = model.employee_limit,
                    dep_id = currentManager.dep_id,
                    //fix vacany
                };
                model.JobsList = JobCreate(job);

                ViewBag.SuccessMessage = "Job created successfully.";
                return View("ManageJobPosting", model);
            }
            catch (Exception ex)
            {

                ViewBag.ErrorMessage = "An error occurred while processing your request." + ex;

                return View(model);
            }
        }
        [HttpPost]
        public List<Jobs> JobCreate(Jobs job)
        {
            if (ModelState.IsValid)
            {
                _context.Jobs.Add(job);
                _context.SaveChanges();

                string ManagerUsername = Session["ManagerUsername"] as string;
                Managers currentManager = _context.Managers.FirstOrDefault(m => m.username == ManagerUsername);

                int managerDepartmentId = currentManager.dep_id;
                var jobs = _context.Jobs
                           .Where(related_jobs => related_jobs.dep_id == managerDepartmentId) // Changed to related_jobs.dep_id
                           .ToList();
                return jobs;
            }
            else
                return null;
        }

        [HttpPost]
        public ActionResult JobDelete(int job_id)
        {
            var job = _context.Jobs.Find(job_id);
            if (job == null)
            {
                return HttpNotFound();
            }

            var applications = _context.Applications.Where(a => a.job_id == job.job_id).ToList();
            if (applications.Any())
            {
                foreach (var application in applications)
                {
                    _context.Applications.Remove(application);
                }
            }

            _context.Jobs.Remove(job);
            _context.SaveChanges();

            return RedirectToAction("ManageJobPosting", "Job");
        }

        [HttpPost]
        public ActionResult JobUpdate(Jobs model)
        {

            var job = _context.Jobs.Find(model.job_id);

            if (job != null && ModelState.IsValid)
            {

                job.job_name = model.job_name;
                job.job_description = model.job_description;
                job.employee_limit = model.employee_limit;
                job.vacancy = model.employee_limit;
                //vacancy

                _context.SaveChanges();

            }
            return RedirectToAction("ManageJobPosting", "Job");
        }

        [HttpGet]
        public ActionResult ManageEmployees()
        {
            if (Session["ManagerUsername"] == null)
                ViewBag.ErrorMessage = "Manager not found. Redirecting Main Page...";

            return View();
        }

    }

}