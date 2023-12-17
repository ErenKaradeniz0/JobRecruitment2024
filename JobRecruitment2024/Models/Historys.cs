using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobRecruitment2024.Models
{
    public class Historys
    {
        [Key]
        public int history_id { get; set; }
        public DateTime recruitment_date { get; set; }
        public DateTime? dismissal_date { get; set; }
        public int tc { get; set; }
        public int job_id { get; set; }

        // Navigation properties for User and Job (if needed)
        public Users User { get; set; }
        public Jobs Job { get; set; }
    }
}