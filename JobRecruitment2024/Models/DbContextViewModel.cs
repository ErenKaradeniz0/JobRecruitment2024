using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using static System.Net.Mime.MediaTypeNames;



namespace JobRecruitment2024.Models
{
    public class DbContextViewModel : DbContext

    {
        public DbContextViewModel() : base("name=ConnectionStringName")
        {

        }
        public DbSet<Users> Users { get; set; }
        public DbSet<Managers> Managers { get; set; }
        public DbSet<Departments> Departments { get; set; }
        public DbSet<Jobs> Jobs { get; set; }
        public DbSet<Applications> Applications { get; set; }
        public DbSet<Histories> Histories { get; set; }


    }
}