using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class UpdateDM3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TR_Nature_Income_Payment",
                table: "DMTR_Pending",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FBT_Abbr",
                table: "DMFBT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TR_Nature_Income_Payment",
                table: "DMTR_Pending");

            migrationBuilder.DropColumn(
                name: "FBT_Abbr",
                table: "DMFBT");
        }
    }
}
