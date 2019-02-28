using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class UpdateAccountModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Acc_FName",
                table: "Account",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Acc_LName",
                table: "Account",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Acc_FName",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Acc_LName",
                table: "Account");
        }
    }
}
