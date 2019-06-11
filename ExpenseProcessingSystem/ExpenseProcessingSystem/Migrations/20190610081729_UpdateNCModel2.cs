using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class UpdateNCModel2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseEntryNonCashDetailAccounts_ExpenseEntryNonCashDetails_ExpenseEntryNCModelExpNCDtl_ID",
                table: "ExpenseEntryNonCashDetailAccounts");

            migrationBuilder.RenameColumn(
                name: "ExpenseEntryNCModelExpNCDtl_ID",
                table: "ExpenseEntryNonCashDetailAccounts",
                newName: "ExpenseEntryNCDtlModelExpNCDtl_ID");

            migrationBuilder.RenameIndex(
                name: "IX_ExpenseEntryNonCashDetailAccounts_ExpenseEntryNCModelExpNCDtl_ID",
                table: "ExpenseEntryNonCashDetailAccounts",
                newName: "IX_ExpenseEntryNonCashDetailAccounts_ExpenseEntryNCDtlModelExpNCDtl_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseEntryNonCashDetailAccounts_ExpenseEntryNonCashDetails_ExpenseEntryNCDtlModelExpNCDtl_ID",
                table: "ExpenseEntryNonCashDetailAccounts",
                column: "ExpenseEntryNCDtlModelExpNCDtl_ID",
                principalTable: "ExpenseEntryNonCashDetails",
                principalColumn: "ExpNCDtl_ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseEntryNonCashDetailAccounts_ExpenseEntryNonCashDetails_ExpenseEntryNCDtlModelExpNCDtl_ID",
                table: "ExpenseEntryNonCashDetailAccounts");

            migrationBuilder.RenameColumn(
                name: "ExpenseEntryNCDtlModelExpNCDtl_ID",
                table: "ExpenseEntryNonCashDetailAccounts",
                newName: "ExpenseEntryNCModelExpNCDtl_ID");

            migrationBuilder.RenameIndex(
                name: "IX_ExpenseEntryNonCashDetailAccounts_ExpenseEntryNCDtlModelExpNCDtl_ID",
                table: "ExpenseEntryNonCashDetailAccounts",
                newName: "IX_ExpenseEntryNonCashDetailAccounts_ExpenseEntryNCModelExpNCDtl_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseEntryNonCashDetailAccounts_ExpenseEntryNonCashDetails_ExpenseEntryNCModelExpNCDtl_ID",
                table: "ExpenseEntryNonCashDetailAccounts",
                column: "ExpenseEntryNCModelExpNCDtl_ID",
                principalTable: "ExpenseEntryNonCashDetails",
                principalColumn: "ExpNCDtl_ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
