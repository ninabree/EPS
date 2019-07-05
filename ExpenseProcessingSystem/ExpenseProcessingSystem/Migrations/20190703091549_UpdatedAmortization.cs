using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class UpdatedAmortization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Amor_Status",
                table: "ExpenseEntryAmortizations",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Amor_Number",
                table: "ExpenseEntryAmortizations",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amor_Status",
                table: "ExpenseEntryAmortizations");

            migrationBuilder.DropColumn(
                name: "Amor_Number",
                table: "ExpenseEntryAmortizations");
        }
    }
}
