using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class ApplyAnnotationsToReversalEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReversalEntry",
                columns: table => new
                {
                    Reversal_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Reversal_ExpenseEntryID = table.Column<int>(nullable: false),
                    Reversal_ExpenseType = table.Column<int>(nullable: false),
                    Reversal_ExpenseDtlID = table.Column<int>(nullable: false),
                    Reversal_NonCashDtlID = table.Column<int>(nullable: false),
                    Reversal_LiqDtlID = table.Column<int>(nullable: false),
                    Reversal_LiqInterEntityID = table.Column<int>(nullable: false),
                    Reversal_GOExpressID = table.Column<int>(nullable: false),
                    Reversal_GOExpressHistID = table.Column<int>(nullable: false),
                    Reversal_TransNo = table.Column<int>(nullable: false),
                    Reversal_ReversedDate = table.Column<DateTime>(nullable: false),
                    Reversal_ReversedUserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReversalEntry", x => x.Reversal_ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReversalEntry");
        }
    }
}
