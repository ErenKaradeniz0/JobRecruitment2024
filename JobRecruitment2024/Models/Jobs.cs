﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobRecruitment2024.Models
{
    public class Jobs
    {
        [Key]
        public int job_id { get; set; }
        public string job_name { get; set; }
        public string job_description { get; set; }
        public int salary { get; set; }
        public int employee_limit { get; set; }
        public int vacancy {  get; set; }
        public int dep_id { get; set; }
    
    }
}