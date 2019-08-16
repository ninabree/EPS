using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class Converteddoubletodecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PC_Recieved",
                table: "PettyCash",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "PC_Disbursed",
                table: "PettyCash",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Liq_TaxRate",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Liq_InterRate_3_2",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Liq_InterRate_3_1",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Liq_InterRate_2_2",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Liq_InterRate_2_1",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Liq_InterRate_1_2",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Liq_InterRate_1_1",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Liq_Amount_3_2",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Liq_Amount_3_1",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Liq_Amount_2_2",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Liq_Amount_2_1",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Liq_Amount_1_2",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Liq_Amount_1_1",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "LiqCashBreak_Denomination",
                table: "LiquidationCashBreakdown",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpNCDtlAcc_Inter_Rate",
                table: "ExpenseEntryNonCashDetailAccounts",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpNCDtlAcc_Amount",
                table: "ExpenseEntryNonCashDetailAccounts",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpNC_IE_DebitAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpNC_IE_CredAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpNC_DebitAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpNC_CredAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpNC_CS_DebitAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpNC_CS_CredAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "InterAcc_Rate",
                table: "ExpenseEntryInterEntityAccs",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "InterAcc_Amount",
                table: "ExpenseEntryInterEntityAccs",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpDtl_DDVInter_Conv_Amount2",
                table: "ExpenseEntryInterEntity",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpDtl_DDVInter_Conv_Amount1",
                table: "ExpenseEntryInterEntity",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpDtl_DDVInter_Amount2",
                table: "ExpenseEntryInterEntity",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpDtl_DDVInter_Amount1",
                table: "ExpenseEntryInterEntity",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "GbaseDtl_Amount",
                table: "ExpenseEntryGbaseDtls",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpDtl_Debit",
                table: "ExpenseEntryDetails",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpDtl_Credit_Ewt",
                table: "ExpenseEntryDetails",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "ExpDtl_Credit_Cash",
                table: "ExpenseEntryDetails",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "CashBreak_Denomination",
                table: "ExpenseEntryCashBreakdown",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Amor_Price",
                table: "ExpenseEntryAmortizations",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "Expense_Debit_Total",
                table: "ExpenseEntry",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "Expense_Credit_Total",
                table: "ExpenseEntry",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<decimal>(
                name: "Budget_New_Amount",
                table: "Budget",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Budget_Amount",
                table: "Budget",
                nullable: false,
                oldClrType: typeof(double));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "PC_Recieved",
                table: "PettyCash",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "PC_Disbursed",
                table: "PettyCash",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Liq_TaxRate",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Liq_InterRate_3_2",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Liq_InterRate_3_1",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Liq_InterRate_2_2",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Liq_InterRate_2_1",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Liq_InterRate_1_2",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Liq_InterRate_1_1",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Liq_Amount_3_2",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Liq_Amount_3_1",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Liq_Amount_2_2",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Liq_Amount_2_1",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Liq_Amount_1_2",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Liq_Amount_1_1",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "LiqCashBreak_Denomination",
                table: "LiquidationCashBreakdown",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "ExpNCDtlAcc_Inter_Rate",
                table: "ExpenseEntryNonCashDetailAccounts",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "ExpNCDtlAcc_Amount",
                table: "ExpenseEntryNonCashDetailAccounts",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "ExpNC_IE_DebitAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "ExpNC_IE_CredAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "ExpNC_DebitAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "ExpNC_CredAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "ExpNC_CS_DebitAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "ExpNC_CS_CredAmt",
                table: "ExpenseEntryNonCash",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "InterAcc_Rate",
                table: "ExpenseEntryInterEntityAccs",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "InterAcc_Amount",
                table: "ExpenseEntryInterEntityAccs",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "ExpDtl_DDVInter_Conv_Amount2",
                table: "ExpenseEntryInterEntity",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "ExpDtl_DDVInter_Conv_Amount1",
                table: "ExpenseEntryInterEntity",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "ExpDtl_DDVInter_Amount2",
                table: "ExpenseEntryInterEntity",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "ExpDtl_DDVInter_Amount1",
                table: "ExpenseEntryInterEntity",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "GbaseDtl_Amount",
                table: "ExpenseEntryGbaseDtls",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "ExpDtl_Debit",
                table: "ExpenseEntryDetails",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "ExpDtl_Credit_Ewt",
                table: "ExpenseEntryDetails",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "ExpDtl_Credit_Cash",
                table: "ExpenseEntryDetails",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "CashBreak_Denomination",
                table: "ExpenseEntryCashBreakdown",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "Amor_Price",
                table: "ExpenseEntryAmortizations",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "Expense_Debit_Total",
                table: "ExpenseEntry",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "Expense_Credit_Total",
                table: "ExpenseEntry",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Budget_New_Amount",
                table: "Budget",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Budget_Amount",
                table: "Budget",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
