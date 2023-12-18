using System;
using System.ComponentModel.DataAnnotations;

namespace JobRecruitment2024.Models
{
    public class Users
    {
        [Key]
        public string tc { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        public string surname { get; set; }

        public string insurance_num { get; set; }
        public decimal salary { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string email { get; set; }
        public string password { get; set; }

        public string emp_status { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        public string phone_num { get; set; }
    }
}