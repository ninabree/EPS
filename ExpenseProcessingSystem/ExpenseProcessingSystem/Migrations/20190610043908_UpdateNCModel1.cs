using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class UpdateNCModel1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpNC_CredAmt",
                table: "ExpenseEntryNonCash");

            migrationBuilder.DropColumn(
                name: "ExpNC_DebitAmt",
                table: "ExpenseEntryNonCash");

            migrationBuilder.DropColumn(
                name: "ExpNC_TotalAmt",
                table: "ExpenseEntryNonCash");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "ExpNC_CredAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ExpNC_DebitAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ExpNC_TotalAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
