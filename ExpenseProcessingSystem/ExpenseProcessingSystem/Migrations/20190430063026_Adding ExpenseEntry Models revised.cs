using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class AddingExpenseEntryModelsrevised : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpDtl_Expense_ID",
                table: "ExpenseEntryDetails");

            migrationBuilder.CreateTable(
                name: "ExpenseEntryAmortizations",
                columns: table => new
                {
                    Amor_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amor_Sched_Date = table.Column<DateTime>(nullable: false),
                    Amor_Price = table.Column<float>(nullable: false),
                    ExpenseEntryDetailModelExpDtl_ID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseEntryAmortizations", x => x.Amor_ID);
                    table.ForeignKey(
                        name: "FK_ExpenseEntryAmortizations_ExpenseEntryDetails_ExpenseEntryDetailModelExpDtl_ID",
                        column: x => x.ExpenseEntryDetailModelExpDtl_ID,
                        principalTable: "ExpenseEntryDetails",
                        principalColumn: "ExpDtl_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseEntryGbaseDtls",
                columns: table => new
                {
                    GbaseDtl_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GbaseDtl_Document_Type = table.Column<string>(nullable: true),
                    GbaseDtl_InvoiceNo = table.Column<string>(nullable: true),
                    GbaseDtl_Description = table.Column<string>(nullable: true),
                    GabseDtl_Amount = table.Column<float>(nullable: false),
                    ExpenseEntryDetailModelExpDtl_ID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseEntryGbaseDtls", x => x.GbaseDtl_ID);
                    table.ForeignKey(
                        name: "FK_ExpenseEntryGbaseDtls_ExpenseEntryDetails_ExpenseEntryDetailModelExpDtl_ID",
                        column: x => x.ExpenseEntryDetailModelExpDtl_ID,
                        principalTable: "ExpenseEntryDetails",
                        principalColumn: "ExpDtl_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseEntryAmortizations_ExpenseEntryDetailModelExpDtl_ID",
                table: "ExpenseEntryAmortizations",
                column: "ExpenseEntryDetailModelExpDtl_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseEntryGbaseDtls_ExpenseEntryDetailModelExpDtl_ID",
                table: "ExpenseEntryGbaseDtls",
                column: "ExpenseEntryDetailModelExpDtl_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseEntryAmortizations");

            migrationBuilder.DropTable(
                name: "ExpenseEntryGbaseDtls");

            migrationBuilder.AddColumn<int>(
                name: "ExpDtl_Expense_ID",
                table: "ExpenseEntryDetails",
                nullable: false,
                defaultValue: 0);
        }
    }
}
