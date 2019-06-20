using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class LiquidationEntryDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LiquidationEntryDetails",
                columns: table => new
                {
                    Liq_DtlID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Liq_Status = table.Column<int>(nullable: false),
                    Liq_Created_Date = table.Column<DateTime>(nullable: false),
                    Liq_LastUpdated_Date = table.Column<DateTime>(nullable: false),
                    Liq_Created_UserID = table.Column<int>(nullable: false),
                    Liq_Verifier1 = table.Column<int>(nullable: false),
                    Liq_Verifier2 = table.Column<int>(nullable: false),
                    Liq_Approver = table.Column<int>(nullable: false),
                    ExpenseEntryModelExpense_ID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiquidationEntryDetails", x => x.Liq_DtlID);
                    table.ForeignKey(
                        name: "FK_LiquidationEntryDetails_ExpenseEntry_ExpenseEntryModelExpense_ID",
                        column: x => x.ExpenseEntryModelExpense_ID,
                        principalTable: "ExpenseEntry",
                        principalColumn: "Expense_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LiquidationEntryDetails_ExpenseEntryModelExpense_ID",
                table: "LiquidationEntryDetails",
                column: "ExpenseEntryModelExpense_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LiquidationEntryDetails");
        }
    }
}
