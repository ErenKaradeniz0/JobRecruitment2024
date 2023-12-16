using System;
using System.Collections.Generic;
using System.Data.Entity; //entity framework
using System.Linq;
using System.Web;
using static System.Net.Mime.MediaTypeNames;



namespace JobRecruitment2024.Models
{
    public class DbContextModel : DbContext

    {
        public DbContextModel() : base("name=ConnectionStringName")
        {
            // Constructor
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<History> Histories { get; set; }




    }
}