using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class Changedamortizationstatusfromstringtoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Amor_Status",
                table: "ExpenseEntryAmortizations",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Amor_Status",
                table: "ExpenseEntryAmortizations",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
