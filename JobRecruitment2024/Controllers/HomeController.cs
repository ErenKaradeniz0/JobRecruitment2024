using JobRecruitment2024.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace JobRecruitment2024.Controllers
{
    public class HomeController : Controller
    {

        private readonly DbContextModel _context;

        public HomeController()
        {
            _context = new DbContextModel();
        }
        public ActionResult Index()
        {
            try
            {
                var users = _context.Managers.ToList();
                ViewBag.Message = "Bağlantı Başarılı!";
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    ViewBag.Message = "Bağlantı Başarısız" +ex.Message;
                    Console.WriteLine(ex.Message);
                    ex = ex.InnerException;
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult Forgot_Password()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Forgot_Password(string email)
        {
            string newPassword = GenerateRandomPassword();

            if (SendPasswordResetEmail(email, newPassword))
            {
                ViewBag.SuccessMessage = "A new password has been sent to your email.";
            }
            else
            {
                ViewBag.ErrorMessage = "Failed to send the new password. Please try again.";
            }

            return View();

        }
        private bool SendPasswordResetEmail(string email, string newPassword)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("jobrecruitmenteren@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Password Reset";
                mail.Body = "Your new password: " + newPassword;

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential("jobrecruitmenteren@gmail.com", "xxxxxx");


                smtpClient.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        private string GenerateRandomPassword()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}