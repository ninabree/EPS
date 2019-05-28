using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class UpdateEntryModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ExpDtl_Vat",
                table: "ExpenseEntryDetails",
                nullable: false,
                oldClrType: typeof(float));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "ExpDtl_Vat",
                table: "ExpenseEntryDetails",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
