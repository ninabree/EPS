using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class ApplyAnnotationsToExpenseEntryDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpDtl_CreditAccount1",
                table: "ExpenseEntryDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExpDtl_CreditAccount2",
                table: "ExpenseEntryDetails",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpDtl_CreditAccount1",
                table: "ExpenseEntryDetails");

            migrationBuilder.DropColumn(
                name: "ExpDtl_CreditAccount2",
                table: "ExpenseEntryDetails");

        }
    }
}
