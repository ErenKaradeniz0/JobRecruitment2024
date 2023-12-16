using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobRecruitment2024.Models
{
    public class Job
    {
        [Key]
        public int job_id { get; set; }
        public string job_name { get; set; }
        public int employee_limit { get; set; }
        public int dep_id { get; set; }

        // Navigation property for Department (if needed)
        public Department Department { get; set; }
    }
}