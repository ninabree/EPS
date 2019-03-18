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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<UserModel> Account { get; set; }
        public DbSet<DMDeptModel> DMDept { get; set; }
        public DbSet<DMPayeeModel> DMPayee { get; set; }
        public DbSet<DMCheckModel> DMCheck { get; set; }
        public DbSet<DMAccountModel> DMAccount { get; set; }
        public DbSet<DMVATModel> DMVAT { get; set; }
        public DbSet<DMFBTModel> DMFBT { get; set; }
        public DbSet<DMEWTModel> DMEWT { get; set; }
        public DbSet<DMCurrencyModel> DMCurrency { get; set; }
        public DbSet<BudgetModel> Budget { get; set; }
        public DbSet<SystemMessageModel> SystemMessageModels { get; set; }

        //PENDING ENTRY TABLES
        public DbSet<DMPayeeModel_Pending> DMPayee_Pending { get; set; }
    }
}
