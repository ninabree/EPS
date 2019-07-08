using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class ApplyAnnotationsToExpenseTransLists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "ExpenseEntryAmortizations");

            migrationBuilder.AddColumn<int>(
                name: "TL_ExpenseDtlID",
                table: "ExpenseTransLists",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TL_ExpenseDtlID",
                table: "ExpenseTransLists");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "ExpenseEntryAmortizations",
                nullable: true);
        }
    }
}
