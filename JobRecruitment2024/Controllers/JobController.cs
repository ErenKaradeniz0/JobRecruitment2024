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

            return RedirectToAction("ManageJobPosting", "Manager");
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

                    return RedirectToAction("ManageJobPosting", "Manager");
                }

                return RedirectToAction("ManageJobPosting","Manager");
            }
        }
    }