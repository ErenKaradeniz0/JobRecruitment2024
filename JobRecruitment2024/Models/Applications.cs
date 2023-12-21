using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobRecruitment2024.Models
{
    public class Applications
    {
        [Key]
        public int application_id { get; set; }
        public string app_status { get; set; }
        public int job_id { get; set; }
        public string tc { get; set; }
    }
}