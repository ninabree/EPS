using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class Modifiedsometablecolumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GabseDtl_Amount",
                table: "ExpenseEntryGbaseDtls",
                newName: "GbaseDtl_Amount");

            migrationBuilder.RenameColumn(
                name: "ExpDt_Ccy",
                table: "ExpenseEntryDetails",
                newName: "ExpDtl_Ccy");

            migrationBuilder.AlterColumn<float>(
                name: "ExpDtl_Vat",
                table: "ExpenseEntryDetails",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GbaseDtl_Amount",
                table: "ExpenseEntryGbaseDtls",
                newName: "GabseDtl_Amount");

            migrationBuilder.RenameColumn(
                name: "ExpDtl_Ccy",
                table: "ExpenseEntryDetails",
                newName: "ExpDt_Ccy");

            migrationBuilder.AlterColumn<int>(
                name: "ExpDtl_Vat",
                table: "ExpenseEntryDetails",
                nullable: false,
                oldClrType: typeof(float));
        }
    }
}
