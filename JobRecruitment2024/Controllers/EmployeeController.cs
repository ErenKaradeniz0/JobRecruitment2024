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
                    job.vacancy -= 1;
                    _context.SaveChanges();

                    string dateString = DateTime.Today.ToString("yyyy-MM-dd");

                    var newHistoryEntry = new Histories
                    {
                        recruitment_date = dateString, // Assuming this is the recruitment date
                        dismissal_date = null, // Assuming this is null when the user is employed
                        job_id = job.job_id,
                        tc = currentUser.tc
                        // You might need to populate other fields based on your table structure
                    };
                    _context.Histories.Add(newHistoryEntry);
                    _context.SaveChanges();


                    TempData["SuccessMessage"] = "Job confirmed. Your other applications deleted.";
                }
                else
                {
                    TempData["ErrorMessage"] = "User not found.";
                }

                return RedirectToAction("UserMainPage", "User");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while confirming the job: " + ex.Message;
                return RedirectToAction("MyApplications","Application");
            }
        }

        [HttpGet]
        public ActionResult EmployeeMainPage()
        {
            var employeeEmail = Session["UserEmail"] as string; // Assuming you store the employee's email in session

            if (employeeEmail != null)
            {
                var employee = _context.Users.FirstOrDefault(e => e.email == employeeEmail);
                if (employee != null)
                {
                    var job = _context.Jobs.FirstOrDefault(j => j.job_id == employee.job_id);
                    if (job != null)
                    {
                        var department = _context.Managers.FirstOrDefault(m => m.dep_id == job.dep_id);
                        if (department != null)
                        {
                            var managers = _context.Managers.FirstOrDefault(m => m.manager_id == department.manager_id);
                            if (managers != null)
                            {
                                var managerInfo = new Managers
                                {
                                    email = managers.email,
                                    phone_num = managers.phone_num,
                                    name = managers.name,
                                    surname = managers.surname
                                };
                                return View(managerInfo);
                            }
                        }
                    }
                }
            }
            else
            {
                ViewBag.ErrorMessage = "User not found. Redirecting Login Page...";
            }
            return View();
        }

        [HttpGet]
        public ActionResult ManageEmployees()
        {

            try
            {
                string ManagerUsername = Session["ManagerUsername"] as string;
                Managers manager = _context.Managers.FirstOrDefault(m => m.username == ManagerUsername);

                if (manager != null)
                {
                    var employees = _context.Users
                       .Join(
                           _context.Jobs,
                           user => user.job_id,
                           job => job.job_id,
                           (user, job) => new { User = user, Job = job }
                       )
                       .Where(joined => joined.User.job_id == joined.Job.job_id && joined.Job.dep_id == manager.dep_id)
                       .Select(joined => joined.User)
                       .ToList();

                    if (employees == null || !employees.Any())
                    {
                        ViewBag.ErrorMessage = "No Employees Found";
                    }

                    var UserViewModel = new UserViewModel
                    {
                        UserList = employees
                    };

                    return View(UserViewModel);
                }
                else
                {
                    ViewBag.ErrorMessage = "Manager not found. Redirecting Login Page...";
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
        public ActionResult ManageEmployees(Managers manager)
        {
            return View();
        }

        [HttpPost]
        public ActionResult EmployeeUpdate(Users model)
        {

            var employee = _context.Users.Find(model.tc);

            if (employee != null && ModelState.IsValid)
            {

                employee.salary = model.salary;
                _context.SaveChanges();

            }
            return RedirectToAction("ManageEmployees", "Employee");
        }
        [HttpPost]
        public ActionResult FireEmployee(string tc)
        {
            var employee = _context.Users.Find(tc);
            var job = _context.Jobs.FirstOrDefault(u => u.job_id == employee.job_id);
            if (employee == null)
            {
                return HttpNotFound();
            }

            var latestHistoryEntry = _context.Histories
                 .Where(h => h.job_id == job.job_id && h.tc == employee.tc)
                 .OrderByDescending(h => h.history_id)
                 .FirstOrDefault();
            string dateString = DateTime.Today.ToString("yyyy-MM-dd");
            if (latestHistoryEntry != null)
            {
                latestHistoryEntry.dismissal_date = dateString;
                _context.SaveChanges();
            }

            employee.job_id = null;
            employee.emp_status = null;
            employee.salary = 0;
            job.vacancy += 1;
            _context.SaveChanges();

            return RedirectToAction("ManageEmployees", "Employee");
        }

        [HttpPost]

        public ActionResult LeaveJob(string tc)
        {

            try
            {
                var employee = _context.Users.Find(tc);
                var job = _context.Jobs.FirstOrDefault(u => u.job_id == employee.job_id);

                var latestHistoryEntry = _context.Histories
                     .Where(h => h.job_id == job.job_id && h.tc == employee.tc)
                     .OrderByDescending(h => h.history_id)
                     .FirstOrDefault();
                string dateString = DateTime.Today.ToString("yyyy-MM-dd");
                if (latestHistoryEntry != null)
                {
                    latestHistoryEntry.dismissal_date = dateString;
                    _context.SaveChanges();
                }
                employee.job_id = null;
                employee.emp_status = null;
                employee.salary = 0;
                job.vacancy += 1;
                _context.SaveChanges();
                TempData["SuccessMessage"] = "You Left your Job.";
                return RedirectToAction("UserMainPage", "User");
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
    }

}