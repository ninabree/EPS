using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class NCPeriodToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpNCDtl_Remarks_Period_To",
                table: "ExpenseEntryNonCashDetails");
            migrationBuilder.DropColumn(
                name: "ExpNCDtl_Remarks_Period_From",
                table: "ExpenseEntryNonCashDetails");

            migrationBuilder.AddColumn<string>(
                name: "ExpNCDtl_Remarks_Period",
                table: "ExpenseEntryNonCashDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpNCDtl_Remarks_Period",
                table: "ExpenseEntryNonCashDetails",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpNCDtl_Remarks_Period_To",
                table: "ExpenseEntryNonCashDetails",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            
        }
    }
}
