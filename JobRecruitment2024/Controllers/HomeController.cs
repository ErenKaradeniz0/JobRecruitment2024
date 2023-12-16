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
        public ActionResult Index()
        {
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
                smtpClient.Credentials = new System.Net.NetworkCredential("jobrecruitmenteren@gmail.com", "xxxx");


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
            // Your password generation logic goes here
            // This is a simple example; you might use a stronger password generation method
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }




    }

}