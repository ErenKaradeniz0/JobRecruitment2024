using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobRecruitment2024.Models
{
    public class JobViewModel
    {
        public IEnumerable<Jobs> JobsList { get; set; }
        public Jobs Job { get; set; }
    }
}