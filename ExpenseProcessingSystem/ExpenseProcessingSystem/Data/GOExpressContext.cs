using ExpenseProcessingSystem.Models.Gbase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Data
{
    public class GOExpressContext : DbContext
    {
        public GOExpressContext(DbContextOptions<GOExpressContext> options)
                : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<tblCM10Model> tblCM10 { get; set; }
    }
}
