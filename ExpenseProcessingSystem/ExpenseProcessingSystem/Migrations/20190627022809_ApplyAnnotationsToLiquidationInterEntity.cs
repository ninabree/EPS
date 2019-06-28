using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class ApplyAnnotationsToLiquidationInterEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiquidationInterEntity_LiquidationEntryDetails_LiquidationEntryDetailModelLiq_DtlID",
                table: "LiquidationInterEntity");

            migrationBuilder.RenameColumn(
                name: "LiquidationEntryDetailModelLiq_DtlID",
                table: "LiquidationInterEntity",
                newName: "ExpenseEntryDetailModelExpDtl_ID");

            migrationBuilder.RenameIndex(
                name: "IX_LiquidationInterEntity_LiquidationEntryDetailModelLiq_DtlID",
                table: "LiquidationInterEntity",
                newName: "IX_LiquidationInterEntity_ExpenseEntryDetailModelExpDtl_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_LiquidationInterEntity_ExpenseEntryDetails_ExpenseEntryDetailModelExpDtl_ID",
                table: "LiquidationInterEntity",
                column: "ExpenseEntryDetailModelExpDtl_ID",
                principalTable: "ExpenseEntryDetails",
                principalColumn: "ExpDtl_ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LiquidationInterEntity_ExpenseEntryDetails_ExpenseEntryDetailModelExpDtl_ID",
                table: "LiquidationInterEntity");

            migrationBuilder.RenameColumn(
                name: "ExpenseEntryDetailModelExpDtl_ID",
                table: "LiquidationInterEntity",
                newName: "LiquidationEntryDetailModelLiq_DtlID");

            migrationBuilder.RenameIndex(
                name: "IX_LiquidationInterEntity_ExpenseEntryDetailModelExpDtl_ID",
                table: "LiquidationInterEntity",
                newName: "IX_LiquidationInterEntity_LiquidationEntryDetailModelLiq_DtlID");

            migrationBuilder.AddForeignKey(
                name: "FK_LiquidationInterEntity_LiquidationEntryDetails_LiquidationEntryDetailModelLiq_DtlID",
                table: "LiquidationInterEntity",
                column: "LiquidationEntryDetailModelLiq_DtlID",
                principalTable: "LiquidationEntryDetails",
                principalColumn: "Liq_DtlID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
