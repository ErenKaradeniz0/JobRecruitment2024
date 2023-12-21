﻿using JobRecruitment2024.Models;
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

        private readonly DbContextViewModel _context;

        public HomeController()
        {
            _context = new DbContextViewModel();
        }
        public ActionResult Index()
        {
            try
            {
                var users = _context.Managers.ToList();
                ViewBag.Message = "Connection successful!!";
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    ViewBag.Message = "Connection unsuccessful! " + ex.Message;
                    Console.WriteLine(ex.Message);
                    ex = ex.InnerException;
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult Forgot_Password()
        {
            string referrerUrl = Request.UrlReferrer?.ToString();
            if (referrerUrl.Contains("Manager"))
                Session["userType"] = "Manager";

            else if (referrerUrl.Contains("User"))
               Session["userType"] = "User";

            return View();
        }

        [HttpPost]
        public ActionResult Forgot_Password(string email)
        {
            string newPassword = GenerateRandomPassword();
            string userType = Session["userType"] as string;
            if (userType == "Manager")
            {
                var client = _context.Managers.FirstOrDefault(m => m.email == email);
                client.password = newPassword;
            }
            else if (userType == "User")
            {
                var client = _context.Users.FirstOrDefault(u => u.email == email);
                client.password = newPassword;
            }
             
     
            if (SendPasswordResetEmail(email, newPassword))
            {
                ViewBag.SuccessMessage = "A new password has been sent to your email.";
                _context.SaveChanges();
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
                smtpClient.Credentials = new System.Net.NetworkCredential("jobrecruitmenteren@gmail.com", "ysmeldaixamakcla");


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