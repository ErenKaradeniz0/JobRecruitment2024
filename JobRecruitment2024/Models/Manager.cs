using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JobRecruitment2024.Models
{
    public class Manager
    {
        public int ManagerID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public int DepID { get; set; }
    }
}