using System;
using ExpenseProcessingSystem.Models.Gbase;
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

        public virtual DbSet<TblRequestDetails> TblRequestDetails { get; set; }
        public virtual DbSet<TblRequestItem> TblRequestItem { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Setting.GwriteConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblRequestDetails>(entity =>
            {
                entity.HasKey(e => e.RequestId);

                entity.ToTable("tblRequest_Details");

                entity.Property(e => e.RequestId)
                    .HasColumnName("RequestID")
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Priority).HasDefaultValueSql("((2))");

                entity.Property(e => e.RacfId)
                    .IsRequired()
                    .HasColumnName("RacfID")
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.RacfPassword)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.RequestCreated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ReturnMessage)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Scripting')");

                entity.Property(e => e.StatusDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.SystemAbbr)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");
            });

            modelBuilder.Entity<TblRequestItem>(entity =>
            {
                entity.HasKey(e => e.ItemId);

                entity.ToTable("tblRequest_Item");

                entity.Property(e => e.ItemId)
                    .HasColumnName("ItemID")
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Command)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.RequestId)
                    .HasColumnName("RequestID")
                    .HasColumnType("numeric(18, 0)");

                entity.Property(e => e.ScreenCapture).HasColumnType("text");
            });
        }
    }
}
