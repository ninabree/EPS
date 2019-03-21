using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class AddPendingDMTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DMAccount_Pending",
                columns: table => new
                {
                    Pending_Account_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pending_Account_MasterID = table.Column<int>(nullable: false),
                    Pending_Account_Name = table.Column<string>(nullable: true),
                    Pending_Account_Code = table.Column<string>(nullable: true),
                    Pending_Account_No = table.Column<int>(nullable: false),
                    Pending_Account_Cust = table.Column<string>(nullable: true),
                    Pending_Account_Div = table.Column<string>(nullable: true),
                    Pending_Account_Fund = table.Column<string>(nullable: true),
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
                name: "DMCheck_Pending",
                columns: table => new
                {
                    Pending_Check_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pending_Check_MasterID = table.Column<int>(nullable: false),
                    Pending_Check_Input_Date = table.Column<DateTime>(nullable: false),
                    Pending_Check_Series_From = table.Column<string>(nullable: true),
                    Pending_Check_Series_To = table.Column<string>(nullable: true),
                    Pending_Check_Type = table.Column<string>(nullable: true),
                    Pending_Check_Name = table.Column<string>(nullable: true),
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
                name: "DMCurrency_Pending",
                columns: table => new
                {
                    Pending_Curr_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pending_Curr_MasterID = table.Column<int>(nullable: false),
                    Pending_Curr_Name = table.Column<string>(nullable: true),
                    Pending_Curr_CCY_Code = table.Column<string>(nullable: true),
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
                name: "DMDept_Pending",
                columns: table => new
                {
                    Pending_Dept_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pending_Dept_MasterID = table.Column<int>(nullable: false),
                    Pending_Dept_Name = table.Column<string>(nullable: true),
                    Pending_Dept_Code = table.Column<string>(nullable: true),
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
                name: "DMEWT_Pending",
                columns: table => new
                {
                    Pending_EWT_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pending_EWT_MasterID = table.Column<int>(nullable: false),
                    Pending_EWT_Nature = table.Column<string>(nullable: true),
                    Pending_EWT_Tax_Rate = table.Column<int>(nullable: false),
                    Pending_EWT_ATC = table.Column<string>(nullable: true),
                    Pending_EWT_Tax_Rate_Desc = table.Column<string>(nullable: true),
                    Pending_EWT_Creator_ID = table.Column<int>(nullable: false),
                    Pending_EWT_Approver_ID = table.Column<int>(nullable: false),
                    Pending_EWT_Filed_Date = table.Column<DateTime>(nullable: false),
                    Pending_EWT_Status = table.Column<string>(nullable: true),
                    Pending_EWT_isDeleted = table.Column<bool>(nullable: false),
                    Pending_EWT_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMEWT_Pending", x => x.Pending_EWT_ID);
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DMAccount_Pending");

            migrationBuilder.DropTable(
                name: "DMCheck_Pending");

            migrationBuilder.DropTable(
                name: "DMCurrency_Pending");

            migrationBuilder.DropTable(
                name: "DMDept_Pending");

            migrationBuilder.DropTable(
                name: "DMEWT_Pending");

            migrationBuilder.DropTable(
                name: "DMFBT_Pending");

            migrationBuilder.DropTable(
                name: "DMVAT_Pending");
        }
    }
}
