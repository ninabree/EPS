using ExpenseProcessingSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Data
{
    public class EPSDbContext : DbContext
    {
        public EPSDbContext(DbContextOptions<EPSDbContext> options)
                : base(options)
        { }

        public EPSDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<AccountModel> Account { get; set; }
        public DbSet<DMDeptModel> DMDept { get; set; }
        public DbSet<DMPayeeModel> DMPayee { get; set; }
        public DbSet<SystemMessageModel> SystemMessageModels { get; set; }
    }
}
