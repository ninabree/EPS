using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class DMAccountAddCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Account_Currency_MasterID",
                table: "DMAccount",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Account_Currency_MasterID",
                table: "DMAccount");
        }
    }
}
