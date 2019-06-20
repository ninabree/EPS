using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class LiquidationCashBreakdown : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LiquidationCashBreakdown",
                columns: table => new
                {
                    LiqCashBreak_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LiqCashBreak_Denimination = table.Column<double>(nullable: false),
                    LiqCashBreak_NoPcs = table.Column<int>(nullable: false),
                    LiqCashBreak_Amount = table.Column<double>(nullable: false),
                    ExpenseEntryDetailModelExpDtl_ID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiquidationCashBreakdown", x => x.LiqCashBreak_ID);
                    table.ForeignKey(
                        name: "FK_LiquidationCashBreakdown_ExpenseEntryDetails_ExpenseEntryDetailModelExpDtl_ID",
                        column: x => x.ExpenseEntryDetailModelExpDtl_ID,
                        principalTable: "ExpenseEntryDetails",
                        principalColumn: "ExpDtl_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LiquidationCashBreakdown_ExpenseEntryDetailModelExpDtl_ID",
                table: "LiquidationCashBreakdown",
                column: "ExpenseEntryDetailModelExpDtl_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LiquidationCashBreakdown");
        }
    }
}
