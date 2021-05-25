using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Models;

namespace PersonalSite.Data
{
    public class PersonalSiteContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=PersonalSiteDB.db");
        }

        public DbSet<PersonalSite.Models.User> User { get; set; }
    }
}
