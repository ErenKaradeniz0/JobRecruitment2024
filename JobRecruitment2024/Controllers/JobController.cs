using JobRecruitment2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var jobViewModel = new JobViewModel
            {
                JobsList = jobs
            };

            return View(jobViewModel);
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
                    salary = model.salary,
                    employee_limit = model.employee_limit,
                    vacancy = model.employee_limit,
                    dep_id = currentManager.dep_id,
                };
                model.JobsList = JobCreate(job);

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
            string ManagerUsername = Session["ManagerUsername"] as string;
            Managers currentManager = _context.Managers.FirstOrDefault(m => m.username == ManagerUsername);
            int managerDepartmentId = currentManager.dep_id;

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Jobs.Add(job);
                    _context.SaveChanges();
                    ViewBag.SuccessMessage = "Job created successfully.";
                }
                else
                {
                    ViewBag.ErrorMessage = "Job information invalid.";
                }

                var jobs = _context.Jobs
                               .Where(related_jobs => related_jobs.dep_id == managerDepartmentId)
                               .ToList();

                return jobs;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately based on your application's requirements.
                ViewBag.ErrorMessage = "An error occurred while creating the job.";
                // You may want to log the exception for further investigation.
                Console.WriteLine(ex.Message);
                return null; // or return an empty list or handle the error in a way that fits your application.
            }

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

            var histories = _context.Histories.Where(a => a.job_id == job.job_id).ToList();
            if (histories.Any())
            {
                foreach (var history in histories)
                {
                    _context.Histories.Remove(history);
                }
            }

            var employees = _context.Users.Where(a => a.job_id == job.job_id).ToList();
            if (employees.Any())
            {
                foreach(var employee in employees)
                {
                    employee.job_id = null;
                    employee.emp_status = null;
                    employee.salary = 0;
                    job.vacancy += 1;

                }
            }

            _context.Jobs.Remove(job);
            _context.SaveChanges();
            TempData["SuccessMessage"] = "Job posting deleted.";
            return RedirectToAction("ManageJobPosting", "Job");
        }

        [HttpPost]
        public ActionResult JobUpdate(Jobs model)
        {

            var job = _context.Jobs.Find(model.job_id);

            if (job != null && ModelState.IsValid)
            {

                int diff = job.employee_limit - job.vacancy;
                

                job.job_name = model.job_name;
                job.job_description = model.job_description;
                job.salary = model.salary;
                job.employee_limit = model.employee_limit;
                job.vacancy = job.employee_limit - diff;

                _context.SaveChanges();
                TempData["SuccessMessage"] = "Job posting updated.";

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