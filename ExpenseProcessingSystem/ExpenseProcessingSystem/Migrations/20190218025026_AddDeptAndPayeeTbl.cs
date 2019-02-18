using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class AddDeptAndPayeeTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DMDept",
                columns: table => new
                {
                    Dept_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Dept_Name = table.Column<string>(nullable: true),
                    Dept_Code = table.Column<string>(nullable: true),
                    Dept_Creator_ID = table.Column<int>(nullable: false),
                    Dept_Approver_ID = table.Column<int>(nullable: false),
                    Dept_Created_Date = table.Column<DateTime>(nullable: false),
                    Dept_Last_Updated = table.Column<DateTime>(nullable: false),
                    Dept_Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMDept", x => x.Dept_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMPayee",
                columns: table => new
                {
                    Payee_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Payee_Name = table.Column<string>(nullable: true),
                    Payee_TIN = table.Column<string>(nullable: true),
                    Payee_Address = table.Column<string>(nullable: true),
                    Payee_Type = table.Column<string>(nullable: true),
                    Payee_No = table.Column<int>(nullable: false),
                    Payee_Creator_ID = table.Column<int>(nullable: false),
                    Payee_Approver_ID = table.Column<int>(nullable: false),
                    Payee_Created_Date = table.Column<DateTime>(nullable: false),
                    Payee_Last_Updated = table.Column<DateTime>(nullable: false),
                    Payee_Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMPayee", x => x.Payee_ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DMDept");

            migrationBuilder.DropTable(
                name: "DMPayee");
        }
    }
}
