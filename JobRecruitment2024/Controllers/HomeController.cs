﻿using JobRecruitment2024.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
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
        /*Main page
        Check database connection
        Select Account Type
         */
        public ActionResult Index()
        {
            try
            {
                Debug.WriteLine(PasswordEncrypt("a"));
                var managers = _context.Managers.ToList();
                ViewBag.Message = "Database connection successful!";
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    ViewBag.Message = "Database Connection unsuccessful! " + ex.Message;
                    Console.WriteLine(ex.Message);
                    ex = ex.InnerException;
                }
            }
            return View();
        }

        /* Forgot Password
        Reset password of current account via email
        Select Account Type
        Save new Encyrpted Password to Database
        */
        [HttpGet]
        public ActionResult Forgot_Password()
        {
            string referrerUrl = Request.UrlReferrer?.ToString();
            if (referrerUrl != null)
            {
                if (referrerUrl.Contains("Manager"))
                    Session["userType"] = "Manager";

                else if (referrerUrl.Contains("User"))
                    Session["userType"] = "User";
            }

            return View();
        }

        [HttpPost]
        public ActionResult Forgot_Password(string email)
        {
            string newPassword = GenerateRandomPassword();
            string encryptedPassword = PasswordEncrypt(newPassword);
            string userType = Session["userType"] as string;
            IUserType client = null;

            if (userType == "Manager")
            {
                client = _context.Managers.FirstOrDefault(m => m.email == email);
                if (client != null)
                {
                    if (client is Managers manager)
                    {
                        manager.password = encryptedPassword;
                    }
                }
            }
            else if (userType == "User")
            {
                client = _context.Users.FirstOrDefault(u => u.email == email);
                if (client != null)
                {
                    if (client is Users user)
                    {
                        user.password = encryptedPassword;
                    }
                }
            }
            if (client != null)
            {
                if (SendPasswordResetEmail(email, newPassword))
                {
                    ViewBag.SuccessMessage = "A new password has been sent to your email.";
                    _context.SaveChanges();
                }
                else
                {
                    ViewBag.ErrorMessage = "Failed to send the new password. Please try again.";
                }
            }
            else
            {
                ViewBag.ErrorMessage = "You do not have an account.";
            }


            return View();

        }
        /* Send Password Reset Email
         Send new Password to email
         */
        private bool SendPasswordResetEmail(string email, string newPassword)
        {
            string userType = Session["userType"] as string;


            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("jobrecruitmenteren@gmail.com");
                mail.To.Add(email);
                mail.Subject = "Password Reset";

                if (userType == "Manager")
                {
                    var client = _context.Managers.FirstOrDefault(m => m.email == email);
                    mail.Body = "Hello " + client.name +" "+client.surname + ", Your new password: " + newPassword;
                }
                else if (userType == "User")
                {
                    var client = _context.Users.FirstOrDefault(u => u.email == email);
                    mail.Body = "Hello " + client.name + " " + client.surname + "\nYour new password: " + newPassword;
                }
             

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

        /* Generate Random Password
        Generate Random Password
        */
        private string GenerateRandomPassword()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 8).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /* Password Encrypt
         Encrypt Password
         */
        public static string PasswordEncrypt(string password)
        {
            int minAsciiVal = 32;
            int maxAsciiVal = 126;
            int key = 13;
            int mirrorKey = -17;

            string crypto = "";
            string cryptoMirror = "";

            string mirror = new string(password.Reverse().ToArray());

            for (int i = 0; i < password.Length; i++)
            {
                int charPassword = (int)password[i] + key;
                int charMirror = (int)mirror[i] + mirrorKey;

                if (charPassword > maxAsciiVal)
                    charPassword = charPassword - maxAsciiVal + minAsciiVal - 1;

                if (charMirror < minAsciiVal)
                    charMirror = charMirror + maxAsciiVal - minAsciiVal + 1;

                // Escape single quotes in the password
                if (charPassword == 39)
                {
                    charPassword = int.Parse("39" + charPassword);
                }

                if (charMirror == 39)
                {
                    charMirror = int.Parse("39" + charMirror);
                }

                // Escape double quotes in the password
                if (charPassword == 34)
                {
                    charPassword = int.Parse("34" + charPassword);
                }

                if (charMirror == 34)
                {
                    charMirror = int.Parse("34" + charMirror);
                }

                crypto += (char)charPassword;
                cryptoMirror += (char)charMirror;
            }

            string newPassword = crypto + cryptoMirror;

            return newPassword;
        }

    }
}