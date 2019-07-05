using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class AddingExpenseEntryModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpenseEntry",
                columns: table => new
                {
                    Expense_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Expense_Date = table.Column<DateTime>(nullable: false),
                    Expense_Number = table.Column<int>(nullable: false),
                    Expense_CheckId = table.Column<int>(nullable: false),
                    Expense_CheckNo = table.Column<string>(nullable: true),
                    Expense_Payee = table.Column<int>(nullable: false),
                    Expense_Debit_Total = table.Column<float>(nullable: false),
                    Expense_Credit_Total = table.Column<float>(nullable: false),
                    Expense_Verifier = table.Column<int>(nullable: false),
                    Expense_Approver = table.Column<int>(nullable: false),
                    Expense_Status = table.Column<int>(nullable: false),
                    Expense_Creator_ID = table.Column<int>(nullable: false),
                    Expense_Created_Date = table.Column<DateTime>(nullable: false),
                    Expense_Last_Updated = table.Column<DateTime>(nullable: false),
                    Expense_isDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseEntry", x => x.Expense_ID);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseEntryDetails",
                columns: table => new
                {
                    ExpDtl_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExpDtl_Expense_ID = table.Column<int>(nullable: false),
                    ExpDtl_Gbase_Remarks = table.Column<string>(nullable: true),
                    ExpDtl_Account = table.Column<int>(nullable: false),
                    ExpDtl_Fbt = table.Column<bool>(nullable: false),
                    ExpDtl_Dept = table.Column<int>(nullable: false),
                    ExpDtl_Vat = table.Column<int>(nullable: false),
                    ExpDtl_Ewt = table.Column<int>(nullable: false),
                    ExpDt_Ccy = table.Column<int>(nullable: false),
                    ExpDtl_Debit = table.Column<float>(nullable: false),
                    ExpDtl_Credit_Ewt = table.Column<float>(nullable: false),
                    ExpDtl_Credit_Cash = table.Column<float>(nullable: false),
                    ExpDtl_Amor_Month = table.Column<int>(nullable: false),
                    ExpDtl_Amor_Day = table.Column<int>(nullable: false),
                    ExpDtl_Amor_Duration = table.Column<int>(nullable: false),
                    ExpenseEntryModelExpense_ID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseEntryDetails", x => x.ExpDtl_ID);
                    table.ForeignKey(
                        name: "FK_ExpenseEntryDetails_ExpenseEntry_ExpenseEntryModelExpense_ID",
                        column: x => x.ExpenseEntryModelExpense_ID,
                        principalTable: "ExpenseEntry",
                        principalColumn: "Expense_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseEntryDetails_ExpenseEntryModelExpense_ID",
                table: "ExpenseEntryDetails",
                column: "ExpenseEntryModelExpense_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseEntryDetails");

            migrationBuilder.DropTable(
                name: "ExpenseEntry");
        }
    }
}
