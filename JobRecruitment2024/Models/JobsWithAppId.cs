﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JobRecruitment2024.Models
{
    [NotMapped]
    public class JobsWithAppId : Jobs
    {
        public int application_id { get; set; }
    }
}