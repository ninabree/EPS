using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class AddCatAndFBTToEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Pending_Emp_Category_ID",
                table: "DMEmp_Pending",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Pending_Emp_FBT_MasterID",
                table: "DMEmp_Pending",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Emp_Category_ID",
                table: "DMEmp",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Emp_FBT_MasterID",
                table: "DMEmp",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pending_Emp_Category_ID",
                table: "DMEmp_Pending");

            migrationBuilder.DropColumn(
                name: "Pending_Emp_FBT_MasterID",
                table: "DMEmp_Pending");

            migrationBuilder.DropColumn(
                name: "Emp_Category_ID",
                table: "DMEmp");

            migrationBuilder.DropColumn(
                name: "Emp_FBT_MasterID",
                table: "DMEmp");
        }
    }
}
