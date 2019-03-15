using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class AddRemainingDMModels : Migration
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
                    Account_Name = table.Column<string>(nullable: true),
                    Account_Code = table.Column<string>(nullable: true),
                    Account_No = table.Column<int>(nullable: false),
                    Account_Cust = table.Column<string>(nullable: true),
                    Account_Div = table.Column<string>(nullable: true),
                    Account_Fund = table.Column<string>(nullable: true),
                    Account_Creator_ID = table.Column<int>(nullable: false),
                    Account_Approver_ID = table.Column<int>(nullable: false),
                    Account_Created_Date = table.Column<DateTime>(nullable: false),
                    Account_Last_Updated = table.Column<DateTime>(nullable: false),
                    Account_Status = table.Column<string>(nullable: true),
                    Account_isDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMAccount", x => x.Account_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMCheck",
                columns: table => new
                {
                    Check_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Check_Input_Date = table.Column<DateTime>(nullable: false),
                    Check_Series_From = table.Column<string>(nullable: true),
                    Check_Series_To = table.Column<string>(nullable: true),
                    Check_Type = table.Column<string>(nullable: true),
                    Check_Name = table.Column<string>(nullable: true),
                    Check_Creator_ID = table.Column<int>(nullable: false),
                    Check_Approver_ID = table.Column<int>(nullable: false),
                    Check_Created_Date = table.Column<DateTime>(nullable: false),
                    Check_Last_Updated = table.Column<DateTime>(nullable: false),
                    Check_Status = table.Column<string>(nullable: true),
                    Check_isDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMCheck", x => x.Check_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMCurrency",
                columns: table => new
                {
                    Curr_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Curr_Name = table.Column<string>(nullable: true),
                    Curr_CCY_Code = table.Column<string>(nullable: true),
                    Curr_Creator_ID = table.Column<int>(nullable: false),
                    Curr_Approver_ID = table.Column<int>(nullable: false),
                    Curr_Created_Date = table.Column<DateTime>(nullable: false),
                    Curr_Last_Updated = table.Column<DateTime>(nullable: false),
                    Curr_Status = table.Column<string>(nullable: true),
                    Curr_isDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMCurrency", x => x.Curr_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMEWT",
                columns: table => new
                {
                    EWT_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EWT_Nature = table.Column<string>(nullable: true),
                    EWT_Tax_Rate = table.Column<int>(nullable: false),
                    EWT_ATC = table.Column<string>(nullable: true),
                    EWT_Tax_Rate_Desc = table.Column<string>(nullable: true),
                    EWT_Creator_ID = table.Column<int>(nullable: false),
                    EWT_Approver_ID = table.Column<int>(nullable: false),
                    EWT_Created_Date = table.Column<DateTime>(nullable: false),
                    EWT_Last_Updated = table.Column<DateTime>(nullable: false),
                    EWT_Status = table.Column<string>(nullable: true),
                    EWT_isDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMEWT", x => x.EWT_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMFBT",
                columns: table => new
                {
                    FBT_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FBT_Name = table.Column<string>(nullable: true),
                    FBT_Account = table.Column<string>(nullable: true),
                    FBT_Formula = table.Column<string>(nullable: true),
                    FBT_Tax_Rate = table.Column<int>(nullable: false),
                    FBT_Creator_ID = table.Column<int>(nullable: false),
                    FBT_Approver_ID = table.Column<int>(nullable: false),
                    FBT_Created_Date = table.Column<DateTime>(nullable: false),
                    FBT_Last_Updated = table.Column<DateTime>(nullable: false),
                    FBT_Status = table.Column<string>(nullable: true),
                    FBT_isDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMFBT", x => x.FBT_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMVAT",
                columns: table => new
                {
                    VAT_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VAT_Name = table.Column<string>(nullable: true),
                    VAT_Rate = table.Column<string>(nullable: true),
                    VAT_Creator_ID = table.Column<int>(nullable: false),
                    VAT_Approver_ID = table.Column<int>(nullable: false),
                    VAT_Created_Date = table.Column<DateTime>(nullable: false),
                    VAT_Last_Updated = table.Column<DateTime>(nullable: false),
                    VAT_Status = table.Column<string>(nullable: true),
                    VAT_isDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMVAT", x => x.VAT_ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Budget");

            migrationBuilder.DropTable(
                name: "DMAccount");

            migrationBuilder.DropTable(
                name: "DMCheck");

            migrationBuilder.DropTable(
                name: "DMCurrency");

            migrationBuilder.DropTable(
                name: "DMEWT");

            migrationBuilder.DropTable(
                name: "DMFBT");

            migrationBuilder.DropTable(
                name: "DMVAT");
        }
    }
}
