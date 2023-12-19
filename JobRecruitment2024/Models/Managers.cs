using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobRecruitment2024.Models
{
    public class Managers
    {
        [Key]
        public int manager_id { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string username { get; set; }
        public string password { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string name { get; set; }
        [Required(ErrorMessage = "Surname is required")]
        public string surname { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string email { get; set; }
        public int dep_id { get; set; }

        // Navigation property for Department (if needed)
        public Departments Department { get; set; }
    }
}