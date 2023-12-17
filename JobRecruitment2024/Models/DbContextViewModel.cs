﻿using System;
using System.Collections.Generic;
using System.Data.Entity; //entity framework
using System.Linq;
using System.Web;
using static System.Net.Mime.MediaTypeNames;



namespace JobRecruitment2024.Models
{
    public class DbContextViewModel : DbContext

    {
        public DbContextViewModel() : base("name=ConnectionStringName")
        {
            // Constructor
        }
        public DbSet<Users> Users { get; set; }
        public DbSet<Managers> Managers { get; set; }
        public DbSet<Jobs> Jobs { get; set; }
        public DbSet<Applications> Applications { get; set; }
        public DbSet<Historys> Histories { get; set; }




    }
}