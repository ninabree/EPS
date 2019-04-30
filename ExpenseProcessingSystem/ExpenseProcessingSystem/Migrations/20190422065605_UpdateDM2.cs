using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class UpdateDM2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pending_FBT_Account",
                table: "DMFBT_Pending");

            migrationBuilder.AddColumn<int>(
                name: "Pending_Account_FBT_MasterID",
                table: "DMAccount_Pending",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pending_Account_FBT_MasterID",
                table: "DMAccount_Pending");

            migrationBuilder.AddColumn<string>(
                name: "Pending_FBT_Account",
                table: "DMFBT_Pending",
                nullable: true);
        }
    }
}
