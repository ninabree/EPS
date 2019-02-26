using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class AddIsDeletedColumnToDeptAndPayee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Payee_isDeleted",
                table: "DMPayee",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Dept_isDeleted",
                table: "DMDept",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Payee_isDeleted",
                table: "DMPayee");

            migrationBuilder.DropColumn(
                name: "Dept_isDeleted",
                table: "DMDept");
        }
    }
}
