using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class ExpenseEntryCashBreakdownModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpenseEntryCashBreakdown",
                columns: table => new
                {
                    CashBreak_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CashBreak_Denimination = table.Column<double>(nullable: false),
                    CashBreak_NoPcs = table.Column<float>(nullable: false),
                    CashBreak_Amount = table.Column<float>(nullable: false),
                    ExpenseEntryDetailModelExpDtl_ID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseEntryCashBreakdown", x => x.CashBreak_ID);
                    table.ForeignKey(
                        name: "FK_ExpenseEntryCashBreakdown_ExpenseEntryDetails_ExpenseEntryDetailModelExpDtl_ID",
                        column: x => x.ExpenseEntryDetailModelExpDtl_ID,
                        principalTable: "ExpenseEntryDetails",
                        principalColumn: "ExpDtl_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseEntryCashBreakdown_ExpenseEntryDetailModelExpDtl_ID",
                table: "ExpenseEntryCashBreakdown",
                column: "ExpenseEntryDetailModelExpDtl_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseEntryCashBreakdown");
        }
    }
}
