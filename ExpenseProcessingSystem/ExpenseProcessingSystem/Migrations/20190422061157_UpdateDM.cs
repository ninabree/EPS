using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class UpdateDM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DMNCC");

            migrationBuilder.DropTable(
                name: "DMNCC_Pending");

            migrationBuilder.DropColumn(
                name: "FBT_Account",
                table: "DMFBT");

            migrationBuilder.AddColumn<string>(
                name: "TR_Nature_Income_Payment",
                table: "DMTR",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Account_FBT_MasterID",
                table: "DMAccount",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TR_Nature_Income_Payment",
                table: "DMTR");

            migrationBuilder.DropColumn(
                name: "Account_FBT_MasterID",
                table: "DMAccount");

            migrationBuilder.AddColumn<string>(
                name: "FBT_Account",
                table: "DMFBT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DMNCC",
                columns: table => new
                {
                    NCC_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NCC_Approver_ID = table.Column<int>(nullable: false),
                    NCC_Created_Date = table.Column<DateTime>(nullable: false),
                    NCC_Creator_ID = table.Column<int>(nullable: false),
                    NCC_Last_Updated = table.Column<DateTime>(nullable: false),
                    NCC_MasterID = table.Column<int>(nullable: false),
                    NCC_Name = table.Column<string>(nullable: true),
                    NCC_Pro_Forma = table.Column<string>(nullable: true),
                    NCC_Status = table.Column<string>(nullable: true),
                    NCC_isActive = table.Column<bool>(nullable: false),
                    NCC_isDeleted = table.Column<bool>(nullable: false)
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
                    Pending_NCC_Approver_ID = table.Column<int>(nullable: false),
                    Pending_NCC_Creator_ID = table.Column<int>(nullable: false),
                    Pending_NCC_Filed_Date = table.Column<DateTime>(nullable: false),
                    Pending_NCC_MasterID = table.Column<int>(nullable: false),
                    Pending_NCC_Name = table.Column<string>(nullable: true),
                    Pending_NCC_Pro_Forma = table.Column<string>(nullable: true),
                    Pending_NCC_Status = table.Column<string>(nullable: true),
                    Pending_NCC_isActive = table.Column<bool>(nullable: false),
                    Pending_NCC_isDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMNCC_Pending", x => x.Pending_NCC_ID);
                });
        }
    }
}
