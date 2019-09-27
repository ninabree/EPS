using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class ApplyAnnotationsToExpenseTransLists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TL_GBaseMessage",
                table: "ExpenseTransLists",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TL_StatusID",
                table: "ExpenseTransLists",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TL_GBaseMessage",
                table: "ExpenseTransLists");

            migrationBuilder.DropColumn(
                name: "TL_StatusID",
                table: "ExpenseTransLists");
        }
    }
}
