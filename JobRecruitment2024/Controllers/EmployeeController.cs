using JobRecruitment2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

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
                    if(job.vacancy > 0)
                    {
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
                            recruitment_date = dateString,
                            dismissal_date = null,
                            job_id = job.job_id,
                            tc = currentUser.tc
                        };
                        _context.Histories.Add(newHistoryEntry);
                        _context.SaveChanges();


                        TempData["SuccessMessage"] = "Job confirmed. Your other applications deleted.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "There is no vacany.";
                    }
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
        public ActionResult JobInformation()
        {
            string employeeEmail = Session["UserEmail"] as string;

            if (employeeEmail == null)
            {
                ViewBag.ErrorMessage = "User not found. Redirecting to Login Page...";
                return RedirectToAction("UserLoginPage", "User");
            }

            var currentUser = _context.Users.FirstOrDefault(u => u.email == employeeEmail);

            if (currentUser == null)
            {
                ViewBag.ErrorMessage = "User not found.";
                return View();
            }

            var userInfo = (from user in _context.Users
                            join job in _context.Jobs on user.job_id equals job.job_id
                            join dep in _context.Departments on job.dep_id equals dep.dep_id
                            join manager in _context.Managers on dep.dep_id equals manager.dep_id
                            where job.job_id == currentUser.job_id
                            select new UserViewModel
                            {
                                tc = user.tc,
                                job_id = job.job_id,
                                job_name = job.job_name,
                                salary = job.salary,
                                name = manager.name,
                                surname = manager.surname,
                                email = manager.email,
                                phone_num = manager.phone_num,
                                dep_name = dep.dep_name,
                            }).FirstOrDefault();

            if (userInfo == null)
            {
                ViewBag.ErrorMessage = "You are not currently working.";
                return View();
            }

            return View(userInfo);
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

                    List<UserViewModel> employees = (from user in _context.Users
                                                         join job in _context.Jobs on user.job_id equals job.job_id
                                                         where job.dep_id == manager.dep_id
                                                         select new UserViewModel
                                                         {
                                                             tc = user.tc,
                                                             name = user.name,
                                                             surname = user.surname,
                                                             email = user.email,
                                                             phone_num = user.phone_num,
                                                             insurance_num = user.insurance_num,
                                                             salary = user.salary,
                                                             job_id = job.job_id,
                                                             job_name = job.job_name,
                                                         }).ToList();


                    if (employees == null || !employees.Any())
                    {
                        ViewBag.ErrorMessage = "No Employees Found";
                    }

                    UserViewModel employeesModel = new UserViewModel
                    {
                        UserList = employees
                    };

                    return View(employeesModel);
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
        public ActionResult ManageEmployees(List<UserViewModel> employees)
        {
            return View(employees);
        }

        [HttpPost]
        public ActionResult EmployeeUpdate(Users model)
        {

            var employee = _context.Users.Find(model.tc);

            if (employee != null)
            {
                try
                {
                    employee.name = model.name;
                employee.surname = model.surname;
                employee.email = model.email;
                employee.phone_num = model.phone_num;
                employee.salary = model.salary;
             
                _context.SaveChanges();
                 TempData["SuccessMessage"] = "Employee updated successfully!";
                }
                catch (Exception e)
                {
                TempData["ErrorMessage"] = "An error occurred while updating the employee." +e;
                }

            }
            return RedirectToAction("ManageEmployees", "Employee");
        }
        [HttpPost]
        public ActionResult FireEmployee(string tc)
        {
            try
            {
                var employee = _context.Users.Find(tc);
                var job = _context.Jobs.FirstOrDefault(u => u.job_id == employee.job_id);
                if (employee == null || job == null)
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
                TempData["SuccessMessage"] = "You fired selected Employee.";
                return RedirectToAction("ManageEmployees", "Employee");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while firing the employee."+ex;

                if (ex.InnerException != null)
                {
                    ViewBag.InnerErrorMessage = ex.InnerException.Message;
                }

                return View("Index", "Home");
            }
        }

        [HttpPost]

        public ActionResult LeaveJob(string tc)
        {

            try
            {
                var employee = _context.Users.Find(tc);
                var job = _context.Jobs.FirstOrDefault(u => u.job_id == employee.job_id);
                if (employee == null || job == null)
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
                TempData["SuccessMessage"] = "You left your Job.";
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