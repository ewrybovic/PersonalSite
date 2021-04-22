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
        public PersonalSiteContext (DbContextOptions<PersonalSiteContext> options)
            : base(options)
        {
        }

        public DbSet<PersonalSite.Models.User> User { get; set; }
    }
}
