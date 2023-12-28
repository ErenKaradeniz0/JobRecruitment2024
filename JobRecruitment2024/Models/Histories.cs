using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobRecruitment2024.Models
{
    public class Histories
    {
        [Key]
        public int history_id { get; set; }
        public string recruitment_date { get; set; }
        public string dismissal_date { get; set; }
        public int job_id { get; set; }
        public string tc { get; set; }
        //make
    }
}