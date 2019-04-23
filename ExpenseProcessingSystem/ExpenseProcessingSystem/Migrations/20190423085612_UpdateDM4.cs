using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class UpdateDM4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TR_Nature_Income_Payment",
                table: "DMTR_Pending",
                newName: "Pending_TR_Nature_Income_Payment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Pending_TR_Nature_Income_Payment",
                table: "DMTR_Pending",
                newName: "TR_Nature_Income_Payment");
        }
    }
}
