using ExpenseProcessingSystem.Models;
using ExpenseProcessingSystem.Models.Pending;
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
        public DbSet<UserModel> User { get; set; }
        public DbSet<DMDeptModel> DMDept { get; set; }
        public DbSet<DMVendorModel> DMVendor { get; set; }
        public DbSet<DMCheckModel> DMCheck { get; set; }
        public DbSet<DMAccountModel> DMAccount { get; set; }
        public DbSet<DMVATModel> DMVAT { get; set; }
        public DbSet<DMFBTModel> DMFBT { get; set; }
        public DbSet<DMTRModel> DMTR { get; set; }
        public DbSet<DMCurrencyModel> DMCurrency { get; set; }
        public DbSet<BudgetModel> Budget { get; set; }
        public DbSet<DMEmpModel> DMEmp { get; set; }
        public DbSet<DMCustModel> DMCust { get; set; }
        public DbSet<DMBIRCertSignModel> DMBCS { get; set; }
        public DbSet<SystemMessageModel> SystemMessageModels { get; set; }
        public DbSet<DMVendorTRVATModel> DMVendorTRVAT { get; set; }
        public DbSet<DMAccountGroupModel> DMAccountGroup { get; set; }

        //PENDING ENTRY TABLES
        public DbSet<DMVendorModel_Pending> DMVendor_Pending { get; set; }
        public DbSet<DMDeptModel_Pending> DMDept_Pending { get; set; }
        public DbSet<DMCheckModel_Pending> DMCheck_Pending { get; set; }
        public DbSet<DMAccountModel_Pending> DMAccount_Pending { get; set; }
        public DbSet<DMVATModel_Pending> DMVAT_Pending { get; set; }
        public DbSet<DMFBTModel_Pending> DMFBT_Pending { get; set; }
        public DbSet<DMTRModel_Pending> DMTR_Pending { get; set; }
        public DbSet<DMCurrencyModel_Pending> DMCurrency_Pending { get; set; }
        public DbSet<DMEmpModel_Pending> DMEmp_Pending { get; set; }
        public DbSet<DMCustModel_Pending> DMCust_Pending { get; set; }
        public DbSet<DMBIRCertSignModel_Pending> DMBCS_Pending { get; set; }
        public DbSet<DMVendorTRVATModel_Pending> DMVendorTRVAT_Pending { get; set; }
        public DbSet<DMAccountGroupModel_Pending> DMAccountGroup_Pending { get; set; }

        //HOME
        public DbSet<HomeNotifModel> HomeNotif { get; set; }
        //FILES
        public DbSet<FileLocationModel> FileLocation { get; set; }
        //STATUS
        public DbSet<StatusListModel> StatusList { get; set; }

        //EXPENSE ENTRY TABLES
        public DbSet<ExpenseEntryModel> ExpenseEntry { get; set; }
        public DbSet<ExpenseEntryDetailModel> ExpenseEntryDetails { get; set; }
        public DbSet<ExpenseEntryAmortizationModel> ExpenseEntryAmortizations { get; set; }
        public DbSet<ExpenseEntryInterEntityModel> ExpenseEntryInterEntity { get; set; }
        public DbSet<ExpenseEntryGbaseDtl> ExpenseEntryGbaseDtls { get; set; }
        public DbSet<ExpenseEntryCashBreakdownModel> ExpenseEntryCashBreakdown { get; set; }
        //EXPENSE ENTRY TABLES NON CASH
        public DbSet<ExpenseEntryNCModel> ExpenseEntryNonCash { get; set; }
        public DbSet<ExpenseEntryNCDtlModel> ExpenseEntryNonCashDetails { get; set; }
        public DbSet<ExpenseEntryNCDtlAccModel> ExpenseEntryNonCashDetailAccounts { get; set; }

        //LIQUIDATION TABLES
        public DbSet<LiquidationEntryDetailModel> LiquidationEntryDetails { get; set; }
        public DbSet<LiquidationCashBreakdownModel> LiquidationCashBreakdown { get; set; }

        //GOExpress table history
        public DbSet<GOExpressHistModel> GOExpressHist { get; set; }


    }
}
