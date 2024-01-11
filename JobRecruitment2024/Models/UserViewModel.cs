﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JobRecruitment2024.Models
{
    [NotMapped]
    public class UserViewModel:Users
    {
        public IEnumerable<UserViewModel> UserList { get; set; }

        public string job_name {  get; set; }

        public string dep_name {  get; set; }
    }
}