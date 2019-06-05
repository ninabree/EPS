using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class AddedAccountBudgetCodeToDMAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseEntryNonCashDetails_ExpenseEntry_ExpenseEntryNCModelExpense_ID",
                table: "ExpenseEntryNonCashDetails");

            migrationBuilder.DropIndex(
                name: "IX_ExpenseEntryNonCashDetails_ExpenseEntryNCModelExpense_ID",
                table: "ExpenseEntryNonCashDetails");

            migrationBuilder.DropColumn(
                name: "ExpenseEntryNCModelExpense_ID",
                table: "ExpenseEntryNonCashDetails");

            migrationBuilder.AddColumn<string>(
                name: "Pending_Account_Budget_Code",
                table: "DMAccount_Pending",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Account_Budget_Code",
                table: "DMAccount",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pending_Account_Budget_Code",
                table: "DMAccount_Pending");

            migrationBuilder.DropColumn(
                name: "Account_Budget_Code",
                table: "DMAccount");

            migrationBuilder.AddColumn<int>(
                name: "ExpenseEntryNCModelExpense_ID",
                table: "ExpenseEntryNonCashDetails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseEntryNonCashDetails_ExpenseEntryNCModelExpense_ID",
                table: "ExpenseEntryNonCashDetails",
                column: "ExpenseEntryNCModelExpense_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseEntryNonCashDetails_ExpenseEntry_ExpenseEntryNCModelExpense_ID",
                table: "ExpenseEntryNonCashDetails",
                column: "ExpenseEntryNCModelExpense_ID",
                principalTable: "ExpenseEntry",
                principalColumn: "Expense_ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
