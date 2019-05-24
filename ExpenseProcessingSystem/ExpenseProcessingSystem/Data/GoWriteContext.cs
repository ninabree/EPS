using ExpenseProcessingSystem.Models.Gbase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseProcessingSystem.Data
{
    public class GoWriteContext : DbContext
    {
        public GoWriteContext(DbContextOptions<GoWriteContext> options)
                : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<tblRequest_DetailsModel> tblRequest_Details { get; set; }
        public DbSet<tblRequest_ItemModel> tblRequest_Item { get; set; }
    }
}
