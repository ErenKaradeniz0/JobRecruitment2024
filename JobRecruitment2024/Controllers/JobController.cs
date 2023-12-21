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

        [HttpPost]
        public void JobCreate(Jobs job)
        {
            if (ModelState.IsValid)
            {
                _context.Jobs.Add(job);
                _context.SaveChanges();
            }
        }

        [HttpPost]
        public ActionResult JobDelete(int id)
        {
            var job = _context.Jobs.Find(id);
            if (job == null)
            {
                return HttpNotFound();
            }

            _context.Jobs.Remove(job);
            _context.SaveChanges();
            return RedirectToAction("ManageJobPosting", "Manager");
        }

        [HttpGet]
        public ActionResult JobUpdate(int job_id)
        {
            var jobtest = job_id;

            return View("ManageJobPosting", "Manager");
        }

        [HttpPost]
        public ActionResult Edit(Jobs model)
        {

                var job = _context.Jobs.Find(model.job_id);

                if (job != null && ModelState.IsValid)
                {

                    job.job_name = model.job_name;
                    job.employee_limit = model.employee_limit;

                    // Save changes to the database
                    _context.SaveChanges();

                    return RedirectToAction("ManageJobPosting", "Manager");
                }

                // If the job was not found or the model state is invalid, return to the view with the job
                return View(job);
            }
        }
    }