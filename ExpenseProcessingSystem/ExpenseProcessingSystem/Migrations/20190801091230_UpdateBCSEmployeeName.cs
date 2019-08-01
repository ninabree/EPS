using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class UpdateBCSEmployeeName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pending_BCS_Name",
                table: "DMBCS_Pending");

            migrationBuilder.DropColumn(
                name: "BCS_Name",
                table: "DMBCS");

            migrationBuilder.AddColumn<int>(
                name: "Pending_BCS_Emp_MasterID",
                table: "DMBCS_Pending",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BCS_Emp_MasterID",
                table: "DMBCS",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pending_BCS_Emp_MasterID",
                table: "DMBCS_Pending");

            migrationBuilder.DropColumn(
                name: "BCS_Emp_MasterID",
                table: "DMBCS");

            migrationBuilder.AddColumn<string>(
                name: "Pending_BCS_Name",
                table: "DMBCS_Pending",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BCS_Name",
                table: "DMBCS",
                nullable: true);
        }
    }
}
