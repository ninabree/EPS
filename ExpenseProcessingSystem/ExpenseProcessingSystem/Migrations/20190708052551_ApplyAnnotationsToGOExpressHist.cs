using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class ApplyAnnotationsToGOExpressHist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpenseDetailModelExpDtl_ID",
                table: "GOExpressHist",
                newName: "ExpenseDetailID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpenseDetailModelExpDtl_ID",
                table: "GOExpressHist",
                newName: "ExpenseDetailID");
        }
    }
}
