using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JobRecruitment2024.Models
{
    [NotMapped]
    public class UserViewModel:Users
    {
        public IEnumerable<Users> UserList { get; set; }

        public string job_name {  get; set; }
    }
}