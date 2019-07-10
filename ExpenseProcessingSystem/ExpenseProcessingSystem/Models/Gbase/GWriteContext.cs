using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ExpenseProcessingSystem.Models.Gbase
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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=J-PCS008\\SQLEXPRESS;Persist Security Info=True;Password=emiAdmin;User ID=sa;Initial Catalog=G-Write");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {}
    }
}
