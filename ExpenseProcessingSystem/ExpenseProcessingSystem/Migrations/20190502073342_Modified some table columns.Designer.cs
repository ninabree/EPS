﻿// <auto-generated />
using System;
using ExpenseProcessingSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ExpenseProcessingSystem.Migrations
{
    [DbContext(typeof(EPSDbContext))]
    [Migration("20190502073342_Modified some table columns")]
    partial class Modifiedsometablecolumns
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ExpenseProcessingSystem.Models.BudgetModel", b =>
                {
                    b.Property<int>("Budget_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Budget_Acc_ID");

                    b.Property<string>("Budget_Amount");

                    b.Property<int>("Budget_Approver_ID");

                    b.Property<DateTime>("Budget_Created_Date");

                    b.Property<int>("Budget_Creator_ID");

                    b.Property<DateTime>("Budget_Last_Updated");

                    b.Property<string>("Budget_Status");

                    b.Property<bool>("Budget_isDeleted");

                    b.HasKey("Budget_ID");

                    b.ToTable("Budget");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.DMAccountModel", b =>
                {
                    b.Property<int>("Account_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Account_Approver_ID");

                    b.Property<string>("Account_Code");

                    b.Property<DateTime>("Account_Created_Date");

                    b.Property<int>("Account_Creator_ID");

                    b.Property<string>("Account_Cust");

                    b.Property<string>("Account_Div");

                    b.Property<bool>("Account_Fund");

                    b.Property<DateTime>("Account_Last_Updated");

                    b.Property<int>("Account_MasterID");

                    b.Property<string>("Account_Name");

                    b.Property<string>("Account_No");

                    b.Property<string>("Account_Status");

                    b.Property<bool>("Account_isActive");

                    b.Property<bool>("Account_isDeleted");

                    b.HasKey("Account_ID");

                    b.ToTable("DMAccount");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.DMAccountModel_Pending", b =>
                {
                    b.Property<int>("Pending_Account_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Pending_Account_Approver_ID");

                    b.Property<string>("Pending_Account_Code");

                    b.Property<int>("Pending_Account_Creator_ID");

                    b.Property<string>("Pending_Account_Cust");

                    b.Property<string>("Pending_Account_Div");

                    b.Property<DateTime>("Pending_Account_Filed_Date");

                    b.Property<bool>("Pending_Account_Fund");

                    b.Property<int>("Pending_Account_MasterID");

                    b.Property<string>("Pending_Account_Name");

                    b.Property<string>("Pending_Account_No");

                    b.Property<string>("Pending_Account_Status");

                    b.Property<bool>("Pending_Account_isActive");

                    b.Property<bool>("Pending_Account_isDeleted");

                    b.HasKey("Pending_Account_ID");

                    b.ToTable("DMAccount_Pending");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.DMBIRCertSignModel", b =>
                {
                    b.Property<int>("BCS_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BCS_Approver_ID");

                    b.Property<DateTime>("BCS_Created_Date");

                    b.Property<int>("BCS_Creator_ID");

                    b.Property<DateTime>("BCS_Last_Updated");

                    b.Property<int>("BCS_MasterID");

                    b.Property<string>("BCS_Name");

                    b.Property<string>("BCS_Position");

                    b.Property<string>("BCS_Signatures");

                    b.Property<string>("BCS_Status");

                    b.Property<int>("BCS_TIN");

                    b.Property<bool>("BCS_isActive");

                    b.Property<bool>("BCS_isDeleted");

                    b.HasKey("BCS_ID");

                    b.ToTable("DMBCS");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.DMCheckModel", b =>
                {
                    b.Property<int>("Check_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Check_Approver_ID");

                    b.Property<string>("Check_Bank_Info");

                    b.Property<DateTime>("Check_Created_Date");

                    b.Property<int>("Check_Creator_ID");

                    b.Property<DateTime>("Check_Input_Date");

                    b.Property<DateTime>("Check_Last_Updated");

                    b.Property<int>("Check_MasterID");

                    b.Property<string>("Check_Series_From");

                    b.Property<string>("Check_Series_To");

                    b.Property<string>("Check_Status");

                    b.Property<bool>("Check_isActive");

                    b.Property<bool>("Check_isDeleted");

                    b.HasKey("Check_ID");

                    b.ToTable("DMCheck");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.DMCheckModel_Pending", b =>
                {
                    b.Property<int>("Pending_Check_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Pending_Check_Approver_ID");

                    b.Property<string>("Pending_Check_Bank_Info");

                    b.Property<int>("Pending_Check_Creator_ID");

                    b.Property<DateTime>("Pending_Check_Filed_Date");

                    b.Property<DateTime>("Pending_Check_Input_Date");

                    b.Property<int>("Pending_Check_MasterID");

                    b.Property<string>("Pending_Check_Series_From");

                    b.Property<string>("Pending_Check_Series_To");

                    b.Property<string>("Pending_Check_Status");

                    b.Property<bool>("Pending_Check_isActive");

                    b.Property<bool>("Pending_Check_isDeleted");

                    b.HasKey("Pending_Check_ID");

                    b.ToTable("DMCheck_Pending");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.DMCurrencyModel", b =>
                {
                    b.Property<int>("Curr_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Curr_Approver_ID");

                    b.Property<string>("Curr_CCY_ABBR");

                    b.Property<DateTime>("Curr_Created_Date");

                    b.Property<int>("Curr_Creator_ID");

                    b.Property<DateTime>("Curr_Last_Updated");

                    b.Property<int>("Curr_MasterID");

                    b.Property<string>("Curr_Name");

                    b.Property<string>("Curr_Status");

                    b.Property<bool>("Curr_isActive");

                    b.Property<bool>("Curr_isDeleted");

                    b.HasKey("Curr_ID");

                    b.ToTable("DMCurrency");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.DMCustModel", b =>
                {
                    b.Property<int>("Cust_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Cust_Abbr");

                    b.Property<int>("Cust_Approver_ID");

                    b.Property<DateTime>("Cust_Created_Date");

                    b.Property<int>("Cust_Creator_ID");

                    b.Property<DateTime>("Cust_Last_Updated");

                    b.Property<int>("Cust_MasterID");

                    b.Property<string>("Cust_Name");

                    b.Property<string>("Cust_No");

                    b.Property<string>("Cust_Status");

                    b.Property<bool>("Cust_isActive");

                    b.Property<bool>("Cust_isDeleted");

                    b.HasKey("Cust_ID");

                    b.ToTable("DMCust");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.DMDeptModel", b =>
                {
                    b.Property<int>("Dept_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Dept_Approver_ID");

                    b.Property<string>("Dept_Budget_Unit");

                    b.Property<string>("Dept_Code");

                    b.Property<DateTime>("Dept_Created_Date");

                    b.Property<int>("Dept_Creator_ID");

                    b.Property<DateTime>("Dept_Last_Updated");

                    b.Property<int>("Dept_MasterID");

                    b.Property<string>("Dept_Name");

                    b.Property<string>("Dept_Status");

                    b.Property<bool>("Dept_isActive");

                    b.Property<bool>("Dept_isDeleted");

                    b.HasKey("Dept_ID");

                    b.ToTable("DMDept");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.DMDeptModel_Pending", b =>
                {
                    b.Property<int>("Pending_Dept_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Pending_Dept_Approver_ID");

                    b.Property<string>("Pending_Dept_Budget_Unit");

                    b.Property<string>("Pending_Dept_Code");

                    b.Property<int>("Pending_Dept_Creator_ID");

                    b.Property<DateTime>("Pending_Dept_Filed_Date");

                    b.Property<int>("Pending_Dept_MasterID");

                    b.Property<string>("Pending_Dept_Name");

                    b.Property<string>("Pending_Dept_Status");

                    b.Property<bool>("Pending_Dept_isActive");

                    b.Property<bool>("Pending_Dept_isDeleted");

                    b.HasKey("Pending_Dept_ID");

                    b.ToTable("DMDept_Pending");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.DMEmpModel", b =>
                {
                    b.Property<int>("Emp_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Emp_Acc_No");

                    b.Property<int>("Emp_Approver_ID");

                    b.Property<DateTime>("Emp_Created_Date");

                    b.Property<int>("Emp_Creator_ID");

                    b.Property<DateTime>("Emp_Last_Updated");

                    b.Property<int>("Emp_MasterID");

                    b.Property<string>("Emp_Name");

                    b.Property<string>("Emp_Status");

                    b.Property<string>("Emp_Type");

                    b.Property<bool>("Emp_isActive");

                    b.Property<bool>("Emp_isDeleted");

                    b.HasKey("Emp_ID");

                    b.ToTable("DMEmp");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.DMFBTModel", b =>
                {
                    b.Property<int>("FBT_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FBT_Account");

                    b.Property<int>("FBT_Approver_ID");

                    b.Property<DateTime>("FBT_Created_Date");

                    b.Property<int>("FBT_Creator_ID");

                    b.Property<string>("FBT_Formula");

                    b.Property<DateTime>("FBT_Last_Updated");

                    b.Property<int>("FBT_MasterID");

                    b.Property<string>("FBT_Name");

                    b.Property<string>("FBT_Status");

                    b.Property<int>("FBT_Tax_Rate");

                    b.Property<bool>("FBT_isActive");

                    b.Property<bool>("FBT_isDeleted");

                    b.HasKey("FBT_ID");

                    b.ToTable("DMFBT");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.DMNonCashCategoryModel", b =>
                {
                    b.Property<int>("NCC_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("NCC_Approver_ID");

                    b.Property<DateTime>("NCC_Created_Date");

                    b.Property<int>("NCC_Creator_ID");

                    b.Property<DateTime>("NCC_Last_Updated");

                    b.Property<int>("NCC_MasterID");

                    b.Property<string>("NCC_Name");

                    b.Property<string>("NCC_Pro_Forma");

                    b.Property<string>("NCC_Status");

                    b.Property<bool>("NCC_isActive");

                    b.Property<bool>("NCC_isDeleted");

                    b.HasKey("NCC_ID");

                    b.ToTable("DMNCC");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.DMTRModel", b =>
                {
                    b.Property<int>("TR_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("TR_ATC");

                    b.Property<int>("TR_Approver_ID");

                    b.Property<DateTime>("TR_Created_Date");

                    b.Property<int>("TR_Creator_ID");

                    b.Property<DateTime>("TR_Last_Updated");

                    b.Property<int>("TR_MasterID");

                    b.Property<string>("TR_Nature");

                    b.Property<string>("TR_Status");

                    b.Property<int>("TR_Tax_Rate");

                    b.Property<string>("TR_WT_Title");

                    b.Property<bool>("TR_isActive");

                    b.Property<bool>("TR_isDeleted");

                    b.HasKey("TR_ID");

                    b.ToTable("DMTR");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.DMVATModel", b =>
                {
                    b.Property<int>("VAT_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("VAT_Approver_ID");

                    b.Property<DateTime>("VAT_Created_Date");

                    b.Property<int>("VAT_Creator_ID");

                    b.Property<DateTime>("VAT_Last_Updated");

                    b.Property<int>("VAT_MasterID");

                    b.Property<string>("VAT_Name");

                    b.Property<string>("VAT_Rate");

                    b.Property<string>("VAT_Status");

                    b.Property<bool>("VAT_isActive");

                    b.Property<bool>("VAT_isDeleted");

                    b.HasKey("VAT_ID");

                    b.ToTable("DMVAT");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.DMVendorModel", b =>
                {
                    b.Property<int>("Vendor_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Vendor_Address");

                    b.Property<int>("Vendor_Approver_ID");

                    b.Property<DateTime>("Vendor_Created_Date");

                    b.Property<int>("Vendor_Creator_ID");

                    b.Property<DateTime>("Vendor_Last_Updated");

                    b.Property<int>("Vendor_MasterID");

                    b.Property<string>("Vendor_Name");

                    b.Property<string>("Vendor_Status");

                    b.Property<string>("Vendor_TIN");

                    b.Property<bool>("Vendor_isActive");

                    b.Property<bool>("Vendor_isDeleted");

                    b.HasKey("Vendor_ID");

                    b.ToTable("DMVendor");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.DMVendorModel_Pending", b =>
                {
                    b.Property<int>("Pending_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Pending_Vendor_Address");

                    b.Property<int>("Pending_Vendor_Approver_ID");

                    b.Property<int>("Pending_Vendor_Creator_ID");

                    b.Property<DateTime>("Pending_Vendor_Filed_Date");

                    b.Property<bool>("Pending_Vendor_IsDeleted");

                    b.Property<int>("Pending_Vendor_MasterID");

                    b.Property<string>("Pending_Vendor_Name");

                    b.Property<string>("Pending_Vendor_Status");

                    b.Property<string>("Pending_Vendor_TIN");

                    b.Property<bool>("Pending_Vendor_isActive");

                    b.HasKey("Pending_ID");

                    b.ToTable("DMVendor_Pending");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.ExpenseEntryAmortizationModel", b =>
                {
                    b.Property<int>("Amor_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<float>("Amor_Price");

                    b.Property<DateTime>("Amor_Sched_Date");

                    b.Property<int?>("ExpenseEntryDetailModelExpDtl_ID");

                    b.HasKey("Amor_ID");

                    b.HasIndex("ExpenseEntryDetailModelExpDtl_ID");

                    b.ToTable("ExpenseEntryAmortizations");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.ExpenseEntryDetailModel", b =>
                {
                    b.Property<int>("ExpDtl_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ExpDtl_Account");

                    b.Property<int>("ExpDtl_Amor_Day");

                    b.Property<int>("ExpDtl_Amor_Duration");

                    b.Property<int>("ExpDtl_Amor_Month");

                    b.Property<int>("ExpDtl_Ccy");

                    b.Property<float>("ExpDtl_Credit_Cash");

                    b.Property<float>("ExpDtl_Credit_Ewt");

                    b.Property<float>("ExpDtl_Debit");

                    b.Property<int>("ExpDtl_Dept");

                    b.Property<int>("ExpDtl_Ewt");

                    b.Property<bool>("ExpDtl_Fbt");

                    b.Property<string>("ExpDtl_Gbase_Remarks");

                    b.Property<float>("ExpDtl_Vat");

                    b.Property<int?>("ExpenseEntryModelExpense_ID");

                    b.HasKey("ExpDtl_ID");

                    b.HasIndex("ExpenseEntryModelExpense_ID");

                    b.ToTable("ExpenseEntryDetails");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.ExpenseEntryGbaseDtl", b =>
                {
                    b.Property<int>("GbaseDtl_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ExpenseEntryDetailModelExpDtl_ID");

                    b.Property<float>("GbaseDtl_Amount");

                    b.Property<string>("GbaseDtl_Description");

                    b.Property<string>("GbaseDtl_Document_Type");

                    b.Property<string>("GbaseDtl_InvoiceNo");

                    b.HasKey("GbaseDtl_ID");

                    b.HasIndex("ExpenseEntryDetailModelExpDtl_ID");

                    b.ToTable("ExpenseEntryGbaseDtls");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.ExpenseEntryModel", b =>
                {
                    b.Property<int>("Expense_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Expense_Approver");

                    b.Property<int>("Expense_CheckId");

                    b.Property<string>("Expense_CheckNo");

                    b.Property<DateTime>("Expense_Created_Date");

                    b.Property<int>("Expense_Creator_ID");

                    b.Property<float>("Expense_Credit_Total");

                    b.Property<DateTime>("Expense_Date");

                    b.Property<float>("Expense_Debit_Total");

                    b.Property<DateTime>("Expense_Last_Updated");

                    b.Property<string>("Expense_Number");

                    b.Property<int>("Expense_Payee");

                    b.Property<int>("Expense_Status");

                    b.Property<int>("Expense_Verifier");

                    b.Property<bool>("Expense_isDeleted");

                    b.HasKey("Expense_ID");

                    b.ToTable("ExpenseEntry");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.FileLocationModel", b =>
                {
                    b.Property<int>("FL_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FL_Location");

                    b.Property<string>("FL_Type");

                    b.HasKey("FL_ID");

                    b.ToTable("FileLocation");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.NotifModel", b =>
                {
                    b.Property<int>("Notif_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Notif_Application_ID");

                    b.Property<int>("Notif_Apprvr_ID");

                    b.Property<DateTime>("Notif_Date");

                    b.Property<string>("Notif_Link_Address");

                    b.Property<string>("Notif_Message");

                    b.Property<bool>("Notif_Status");

                    b.Property<string>("Notif_Type_Screen");

                    b.Property<string>("Notif_Type_Status");

                    b.Property<int>("Notif_User_ID");

                    b.HasKey("Notif_ID");

                    b.ToTable("Notif");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.Pending.DMBIRCertSignModel_Pending", b =>
                {
                    b.Property<int>("Pending_BCS_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Pending_BCS_Approver_ID");

                    b.Property<int>("Pending_BCS_Creator_ID");

                    b.Property<DateTime>("Pending_BCS_Filed_Date");

                    b.Property<int>("Pending_BCS_MasterID");

                    b.Property<string>("Pending_BCS_Name");

                    b.Property<string>("Pending_BCS_Position");

                    b.Property<string>("Pending_BCS_Signatures");

                    b.Property<string>("Pending_BCS_Status");

                    b.Property<int>("Pending_BCS_TIN");

                    b.Property<bool>("Pending_BCS_isActive");

                    b.Property<bool>("Pending_BCS_isDeleted");

                    b.HasKey("Pending_BCS_ID");

                    b.ToTable("DMBCS_Pending");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.Pending.DMCurrencyModel_Pending", b =>
                {
                    b.Property<int>("Pending_Curr_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Pending_Curr_Approver_ID");

                    b.Property<string>("Pending_Curr_CCY_ABBR");

                    b.Property<int>("Pending_Curr_Creator_ID");

                    b.Property<DateTime>("Pending_Curr_Filed_Date");

                    b.Property<int>("Pending_Curr_MasterID");

                    b.Property<string>("Pending_Curr_Name");

                    b.Property<string>("Pending_Curr_Status");

                    b.Property<bool>("Pending_Curr_isActive");

                    b.Property<bool>("Pending_Curr_isDeleted");

                    b.HasKey("Pending_Curr_ID");

                    b.ToTable("DMCurrency_Pending");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.Pending.DMCustModel_Pending", b =>
                {
                    b.Property<int>("Pending_Cust_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Pending_Cust_Abbr");

                    b.Property<int>("Pending_Cust_Approver_ID");

                    b.Property<int>("Pending_Cust_Creator_ID");

                    b.Property<DateTime>("Pending_Cust_Filed_Date");

                    b.Property<int>("Pending_Cust_MasterID");

                    b.Property<string>("Pending_Cust_Name");

                    b.Property<string>("Pending_Cust_No");

                    b.Property<string>("Pending_Cust_Status");

                    b.Property<bool>("Pending_Cust_isActive");

                    b.Property<bool>("Pending_Cust_isDeleted");

                    b.HasKey("Pending_Cust_ID");

                    b.ToTable("DMCust_Pending");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.Pending.DMEmpModel_Pending", b =>
                {
                    b.Property<int>("Pending_Emp_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Pending_Emp_Acc_No");

                    b.Property<int>("Pending_Emp_Approver_ID");

                    b.Property<int>("Pending_Emp_Creator_ID");

                    b.Property<DateTime>("Pending_Emp_Filed_Date");

                    b.Property<int>("Pending_Emp_MasterID");

                    b.Property<string>("Pending_Emp_Name");

                    b.Property<string>("Pending_Emp_Status");

                    b.Property<string>("Pending_Emp_Type");

                    b.Property<bool>("Pending_Emp_isActive");

                    b.Property<bool>("Pending_Emp_isDeleted");

                    b.HasKey("Pending_Emp_ID");

                    b.ToTable("DMEmp_Pending");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.Pending.DMFBTModel_Pending", b =>
                {
                    b.Property<int>("Pending_FBT_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Pending_FBT_Account");

                    b.Property<int>("Pending_FBT_Approver_ID");

                    b.Property<int>("Pending_FBT_Creator_ID");

                    b.Property<DateTime>("Pending_FBT_Filed_Date");

                    b.Property<string>("Pending_FBT_Formula");

                    b.Property<int>("Pending_FBT_MasterID");

                    b.Property<string>("Pending_FBT_Name");

                    b.Property<string>("Pending_FBT_Status");

                    b.Property<int>("Pending_FBT_Tax_Rate");

                    b.Property<bool>("Pending_FBT_isActive");

                    b.Property<bool>("Pending_FBT_isDeleted");

                    b.HasKey("Pending_FBT_ID");

                    b.ToTable("DMFBT_Pending");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.Pending.DMNonCashCategoryModel_Pending", b =>
                {
                    b.Property<int>("Pending_NCC_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Pending_NCC_Approver_ID");

                    b.Property<int>("Pending_NCC_Creator_ID");

                    b.Property<DateTime>("Pending_NCC_Filed_Date");

                    b.Property<int>("Pending_NCC_MasterID");

                    b.Property<string>("Pending_NCC_Name");

                    b.Property<string>("Pending_NCC_Pro_Forma");

                    b.Property<string>("Pending_NCC_Status");

                    b.Property<bool>("Pending_NCC_isActive");

                    b.Property<bool>("Pending_NCC_isDeleted");

                    b.HasKey("Pending_NCC_ID");

                    b.ToTable("DMNCC_Pending");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.Pending.DMTRModel_Pending", b =>
                {
                    b.Property<int>("Pending_TR_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Pending_TR_ATC");

                    b.Property<int>("Pending_TR_Approver_ID");

                    b.Property<int>("Pending_TR_Creator_ID");

                    b.Property<DateTime>("Pending_TR_Filed_Date");

                    b.Property<int>("Pending_TR_MasterID");

                    b.Property<string>("Pending_TR_Nature");

                    b.Property<string>("Pending_TR_Status");

                    b.Property<int>("Pending_TR_Tax_Rate");

                    b.Property<string>("Pending_TR_WT_Title");

                    b.Property<bool>("Pending_TR_isActive");

                    b.Property<bool>("Pending_TR_isDeleted");

                    b.HasKey("Pending_TR_ID");

                    b.ToTable("DMTR_Pending");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.Pending.DMVATModel_Pending", b =>
                {
                    b.Property<int>("Pending_VAT_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Pending_VAT_Approver_ID");

                    b.Property<int>("Pending_VAT_Creator_ID");

                    b.Property<DateTime>("Pending_VAT_Filed_Date");

                    b.Property<int>("Pending_VAT_MasterID");

                    b.Property<string>("Pending_VAT_Name");

                    b.Property<string>("Pending_VAT_Rate");

                    b.Property<string>("Pending_VAT_Status");

                    b.Property<bool>("Pending_VAT_isActive");

                    b.Property<bool>("Pending_VAT_isDeleted");

                    b.HasKey("Pending_VAT_ID");

                    b.ToTable("DMVAT_Pending");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.StatusListModel", b =>
                {
                    b.Property<int>("Status_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Status_Name");

                    b.HasKey("Status_ID");

                    b.ToTable("StatusList");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.SystemMessageModel", b =>
                {
                    b.Property<string>("Msg_Code")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Msg_Content");

                    b.Property<string>("Msg_Type");

                    b.HasKey("Msg_Code");

                    b.ToTable("SystemMessageModels");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.UserModel", b =>
                {
                    b.Property<int>("User_ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("User_Approver_ID");

                    b.Property<string>("User_Comment");

                    b.Property<DateTime>("User_Created_Date");

                    b.Property<int>("User_Creator_ID");

                    b.Property<int>("User_DeptID");

                    b.Property<string>("User_Email");

                    b.Property<string>("User_FName");

                    b.Property<bool>("User_InUse");

                    b.Property<string>("User_LName");

                    b.Property<DateTime>("User_Last_Updated");

                    b.Property<string>("User_Password");

                    b.Property<string>("User_Role");

                    b.Property<string>("User_Status");

                    b.Property<string>("User_UserName");

                    b.HasKey("User_ID");

                    b.ToTable("User");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.ExpenseEntryAmortizationModel", b =>
                {
                    b.HasOne("ExpenseProcessingSystem.Models.ExpenseEntryDetailModel")
                        .WithMany("ExpenseEntryAmortizations")
                        .HasForeignKey("ExpenseEntryDetailModelExpDtl_ID");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.ExpenseEntryDetailModel", b =>
                {
                    b.HasOne("ExpenseProcessingSystem.Models.ExpenseEntryModel")
                        .WithMany("ExpenseEntryDetails")
                        .HasForeignKey("ExpenseEntryModelExpense_ID");
                });

            modelBuilder.Entity("ExpenseProcessingSystem.Models.ExpenseEntryGbaseDtl", b =>
                {
                    b.HasOne("ExpenseProcessingSystem.Models.ExpenseEntryDetailModel")
                        .WithMany("ExpenseEntryGbaseDtls")
                        .HasForeignKey("ExpenseEntryDetailModelExpDtl_ID");
                });
#pragma warning restore 612, 618
        }
    }
}
