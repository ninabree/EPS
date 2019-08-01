using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class UpdateBCSEmployeeName2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Pending_BCS_User_ID",
                table: "DMBCS_Pending",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BCS_User_ID",
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
        }
    }
}
