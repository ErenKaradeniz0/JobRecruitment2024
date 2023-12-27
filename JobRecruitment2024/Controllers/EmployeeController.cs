using JobRecruitment2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace JobRecruitment2024.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly DbContextViewModel _context;

        public EmployeeController()
        {
            _context = new DbContextViewModel();
        }

        [HttpPost]
        public ActionResult Employment(int jobId, string tc)
        {
            try
            {
                // Retrieve the current user
                var currentUser = _context.Users.FirstOrDefault(u => u.tc == tc);

                if (currentUser != null)
                {
                    // Delete other applications for the user
                    _context.Applications.RemoveRange(_context.Applications.Where(app => app.tc == currentUser.tc));
                    var job = _context.Jobs.FirstOrDefault(u => u.job_id == jobId);
                    // Update user's employment status to "employee"
                    currentUser.emp_status = "employee";
                    currentUser.job_id = job.job_id;
                    currentUser.salary = job.salary;
                    currentUser.insurance_num = currentUser.tc;
                   



                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Job confirmed. Your other applications deleted.";
                }
                else
                {
                    TempData["ErrorMessage"] = "User not found.";
                }

                return RedirectToAction("UserMainPage","User");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while confirming the job: " + ex.Message;
                return RedirectToAction("MyApplications");
            }
        }
        [HttpGet]
        public ActionResult ManageEmployees()
        {
            return View();
        }
    }
}