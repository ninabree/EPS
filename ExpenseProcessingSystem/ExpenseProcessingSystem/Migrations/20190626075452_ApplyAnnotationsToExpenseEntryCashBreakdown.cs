using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class ApplyAnnotationsToExpenseEntryCashBreakdown : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LiqCashBreak_Denimination",
                table: "LiquidationCashBreakdown",
                newName: "LiqCashBreak_Denomination");

            migrationBuilder.RenameColumn(
                name: "CashBreak_Denimination",
                table: "ExpenseEntryCashBreakdown",
                newName: "CashBreak_Denomination");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LiqCashBreak_Denomination",
                table: "LiquidationCashBreakdown",
                newName: "LiqCashBreak_Denimination");

            migrationBuilder.RenameColumn(
                name: "CashBreak_Denomination",
                table: "ExpenseEntryCashBreakdown",
                newName: "CashBreak_Denimination");
        }
    }
}
