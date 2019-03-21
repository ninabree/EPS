using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class UpdatePendingPayee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Pending_Payee_IsDeleted",
                table: "DMPayee_Pending",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Pending_Payee_isActive",
                table: "DMPayee_Pending",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pending_Payee_IsDeleted",
                table: "DMPayee_Pending");

            migrationBuilder.DropColumn(
                name: "Pending_Payee_isActive",
                table: "DMPayee_Pending");
        }
    }
}
