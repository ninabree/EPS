using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class AddedDebitCreditToNC : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "ExpNC_CS_CredAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ExpNC_CS_DebitAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ExpNC_CredAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ExpNC_DebitAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ExpNC_IE_CredAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ExpNC_IE_DebitAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpNC_CS_CredAmt",
                table: "ExpenseEntryNonCash");

            migrationBuilder.DropColumn(
                name: "ExpNC_CS_DebitAmt",
                table: "ExpenseEntryNonCash");

            migrationBuilder.DropColumn(
                name: "ExpNC_CredAmt",
                table: "ExpenseEntryNonCash");

            migrationBuilder.DropColumn(
                name: "ExpNC_DebitAmt",
                table: "ExpenseEntryNonCash");

            migrationBuilder.DropColumn(
                name: "ExpNC_IE_CredAmt",
                table: "ExpenseEntryNonCash");

            migrationBuilder.DropColumn(
                name: "ExpNC_IE_DebitAmt",
                table: "ExpenseEntryNonCash");
        }
    }
}
