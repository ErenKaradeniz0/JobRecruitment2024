using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobRecruitment2024.Models
{
    public class Users
    {
        [Key]
        public string tc { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string insurance_num { get; set; }
        public decimal salary { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string emp_status { get; set; }
        public string phone_num { get; set; }
    }
}