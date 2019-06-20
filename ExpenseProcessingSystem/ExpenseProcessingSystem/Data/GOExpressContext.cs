using System;
using ExpenseProcessingSystem.Models.Gbase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ExpenseProcessingSystem.Data
{
    public partial class GOExpressContext : DbContext
    {
        public GOExpressContext()
        {
        }

        public GOExpressContext(DbContextOptions<GOExpressContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblCm10> TblCm10 { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Setting.GOExpConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblCm10>(entity =>
            {
                entity.ToTable("tblCM10");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.AutoApproved)
                    .IsRequired()
                    .HasColumnName("AUTO_APPROVED")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('N')");

                entity.Property(e => e.Branchno)
                    .IsRequired()
                    .HasColumnName("BRANCHNO")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.CcyFormat)
                    .IsRequired()
                    .HasColumnName("CCY_FORMAT")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('G')");

                entity.Property(e => e.Comment)
                    .IsRequired()
                    .HasColumnName("COMMENT")
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Datestamp)
                    .HasColumnName("DATESTAMP")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Empno)
                    .IsRequired()
                    .HasColumnName("EMPNO")
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry11ActNo)
                    .IsRequired()
                    .HasColumnName("ENTRY11_ACT_NO")
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry11ActType)
                    .IsRequired()
                    .HasColumnName("ENTRY11_ACT_TYPE")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry11Actcde)
                    .IsRequired()
                    .HasColumnName("ENTRY11_ACTCDE")
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry11AdvcPrnt)
                    .IsRequired()
                    .HasColumnName("ENTRY11_ADVC_PRNT")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry11Amt)
                    .IsRequired()
                    .HasColumnName("ENTRY11_AMT")
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry11Available)
                    .IsRequired()
                    .HasColumnName("ENTRY11_AVAILABLE")
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry11Ccy)
                    .IsRequired()
                    .HasColumnName("ENTRY11_CCY")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry11CheckNo)
                    .IsRequired()
                    .HasColumnName("ENTRY11_CHECK_NO")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry11Cust)
                    .IsRequired()
                    .HasColumnName("ENTRY11_CUST")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry11Details)
                    .IsRequired()
                    .HasColumnName("ENTRY11_DETAILS")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry11Division)
                    .IsRequired()
                    .HasColumnName("ENTRY11_DIVISION")
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry11Entity)
                    .IsRequired()
                    .HasColumnName("ENTRY11_ENTITY")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry11ExchCcy)
                    .IsRequired()
                    .HasColumnName("ENTRY11_EXCH_CCY")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry11ExchRate)
                    .IsRequired()
                    .HasColumnName("ENTRY11_EXCH_RATE")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry11Fund)
                    .IsRequired()
                    .HasColumnName("ENTRY11_FUND")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry11IbfCode)
                    .IsRequired()
                    .HasColumnName("ENTRY11_IBF_CODE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry11InterAmt)
                    .IsRequired()
                    .HasColumnName("ENTRY11_INTER_AMT")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry11InterRate)
                    .IsRequired()
                    .HasColumnName("ENTRY11_INTER_RATE")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry11Type)
                    .IsRequired()
                    .HasColumnName("ENTRY11_TYPE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry12ActNo)
                    .IsRequired()
                    .HasColumnName("ENTRY12_ACT_NO")
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry12ActType)
                    .IsRequired()
                    .HasColumnName("ENTRY12_ACT_TYPE")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry12Actcde)
                    .IsRequired()
                    .HasColumnName("ENTRY12_ACTCDE")
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry12AdvcPrnt)
                    .IsRequired()
                    .HasColumnName("ENTRY12_ADVC_PRNT")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry12Amt)
                    .IsRequired()
                    .HasColumnName("ENTRY12_AMT")
                    .HasMaxLength(18)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry12Available)
                    .IsRequired()
                    .HasColumnName("ENTRY12_AVAILABLE")
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry12Ccy)
                    .IsRequired()
                    .HasColumnName("ENTRY12_CCY")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry12CheckNo)
                    .IsRequired()
                    .HasColumnName("ENTRY12_CHECK_NO")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry12Cust)
                    .IsRequired()
                    .HasColumnName("ENTRY12_CUST")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry12Details)
                    .IsRequired()
                    .HasColumnName("ENTRY12_DETAILS")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry12Division)
                    .IsRequired()
                    .HasColumnName("ENTRY12_DIVISION")
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry12Entity)
                    .IsRequired()
                    .HasColumnName("ENTRY12_ENTITY")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry12ExchCcy)
                    .IsRequired()
                    .HasColumnName("ENTRY12_EXCH_CCY")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry12ExchRate)
                    .IsRequired()
                    .HasColumnName("ENTRY12_EXCH_RATE")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry12Fund)
                    .IsRequired()
                    .HasColumnName("ENTRY12_FUND")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry12IbfCode)
                    .IsRequired()
                    .HasColumnName("ENTRY12_IBF_CODE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry12InterAmt)
                    .IsRequired()
                    .HasColumnName("ENTRY12_INTER_AMT")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry12InterRate)
                    .IsRequired()
                    .HasColumnName("ENTRY12_INTER_RATE")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry12Type)
                    .IsRequired()
                    .HasColumnName("ENTRY12_TYPE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry21ActNo)
                    .IsRequired()
                    .HasColumnName("ENTRY21_ACT_NO")
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry21ActType)
                    .IsRequired()
                    .HasColumnName("ENTRY21_ACT_TYPE")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry21Actcde)
                    .IsRequired()
                    .HasColumnName("ENTRY21_ACTCDE")
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry21AdvcPrnt)
                    .IsRequired()
                    .HasColumnName("ENTRY21_ADVC_PRNT")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry21Amt)
                    .IsRequired()
                    .HasColumnName("ENTRY21_AMT")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry21Available)
                    .IsRequired()
                    .HasColumnName("ENTRY21_AVAILABLE")
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry21Ccy)
                    .IsRequired()
                    .HasColumnName("ENTRY21_CCY")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry21CheckNo)
                    .IsRequired()
                    .HasColumnName("ENTRY21_CHECK_NO")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry21Cust)
                    .IsRequired()
                    .HasColumnName("ENTRY21_CUST")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry21Details)
                    .IsRequired()
                    .HasColumnName("ENTRY21_DETAILS")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry21Division)
                    .IsRequired()
                    .HasColumnName("ENTRY21_DIVISION")
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry21Entity)
                    .IsRequired()
                    .HasColumnName("ENTRY21_ENTITY")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry21ExchCcy)
                    .IsRequired()
                    .HasColumnName("ENTRY21_EXCH_CCY")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry21ExchRate)
                    .IsRequired()
                    .HasColumnName("ENTRY21_EXCH_RATE")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry21Fund)
                    .IsRequired()
                    .HasColumnName("ENTRY21_FUND")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry21IbfCode)
                    .IsRequired()
                    .HasColumnName("ENTRY21_IBF_CODE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry21InterAmt)
                    .IsRequired()
                    .HasColumnName("ENTRY21_INTER_AMT")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry21InterRate)
                    .IsRequired()
                    .HasColumnName("ENTRY21_INTER_RATE")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry21Type)
                    .IsRequired()
                    .HasColumnName("ENTRY21_TYPE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry22ActNo)
                    .IsRequired()
                    .HasColumnName("ENTRY22_ACT_NO")
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry22ActType)
                    .IsRequired()
                    .HasColumnName("ENTRY22_ACT_TYPE")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry22Actcde)
                    .IsRequired()
                    .HasColumnName("ENTRY22_ACTCDE")
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry22AdvcPrnt)
                    .IsRequired()
                    .HasColumnName("ENTRY22_ADVC_PRNT")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry22Amt)
                    .IsRequired()
                    .HasColumnName("ENTRY22_AMT")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry22Available)
                    .IsRequired()
                    .HasColumnName("ENTRY22_AVAILABLE")
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry22Ccy)
                    .IsRequired()
                    .HasColumnName("ENTRY22_CCY")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry22CheckNo)
                    .IsRequired()
                    .HasColumnName("ENTRY22_CHECK_NO")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry22Cust)
                    .IsRequired()
                    .HasColumnName("ENTRY22_CUST")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry22Details)
                    .IsRequired()
                    .HasColumnName("ENTRY22_DETAILS")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry22Division)
                    .IsRequired()
                    .HasColumnName("ENTRY22_DIVISION")
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry22Entity)
                    .IsRequired()
                    .HasColumnName("ENTRY22_ENTITY")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry22ExchCcy)
                    .IsRequired()
                    .HasColumnName("ENTRY22_EXCH_CCY")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry22ExchRate)
                    .IsRequired()
                    .HasColumnName("ENTRY22_EXCH_RATE")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry22Fund)
                    .IsRequired()
                    .HasColumnName("ENTRY22_FUND")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry22IbfCode)
                    .IsRequired()
                    .HasColumnName("ENTRY22_IBF_CODE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry22InterAmt)
                    .IsRequired()
                    .HasColumnName("ENTRY22_INTER_AMT")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry22InterRate)
                    .IsRequired()
                    .HasColumnName("ENTRY22_INTER_RATE")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry22Type)
                    .IsRequired()
                    .HasColumnName("ENTRY22_TYPE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry31ActNo)
                    .IsRequired()
                    .HasColumnName("ENTRY31_ACT_NO")
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry31ActType)
                    .IsRequired()
                    .HasColumnName("ENTRY31_ACT_TYPE")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry31Actcde)
                    .IsRequired()
                    .HasColumnName("ENTRY31_ACTCDE")
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry31AdvcPrnt)
                    .IsRequired()
                    .HasColumnName("ENTRY31_ADVC_PRNT")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry31Amt)
                    .IsRequired()
                    .HasColumnName("ENTRY31_AMT")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry31Available)
                    .IsRequired()
                    .HasColumnName("ENTRY31_AVAILABLE")
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry31Ccy)
                    .IsRequired()
                    .HasColumnName("ENTRY31_CCY")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry31CheckNo)
                    .IsRequired()
                    .HasColumnName("ENTRY31_CHECK_NO")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry31Cust)
                    .IsRequired()
                    .HasColumnName("ENTRY31_CUST")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry31Details)
                    .IsRequired()
                    .HasColumnName("ENTRY31_DETAILS")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry31Division)
                    .IsRequired()
                    .HasColumnName("ENTRY31_DIVISION")
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry31Entity)
                    .IsRequired()
                    .HasColumnName("ENTRY31_ENTITY")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry31ExchCcy)
                    .IsRequired()
                    .HasColumnName("ENTRY31_EXCH_CCY")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry31ExchRate)
                    .IsRequired()
                    .HasColumnName("ENTRY31_EXCH_RATE")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry31Fund)
                    .IsRequired()
                    .HasColumnName("ENTRY31_FUND")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry31IbfCode)
                    .IsRequired()
                    .HasColumnName("ENTRY31_IBF_CODE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry31InterAmt)
                    .IsRequired()
                    .HasColumnName("ENTRY31_INTER_AMT")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry31InterRate)
                    .IsRequired()
                    .HasColumnName("ENTRY31_INTER_RATE")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry31Type)
                    .IsRequired()
                    .HasColumnName("ENTRY31_TYPE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry32ActNo)
                    .IsRequired()
                    .HasColumnName("ENTRY32_ACT_NO")
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry32ActType)
                    .IsRequired()
                    .HasColumnName("ENTRY32_ACT_TYPE")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry32Actcde)
                    .IsRequired()
                    .HasColumnName("ENTRY32_ACTCDE")
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry32AdvcPrnt)
                    .IsRequired()
                    .HasColumnName("ENTRY32_ADVC_PRNT")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry32Amt)
                    .IsRequired()
                    .HasColumnName("ENTRY32_AMT")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry32Available)
                    .IsRequired()
                    .HasColumnName("ENTRY32_AVAILABLE")
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry32Ccy)
                    .IsRequired()
                    .HasColumnName("ENTRY32_CCY")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry32CheckNo)
                    .IsRequired()
                    .HasColumnName("ENTRY32_CHECK_NO")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry32Cust)
                    .IsRequired()
                    .HasColumnName("ENTRY32_CUST")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry32Details)
                    .IsRequired()
                    .HasColumnName("ENTRY32_DETAILS")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry32Division)
                    .IsRequired()
                    .HasColumnName("ENTRY32_DIVISION")
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry32Entity)
                    .IsRequired()
                    .HasColumnName("ENTRY32_ENTITY")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry32ExchCcy)
                    .IsRequired()
                    .HasColumnName("ENTRY32_EXCH_CCY")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry32ExchRate)
                    .IsRequired()
                    .HasColumnName("ENTRY32_EXCH_RATE")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry32Fund)
                    .IsRequired()
                    .HasColumnName("ENTRY32_FUND")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry32IbfCode)
                    .IsRequired()
                    .HasColumnName("ENTRY32_IBF_CODE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry32InterAmt)
                    .IsRequired()
                    .HasColumnName("ENTRY32_INTER_AMT")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry32InterRate)
                    .IsRequired()
                    .HasColumnName("ENTRY32_INTER_RATE")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry32Type)
                    .IsRequired()
                    .HasColumnName("ENTRY32_TYPE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry41ActNo)
                    .IsRequired()
                    .HasColumnName("ENTRY41_ACT_NO")
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry41ActType)
                    .IsRequired()
                    .HasColumnName("ENTRY41_ACT_TYPE")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry41Actcde)
                    .IsRequired()
                    .HasColumnName("ENTRY41_ACTCDE")
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry41AdvcPrnt)
                    .IsRequired()
                    .HasColumnName("ENTRY41_ADVC_PRNT")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry41Amt)
                    .IsRequired()
                    .HasColumnName("ENTRY41_AMT")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry41Available)
                    .IsRequired()
                    .HasColumnName("ENTRY41_AVAILABLE")
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry41Ccy)
                    .IsRequired()
                    .HasColumnName("ENTRY41_CCY")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry41CheckNo)
                    .IsRequired()
                    .HasColumnName("ENTRY41_CHECK_NO")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry41Cust)
                    .IsRequired()
                    .HasColumnName("ENTRY41_CUST")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry41Details)
                    .IsRequired()
                    .HasColumnName("ENTRY41_DETAILS")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry41Division)
                    .IsRequired()
                    .HasColumnName("ENTRY41_DIVISION")
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry41Entity)
                    .IsRequired()
                    .HasColumnName("ENTRY41_ENTITY")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry41ExchCcy)
                    .IsRequired()
                    .HasColumnName("ENTRY41_EXCH_CCY")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry41ExchRate)
                    .IsRequired()
                    .HasColumnName("ENTRY41_EXCH_RATE")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry41Fund)
                    .IsRequired()
                    .HasColumnName("ENTRY41_FUND")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry41IbfCode)
                    .IsRequired()
                    .HasColumnName("ENTRY41_IBF_CODE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry41InterAmt)
                    .IsRequired()
                    .HasColumnName("ENTRY41_INTER_AMT")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry41InterRate)
                    .IsRequired()
                    .HasColumnName("ENTRY41_INTER_RATE")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry41Type)
                    .IsRequired()
                    .HasColumnName("ENTRY41_TYPE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry42ActNo)
                    .IsRequired()
                    .HasColumnName("ENTRY42_ACT_NO")
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry42ActType)
                    .IsRequired()
                    .HasColumnName("ENTRY42_ACT_TYPE")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry42Actcde)
                    .IsRequired()
                    .HasColumnName("ENTRY42_ACTCDE")
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry42AdvcPrnt)
                    .IsRequired()
                    .HasColumnName("ENTRY42_ADVC_PRNT")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry42Amt)
                    .IsRequired()
                    .HasColumnName("ENTRY42_AMT")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry42Available)
                    .IsRequired()
                    .HasColumnName("ENTRY42_AVAILABLE")
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry42Ccy)
                    .IsRequired()
                    .HasColumnName("ENTRY42_CCY")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry42CheckNo)
                    .IsRequired()
                    .HasColumnName("ENTRY42_CHECK_NO")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry42Cust)
                    .IsRequired()
                    .HasColumnName("ENTRY42_CUST")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry42Details)
                    .IsRequired()
                    .HasColumnName("ENTRY42_DETAILS")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry42Division)
                    .IsRequired()
                    .HasColumnName("ENTRY42_DIVISION")
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry42Entity)
                    .IsRequired()
                    .HasColumnName("ENTRY42_ENTITY")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry42ExchCcy)
                    .IsRequired()
                    .HasColumnName("ENTRY42_EXCH_CCY")
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry42ExchRate)
                    .IsRequired()
                    .HasColumnName("ENTRY42_EXCH_RATE")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry42Fund)
                    .IsRequired()
                    .HasColumnName("ENTRY42_FUND")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry42IbfCode)
                    .IsRequired()
                    .HasColumnName("ENTRY42_IBF_CODE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry42InterAmt)
                    .IsRequired()
                    .HasColumnName("ENTRY42_INTER_AMT")
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry42InterRate)
                    .IsRequired()
                    .HasColumnName("ENTRY42_INTER_RATE")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Entry42Type)
                    .IsRequired()
                    .HasColumnName("ENTRY42_TYPE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Groupcode)
                    .IsRequired()
                    .HasColumnName("GROUPCODE")
                    .HasMaxLength(22)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.MakerEmpno)
                    .IsRequired()
                    .HasColumnName("MAKER_EMPNO")
                    .HasMaxLength(8)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Memo)
                    .IsRequired()
                    .HasColumnName("MEMO")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.OpeBranch)
                    .IsRequired()
                    .HasColumnName("OPE_BRANCH")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.OpeKind)
                    .IsRequired()
                    .HasColumnName("OPE_KIND")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('1')");

                entity.Property(e => e.Recstatus)
                    .IsRequired()
                    .HasColumnName("RECSTATUS")
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('READY')");

                entity.Property(e => e.ReferenceNo)
                    .IsRequired()
                    .HasColumnName("REFERENCE_NO")
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ReferenceType)
                    .IsRequired()
                    .HasColumnName("REFERENCE_TYPE")
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Remarks)
                    .IsRequired()
                    .HasColumnName("REMARKS")
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.SchemeNo)
                    .IsRequired()
                    .HasColumnName("SCHEME_NO")
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Section)
                    .IsRequired()
                    .HasColumnName("SECTION")
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.SystemName)
                    .IsRequired()
                    .HasColumnName("SYSTEM_NAME")
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Timerespond)
                    .HasColumnName("TIMERESPOND")
                    .HasColumnType("datetime");

                entity.Property(e => e.Timesent)
                    .HasColumnName("TIMESENT")
                    .HasColumnType("datetime");

                entity.Property(e => e.Transno)
                    .IsRequired()
                    .HasColumnName("TRANSNO")
                    .HasMaxLength(5)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ValueDate)
                    .IsRequired()
                    .HasColumnName("VALUE_DATE")
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.WarningOverride)
                    .IsRequired()
                    .HasColumnName("WARNING_OVERRIDE")
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('N')");

                entity.Property(e => e.Xmlmsg)
                    .HasColumnName("XMLMSG")
                    .HasColumnType("text")
                    .HasDefaultValueSql("('')");
            });
        }
    }
}
