using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class Removepcb_idfrompettycash : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PCB_ID",
                table: "PettyCash");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PCB_ID",
                table: "PettyCash",
                nullable: false,
                defaultValue: 0);
        }
    }
}
