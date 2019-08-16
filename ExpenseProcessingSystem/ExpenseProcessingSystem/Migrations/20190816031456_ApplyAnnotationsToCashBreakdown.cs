using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class ApplyAnnotationsToCashBreakdown : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "LiqCashBreak_Amount",
                table: "LiquidationCashBreakdown",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "CashBreak_Amount",
                table: "ExpenseEntryCashBreakdown",
                nullable: false,
                oldClrType: typeof(double));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "LiqCashBreak_Amount",
                table: "LiquidationCashBreakdown",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "CashBreak_Amount",
                table: "ExpenseEntryCashBreakdown",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
