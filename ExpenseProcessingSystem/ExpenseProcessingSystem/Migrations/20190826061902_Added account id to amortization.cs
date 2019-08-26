using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class Addedaccountidtoamortization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GW_Type",
                table: "GwriteTransLists",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Amor_Account",
                table: "ExpenseEntryAmortizations",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GW_Type",
                table: "GwriteTransLists");

            migrationBuilder.DropColumn(
                name: "Amor_Account",
                table: "ExpenseEntryAmortizations");
        }
    }
}
