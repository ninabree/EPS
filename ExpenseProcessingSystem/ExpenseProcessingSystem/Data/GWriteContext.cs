using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ExpenseProcessingSystem.Data
{
    public partial class GWriteContext : DbContext
    {
        public GWriteContext()
        {
        }

        public GWriteContext(DbContextOptions<GWriteContext> options)
            : base(options)
        {
        }

        // Unable to generate entity type for table 'dbo.tblRequest_Details'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.tblRequest_Item'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Setting.GwriteConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {}
    }
}
