using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Budget",
                columns: table => new
                {
                    Budget_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Budget_Acc_ID = table.Column<string>(nullable: true),
                    Budget_Amount = table.Column<string>(nullable: true),
                    Budget_Creator_ID = table.Column<int>(nullable: false),
                    Budget_Approver_ID = table.Column<int>(nullable: false),
                    Budget_Created_Date = table.Column<DateTime>(nullable: false),
                    Budget_Last_Updated = table.Column<DateTime>(nullable: false),
                    Budget_Status = table.Column<string>(nullable: true),
                    Budget_isDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget", x => x.Budget_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMAccount",
                columns: table => new
                {
                    Account_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Account_MasterID = table.Column<int>(nullable: false),
                    Account_Name = table.Column<string>(nullable: true),
                    Account_Code = table.Column<string>(nullable: true),
                    Account_No = table.Column<string>(nullable: true),
                    Account_Cust = table.Column<string>(nullable: true),
                    Account_Div = table.Column<string>(nullable: true),
                    Account_Fund = table.Column<bool>(nullable: false),
                    Account_Creator_ID = table.Column<int>(nullable: false),
                    Account_Approver_ID = table.Column<int>(nullable: false),
                    Account_Created_Date = table.Column<DateTime>(nullable: false),
                    Account_Last_Updated = table.Column<DateTime>(nullable: false),
                    Account_Status = table.Column<string>(nullable: true),
                    Account_isDeleted = table.Column<bool>(nullable: false),
                    Account_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMAccount", x => x.Account_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMAccount_Pending",
                columns: table => new
                {
                    Pending_Account_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pending_Account_MasterID = table.Column<int>(nullable: false),
                    Pending_Account_Name = table.Column<string>(nullable: true),
                    Pending_Account_Code = table.Column<string>(nullable: true),
                    Pending_Account_No = table.Column<string>(nullable: true),
                    Pending_Account_Cust = table.Column<string>(nullable: true),
                    Pending_Account_Div = table.Column<string>(nullable: true),
                    Pending_Account_Fund = table.Column<bool>(nullable: false),
                    Pending_Account_Creator_ID = table.Column<int>(nullable: false),
                    Pending_Account_Approver_ID = table.Column<int>(nullable: false),
                    Pending_Account_Filed_Date = table.Column<DateTime>(nullable: false),
                    Pending_Account_Status = table.Column<string>(nullable: true),
                    Pending_Account_isDeleted = table.Column<bool>(nullable: false),
                    Pending_Account_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMAccount_Pending", x => x.Pending_Account_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMBCS",
                columns: table => new
                {
                    BCS_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BCS_MasterID = table.Column<int>(nullable: false),
                    BCS_Name = table.Column<string>(nullable: true),
                    BCS_TIN = table.Column<int>(nullable: false),
                    BCS_Position = table.Column<string>(nullable: true),
                    BCS_Signatures = table.Column<string>(nullable: true),
                    BCS_Creator_ID = table.Column<int>(nullable: false),
                    BCS_Approver_ID = table.Column<int>(nullable: false),
                    BCS_Created_Date = table.Column<DateTime>(nullable: false),
                    BCS_Last_Updated = table.Column<DateTime>(nullable: false),
                    BCS_Status = table.Column<string>(nullable: true),
                    BCS_isDeleted = table.Column<bool>(nullable: false),
                    BCS_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMBCS", x => x.BCS_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMBCS_Pending",
                columns: table => new
                {
                    Pending_BCS_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pending_BCS_MasterID = table.Column<int>(nullable: false),
                    Pending_BCS_Name = table.Column<string>(nullable: true),
                    Pending_BCS_TIN = table.Column<int>(nullable: false),
                    Pending_BCS_Position = table.Column<string>(nullable: true),
                    Pending_BCS_Signatures = table.Column<string>(nullable: true),
                    Pending_BCS_Creator_ID = table.Column<int>(nullable: false),
                    Pending_BCS_Approver_ID = table.Column<int>(nullable: false),
                    Pending_BCS_Filed_Date = table.Column<DateTime>(nullable: false),
                    Pending_BCS_Status = table.Column<string>(nullable: true),
                    Pending_BCS_isDeleted = table.Column<bool>(nullable: false),
                    Pending_BCS_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMBCS_Pending", x => x.Pending_BCS_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMCheck",
                columns: table => new
                {
                    Check_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Check_MasterID = table.Column<int>(nullable: false),
                    Check_Input_Date = table.Column<DateTime>(nullable: false),
                    Check_Series_From = table.Column<string>(nullable: true),
                    Check_Series_To = table.Column<string>(nullable: true),
                    Check_Bank_Info = table.Column<string>(nullable: true),
                    Check_Creator_ID = table.Column<int>(nullable: false),
                    Check_Approver_ID = table.Column<int>(nullable: false),
                    Check_Created_Date = table.Column<DateTime>(nullable: false),
                    Check_Last_Updated = table.Column<DateTime>(nullable: false),
                    Check_Status = table.Column<string>(nullable: true),
                    Check_isDeleted = table.Column<bool>(nullable: false),
                    Check_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMCheck", x => x.Check_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMCheck_Pending",
                columns: table => new
                {
                    Pending_Check_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pending_Check_MasterID = table.Column<int>(nullable: false),
                    Pending_Check_Input_Date = table.Column<DateTime>(nullable: false),
                    Pending_Check_Series_From = table.Column<string>(nullable: true),
                    Pending_Check_Series_To = table.Column<string>(nullable: true),
                    Pending_Check_Bank_Info = table.Column<string>(nullable: true),
                    Pending_Check_Creator_ID = table.Column<int>(nullable: false),
                    Pending_Check_Approver_ID = table.Column<int>(nullable: false),
                    Pending_Check_Filed_Date = table.Column<DateTime>(nullable: false),
                    Pending_Check_Status = table.Column<string>(nullable: true),
                    Pending_Check_isDeleted = table.Column<bool>(nullable: false),
                    Pending_Check_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMCheck_Pending", x => x.Pending_Check_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMCurrency",
                columns: table => new
                {
                    Curr_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Curr_MasterID = table.Column<int>(nullable: false),
                    Curr_Name = table.Column<string>(nullable: true),
                    Curr_CCY_ABBR = table.Column<string>(nullable: true),
                    Curr_Creator_ID = table.Column<int>(nullable: false),
                    Curr_Approver_ID = table.Column<int>(nullable: false),
                    Curr_Created_Date = table.Column<DateTime>(nullable: false),
                    Curr_Last_Updated = table.Column<DateTime>(nullable: false),
                    Curr_Status = table.Column<string>(nullable: true),
                    Curr_isDeleted = table.Column<bool>(nullable: false),
                    Curr_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMCurrency", x => x.Curr_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMCurrency_Pending",
                columns: table => new
                {
                    Pending_Curr_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pending_Curr_MasterID = table.Column<int>(nullable: false),
                    Pending_Curr_Name = table.Column<string>(nullable: true),
                    Pending_Curr_CCY_ABBR = table.Column<string>(nullable: true),
                    Pending_Curr_Creator_ID = table.Column<int>(nullable: false),
                    Pending_Curr_Approver_ID = table.Column<int>(nullable: false),
                    Pending_Curr_Filed_Date = table.Column<DateTime>(nullable: false),
                    Pending_Curr_Status = table.Column<string>(nullable: true),
                    Pending_Curr_isDeleted = table.Column<bool>(nullable: false),
                    Pending_Curr_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMCurrency_Pending", x => x.Pending_Curr_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMCust",
                columns: table => new
                {
                    Cust_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Cust_MasterID = table.Column<int>(nullable: false),
                    Cust_Name = table.Column<string>(nullable: true),
                    Cust_Abbr = table.Column<string>(nullable: true),
                    Cust_No = table.Column<string>(nullable: true),
                    Cust_Creator_ID = table.Column<int>(nullable: false),
                    Cust_Approver_ID = table.Column<int>(nullable: false),
                    Cust_Created_Date = table.Column<DateTime>(nullable: false),
                    Cust_Last_Updated = table.Column<DateTime>(nullable: false),
                    Cust_Status = table.Column<string>(nullable: true),
                    Cust_isDeleted = table.Column<bool>(nullable: false),
                    Cust_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMCust", x => x.Cust_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMCust_Pending",
                columns: table => new
                {
                    Pending_Cust_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pending_Cust_MasterID = table.Column<int>(nullable: false),
                    Pending_Cust_Name = table.Column<string>(nullable: true),
                    Pending_Cust_Abbr = table.Column<string>(nullable: true),
                    Pending_Cust_No = table.Column<string>(nullable: true),
                    Pending_Cust_Creator_ID = table.Column<int>(nullable: false),
                    Pending_Cust_Approver_ID = table.Column<int>(nullable: false),
                    Pending_Cust_Filed_Date = table.Column<DateTime>(nullable: false),
                    Pending_Cust_Status = table.Column<string>(nullable: true),
                    Pending_Cust_isDeleted = table.Column<bool>(nullable: false),
                    Pending_Cust_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMCust_Pending", x => x.Pending_Cust_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMDept",
                columns: table => new
                {
                    Dept_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Dept_MasterID = table.Column<int>(nullable: false),
                    Dept_Name = table.Column<string>(nullable: true),
                    Dept_Code = table.Column<string>(nullable: true),
                    Dept_Budget_Unit = table.Column<string>(nullable: true),
                    Dept_Creator_ID = table.Column<int>(nullable: false),
                    Dept_Approver_ID = table.Column<int>(nullable: false),
                    Dept_Created_Date = table.Column<DateTime>(nullable: false),
                    Dept_Last_Updated = table.Column<DateTime>(nullable: false),
                    Dept_Status = table.Column<string>(nullable: true),
                    Dept_isDeleted = table.Column<bool>(nullable: false),
                    Dept_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMDept", x => x.Dept_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMDept_Pending",
                columns: table => new
                {
                    Pending_Dept_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pending_Dept_MasterID = table.Column<int>(nullable: false),
                    Pending_Dept_Name = table.Column<string>(nullable: true),
                    Pending_Dept_Code = table.Column<string>(nullable: true),
                    Pending_Dept_Budget_Unit = table.Column<string>(nullable: true),
                    Pending_Dept_Creator_ID = table.Column<int>(nullable: false),
                    Pending_Dept_Approver_ID = table.Column<int>(nullable: false),
                    Pending_Dept_Filed_Date = table.Column<DateTime>(nullable: false),
                    Pending_Dept_Status = table.Column<string>(nullable: true),
                    Pending_Dept_isDeleted = table.Column<bool>(nullable: false),
                    Pending_Dept_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMDept_Pending", x => x.Pending_Dept_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMEmp",
                columns: table => new
                {
                    Emp_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Emp_MasterID = table.Column<int>(nullable: false),
                    Emp_Name = table.Column<string>(nullable: true),
                    Emp_Acc_No = table.Column<string>(nullable: true),
                    Emp_Type = table.Column<string>(nullable: true),
                    Emp_Creator_ID = table.Column<int>(nullable: false),
                    Emp_Approver_ID = table.Column<int>(nullable: false),
                    Emp_Created_Date = table.Column<DateTime>(nullable: false),
                    Emp_Last_Updated = table.Column<DateTime>(nullable: false),
                    Emp_Status = table.Column<string>(nullable: true),
                    Emp_isDeleted = table.Column<bool>(nullable: false),
                    Emp_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMEmp", x => x.Emp_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMEmp_Pending",
                columns: table => new
                {
                    Pending_Emp_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pending_Emp_MasterID = table.Column<int>(nullable: false),
                    Pending_Emp_Name = table.Column<string>(nullable: true),
                    Pending_Emp_Acc_No = table.Column<string>(nullable: true),
                    Pending_Emp_Type = table.Column<string>(nullable: true),
                    Pending_Emp_Creator_ID = table.Column<int>(nullable: false),
                    Pending_Emp_Approver_ID = table.Column<int>(nullable: false),
                    Pending_Emp_Filed_Date = table.Column<DateTime>(nullable: false),
                    Pending_Emp_Status = table.Column<string>(nullable: true),
                    Pending_Emp_isDeleted = table.Column<bool>(nullable: false),
                    Pending_Emp_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMEmp_Pending", x => x.Pending_Emp_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMFBT",
                columns: table => new
                {
                    FBT_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FBT_MasterID = table.Column<int>(nullable: false),
                    FBT_Name = table.Column<string>(nullable: true),
                    FBT_Account = table.Column<string>(nullable: true),
                    FBT_Formula = table.Column<string>(nullable: true),
                    FBT_Tax_Rate = table.Column<int>(nullable: false),
                    FBT_Creator_ID = table.Column<int>(nullable: false),
                    FBT_Approver_ID = table.Column<int>(nullable: false),
                    FBT_Created_Date = table.Column<DateTime>(nullable: false),
                    FBT_Last_Updated = table.Column<DateTime>(nullable: false),
                    FBT_Status = table.Column<string>(nullable: true),
                    FBT_isDeleted = table.Column<bool>(nullable: false),
                    FBT_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMFBT", x => x.FBT_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMFBT_Pending",
                columns: table => new
                {
                    Pending_FBT_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pending_FBT_MasterID = table.Column<int>(nullable: false),
                    Pending_FBT_Name = table.Column<string>(nullable: true),
                    Pending_FBT_Account = table.Column<string>(nullable: true),
                    Pending_FBT_Formula = table.Column<string>(nullable: true),
                    Pending_FBT_Tax_Rate = table.Column<int>(nullable: false),
                    Pending_FBT_Creator_ID = table.Column<int>(nullable: false),
                    Pending_FBT_Approver_ID = table.Column<int>(nullable: false),
                    Pending_FBT_Filed_Date = table.Column<DateTime>(nullable: false),
                    Pending_FBT_Status = table.Column<string>(nullable: true),
                    Pending_FBT_isDeleted = table.Column<bool>(nullable: false),
                    Pending_FBT_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMFBT_Pending", x => x.Pending_FBT_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMNCC",
                columns: table => new
                {
                    NCC_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NCC_MasterID = table.Column<int>(nullable: false),
                    NCC_Name = table.Column<string>(nullable: true),
                    NCC_Pro_Forma = table.Column<string>(nullable: true),
                    NCC_Creator_ID = table.Column<int>(nullable: false),
                    NCC_Approver_ID = table.Column<int>(nullable: false),
                    NCC_Created_Date = table.Column<DateTime>(nullable: false),
                    NCC_Last_Updated = table.Column<DateTime>(nullable: false),
                    NCC_Status = table.Column<string>(nullable: true),
                    NCC_isDeleted = table.Column<bool>(nullable: false),
                    NCC_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMNCC", x => x.NCC_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMNCC_Pending",
                columns: table => new
                {
                    Pending_NCC_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pending_NCC_MasterID = table.Column<int>(nullable: false),
                    Pending_NCC_Name = table.Column<string>(nullable: true),
                    Pending_NCC_Pro_Forma = table.Column<string>(nullable: true),
                    Pending_NCC_Creator_ID = table.Column<int>(nullable: false),
                    Pending_NCC_Approver_ID = table.Column<int>(nullable: false),
                    Pending_NCC_Filed_Date = table.Column<DateTime>(nullable: false),
                    Pending_NCC_Status = table.Column<string>(nullable: true),
                    Pending_NCC_isDeleted = table.Column<bool>(nullable: false),
                    Pending_NCC_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMNCC_Pending", x => x.Pending_NCC_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMTR",
                columns: table => new
                {
                    TR_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TR_MasterID = table.Column<int>(nullable: false),
                    TR_WT_Title = table.Column<string>(nullable: true),
                    TR_Nature = table.Column<string>(nullable: true),
                    TR_ATC = table.Column<string>(nullable: true),
                    TR_Tax_Rate = table.Column<int>(nullable: false),
                    TR_Creator_ID = table.Column<int>(nullable: false),
                    TR_Approver_ID = table.Column<int>(nullable: false),
                    TR_Created_Date = table.Column<DateTime>(nullable: false),
                    TR_Last_Updated = table.Column<DateTime>(nullable: false),
                    TR_Status = table.Column<string>(nullable: true),
                    TR_isDeleted = table.Column<bool>(nullable: false),
                    TR_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMTR", x => x.TR_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMTR_Pending",
                columns: table => new
                {
                    Pending_TR_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pending_TR_MasterID = table.Column<int>(nullable: false),
                    Pending_TR_WT_Title = table.Column<string>(nullable: true),
                    Pending_TR_Nature = table.Column<string>(nullable: true),
                    Pending_TR_Tax_Rate = table.Column<int>(nullable: false),
                    Pending_TR_ATC = table.Column<string>(nullable: true),
                    Pending_TR_Creator_ID = table.Column<int>(nullable: false),
                    Pending_TR_Approver_ID = table.Column<int>(nullable: false),
                    Pending_TR_Filed_Date = table.Column<DateTime>(nullable: false),
                    Pending_TR_Status = table.Column<string>(nullable: true),
                    Pending_TR_isDeleted = table.Column<bool>(nullable: false),
                    Pending_TR_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMTR_Pending", x => x.Pending_TR_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMVAT",
                columns: table => new
                {
                    VAT_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VAT_MasterID = table.Column<int>(nullable: false),
                    VAT_Name = table.Column<string>(nullable: true),
                    VAT_Rate = table.Column<string>(nullable: true),
                    VAT_Creator_ID = table.Column<int>(nullable: false),
                    VAT_Approver_ID = table.Column<int>(nullable: false),
                    VAT_Created_Date = table.Column<DateTime>(nullable: false),
                    VAT_Last_Updated = table.Column<DateTime>(nullable: false),
                    VAT_Status = table.Column<string>(nullable: true),
                    VAT_isDeleted = table.Column<bool>(nullable: false),
                    VAT_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMVAT", x => x.VAT_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMVAT_Pending",
                columns: table => new
                {
                    Pending_VAT_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pending_VAT_MasterID = table.Column<int>(nullable: false),
                    Pending_VAT_Name = table.Column<string>(nullable: true),
                    Pending_VAT_Rate = table.Column<string>(nullable: true),
                    Pending_VAT_Creator_ID = table.Column<int>(nullable: false),
                    Pending_VAT_Approver_ID = table.Column<int>(nullable: false),
                    Pending_VAT_Filed_Date = table.Column<DateTime>(nullable: false),
                    Pending_VAT_Status = table.Column<string>(nullable: true),
                    Pending_VAT_isDeleted = table.Column<bool>(nullable: false),
                    Pending_VAT_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMVAT_Pending", x => x.Pending_VAT_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMVendor",
                columns: table => new
                {
                    Vendor_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Vendor_MasterID = table.Column<int>(nullable: false),
                    Vendor_Name = table.Column<string>(nullable: true),
                    Vendor_TIN = table.Column<string>(nullable: true),
                    Vendor_Address = table.Column<string>(nullable: true),
                    Vendor_Creator_ID = table.Column<int>(nullable: false),
                    Vendor_Approver_ID = table.Column<int>(nullable: false),
                    Vendor_Created_Date = table.Column<DateTime>(nullable: false),
                    Vendor_Last_Updated = table.Column<DateTime>(nullable: false),
                    Vendor_Status = table.Column<string>(nullable: true),
                    Vendor_isDeleted = table.Column<bool>(nullable: false),
                    Vendor_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMVendor", x => x.Vendor_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMVendor_Pending",
                columns: table => new
                {
                    Pending_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pending_Vendor_MasterID = table.Column<int>(nullable: false),
                    Pending_Vendor_Name = table.Column<string>(nullable: true),
                    Pending_Vendor_TIN = table.Column<string>(nullable: true),
                    Pending_Vendor_Address = table.Column<string>(nullable: true),
                    Pending_Vendor_Creator_ID = table.Column<int>(nullable: false),
                    Pending_Vendor_Approver_ID = table.Column<int>(nullable: false),
                    Pending_Vendor_Filed_Date = table.Column<DateTime>(nullable: false),
                    Pending_Vendor_Status = table.Column<string>(nullable: true),
                    Pending_Vendor_IsDeleted = table.Column<bool>(nullable: false),
                    Pending_Vendor_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMVendor_Pending", x => x.Pending_ID);
                });

            migrationBuilder.CreateTable(
                name: "Notif",
                columns: table => new
                {
                    Notif_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Notif_Application_ID = table.Column<int>(nullable: false),
                    Notif_User_ID = table.Column<int>(nullable: false),
                    Notif_Apprvr_ID = table.Column<int>(nullable: false),
                    Notif_Message = table.Column<string>(nullable: true),
                    Notif_Link_Address = table.Column<string>(nullable: true),
                    Notif_Date = table.Column<DateTime>(nullable: false),
                    Notif_Status = table.Column<bool>(nullable: false),
                    Notif_Type_Status = table.Column<string>(nullable: true),
                    Notif_Type_Screen = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notif", x => x.Notif_ID);
                });

            migrationBuilder.CreateTable(
                name: "SystemMessageModels",
                columns: table => new
                {
                    Msg_Code = table.Column<string>(nullable: false),
                    Msg_Type = table.Column<string>(nullable: true),
                    Msg_Content = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemMessageModels", x => x.Msg_Code);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    User_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    User_UserName = table.Column<string>(nullable: true),
                    User_FName = table.Column<string>(nullable: true),
                    User_LName = table.Column<string>(nullable: true),
                    User_Password = table.Column<string>(nullable: true),
                    User_DeptID = table.Column<int>(nullable: false),
                    User_Email = table.Column<string>(nullable: true),
                    User_Role = table.Column<string>(nullable: true),
                    User_Comment = table.Column<string>(nullable: true),
                    User_InUse = table.Column<bool>(nullable: false),
                    User_Creator_ID = table.Column<int>(nullable: false),
                    User_Approver_ID = table.Column<int>(nullable: false),
                    User_Created_Date = table.Column<DateTime>(nullable: false),
                    User_Last_Updated = table.Column<DateTime>(nullable: false),
                    User_Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.User_ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Budget");

            migrationBuilder.DropTable(
                name: "DMAccount");

            migrationBuilder.DropTable(
                name: "DMAccount_Pending");

            migrationBuilder.DropTable(
                name: "DMBCS");

            migrationBuilder.DropTable(
                name: "DMBCS_Pending");

            migrationBuilder.DropTable(
                name: "DMCheck");

            migrationBuilder.DropTable(
                name: "DMCheck_Pending");

            migrationBuilder.DropTable(
                name: "DMCurrency");

            migrationBuilder.DropTable(
                name: "DMCurrency_Pending");

            migrationBuilder.DropTable(
                name: "DMCust");

            migrationBuilder.DropTable(
                name: "DMCust_Pending");

            migrationBuilder.DropTable(
                name: "DMDept");

            migrationBuilder.DropTable(
                name: "DMDept_Pending");

            migrationBuilder.DropTable(
                name: "DMEmp");

            migrationBuilder.DropTable(
                name: "DMEmp_Pending");

            migrationBuilder.DropTable(
                name: "DMFBT");

            migrationBuilder.DropTable(
                name: "DMFBT_Pending");

            migrationBuilder.DropTable(
                name: "DMNCC");

            migrationBuilder.DropTable(
                name: "DMNCC_Pending");

            migrationBuilder.DropTable(
                name: "DMTR");

            migrationBuilder.DropTable(
                name: "DMTR_Pending");

            migrationBuilder.DropTable(
                name: "DMVAT");

            migrationBuilder.DropTable(
                name: "DMVAT_Pending");

            migrationBuilder.DropTable(
                name: "DMVendor");

            migrationBuilder.DropTable(
                name: "DMVendor_Pending");

            migrationBuilder.DropTable(
                name: "Notif");

            migrationBuilder.DropTable(
                name: "SystemMessageModels");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
