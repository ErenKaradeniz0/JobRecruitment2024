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
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public void Create(Jobs job)
        {
            if (ModelState.IsValid)
            {   
                _context.Jobs.Add(job);
                _context.SaveChanges();
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
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
        public ActionResult Edit(int id)
        {
            var job = _context.Jobs.Find(id);

            return View("ManageJobPosting","Manager");
        }

        [HttpPost]
        public ActionResult Edit(Jobs job)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(job).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index"); // Redirect to job listing or another appropriate action
            }
            return View(job);
    
        }

    }
}