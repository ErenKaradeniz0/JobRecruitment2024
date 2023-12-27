using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JobRecruitment2024.Models
{
    [NotMapped]
    public class JobViewModel:Jobs
    {
        public IEnumerable<Jobs> JobsList { get; set; }
        public int application_id { get; set; }
        public string app_status { get; set; }
        public string tc { get; set; }

    }
}