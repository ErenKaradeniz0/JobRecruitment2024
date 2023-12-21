using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobRecruitment2024.Models
{
    public class Managers
    {
        [Key]
        public int manager_id { get; set; }
        public string username { get; set; }
  
        public string name { get; set; }
        public string surname { get; set; }
    

        public string email { get; set; }

        public string password { get; set; }

        public string phone_num { get; set; }

        public int dep_id { get; set; }
    }
}