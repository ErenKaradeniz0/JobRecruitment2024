using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobRecruitment2024.Models
{
    public class Department
    {
        [Key]
        public int dep_id { get; set; }
        public string dep_name { get; set; }
    }
}