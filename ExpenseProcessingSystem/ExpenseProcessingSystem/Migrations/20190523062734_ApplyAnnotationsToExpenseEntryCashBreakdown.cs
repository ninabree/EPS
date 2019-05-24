using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class ApplyAnnotationsToExpenseEntryCashBreakdown : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "CashBreak_NoPcs",
                table: "ExpenseEntryCashBreakdown",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<double>(
                name: "CashBreak_Amount",
                table: "ExpenseEntryCashBreakdown",
                nullable: false,
                oldClrType: typeof(float));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "CashBreak_NoPcs",
                table: "ExpenseEntryCashBreakdown",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<float>(
                name: "CashBreak_Amount",
                table: "ExpenseEntryCashBreakdown",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
