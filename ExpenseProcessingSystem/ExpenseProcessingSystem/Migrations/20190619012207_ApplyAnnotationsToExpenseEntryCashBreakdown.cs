using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class ApplyAnnotationsToExpenseEntryCashBreakdown : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CashBreak_NoPcs",
                table: "ExpenseEntryCashBreakdown",
                nullable: false,
                oldClrType: typeof(double));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "CashBreak_NoPcs",
                table: "ExpenseEntryCashBreakdown",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
