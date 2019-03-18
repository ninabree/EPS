using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class MinorEditPendingPayee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Pending_Payee_Approver_ID",
                table: "DMPayee_Pending",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pending_Payee_Approver_ID",
                table: "DMPayee_Pending");
        }
    }
}
