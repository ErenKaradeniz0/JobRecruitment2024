using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobRecruitment2024.Models
{
    public class Application
    {
        [Key]
        public int application_id { get; set; }
        public string app_status { get; set; }
        public int job_id { get; set; }
        public int tc { get; set; }

        // Navigation properties for User and Job (if needed)
        public User User { get; set; }
        public Job Job { get; set; }
    }
}