using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class ExpenseEntryUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Expense_Verifier",
                table: "ExpenseEntry",
                newName: "Expense_Verifier_2");

            migrationBuilder.AddColumn<int>(
                name: "Expense_Verifier_1",
                table: "ExpenseEntry",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Expense_Verifier_1",
                table: "ExpenseEntry");

            migrationBuilder.RenameColumn(
                name: "Expense_Verifier_2",
                table: "ExpenseEntry",
                newName: "Expense_Verifier");
        }
    }
}
