using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class AddNonCashModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpenseEntryNonCash",
                columns: table => new
                {
                    ExpNC_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExpNC_Category_ID = table.Column<int>(nullable: false),
                    ExpenseEntryModelExpense_ID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseEntryNonCash", x => x.ExpNC_ID);
                    table.ForeignKey(
                        name: "FK_ExpenseEntryNonCash_ExpenseEntry_ExpenseEntryModelExpense_ID",
                        column: x => x.ExpenseEntryModelExpense_ID,
                        principalTable: "ExpenseEntry",
                        principalColumn: "Expense_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseEntryNonCashDetails",
                columns: table => new
                {
                    ExpNCDtl_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExpNCDtl_Remarks_Desc = table.Column<string>(nullable: true),
                    ExpNCDtl_Remarks_Period_From = table.Column<DateTime>(nullable: false),
                    ExpNCDtl_Remarks_Period_To = table.Column<DateTime>(nullable: false),
                    ExpenseEntryNCModelExpense_ID = table.Column<int>(nullable: true),
                    ExpenseEntryNCModelExpNC_ID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseEntryNonCashDetails", x => x.ExpNCDtl_ID);
                    table.ForeignKey(
                        name: "FK_ExpenseEntryNonCashDetails_ExpenseEntryNonCash_ExpenseEntryNCModelExpNC_ID",
                        column: x => x.ExpenseEntryNCModelExpNC_ID,
                        principalTable: "ExpenseEntryNonCash",
                        principalColumn: "ExpNC_ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExpenseEntryNonCashDetails_ExpenseEntry_ExpenseEntryNCModelExpense_ID",
                        column: x => x.ExpenseEntryNCModelExpense_ID,
                        principalTable: "ExpenseEntry",
                        principalColumn: "Expense_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseEntryNonCashDetailAccounts",
                columns: table => new
                {
                    ExpNCDtlAcc_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExpNCDtlAcc_Acc_ID = table.Column<int>(nullable: false),
                    ExpNCDtlAcc_Type_ID = table.Column<int>(nullable: false),
                    ExpNCDtlAcc_Curr_ID = table.Column<int>(nullable: false),
                    ExpNCDtlAcc_Acc_Name = table.Column<string>(nullable: true),
                    ExpNCDtlAcc_Inter_Rate = table.Column<float>(nullable: false),
                    ExpNCDtlAcc_Amount = table.Column<float>(nullable: false),
                    ExpenseEntryNCModelExpNCDtl_ID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseEntryNonCashDetailAccounts", x => x.ExpNCDtlAcc_ID);
                    table.ForeignKey(
                        name: "FK_ExpenseEntryNonCashDetailAccounts_ExpenseEntryNonCashDetails_ExpenseEntryNCModelExpNCDtl_ID",
                        column: x => x.ExpenseEntryNCModelExpNCDtl_ID,
                        principalTable: "ExpenseEntryNonCashDetails",
                        principalColumn: "ExpNCDtl_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseEntryNonCash_ExpenseEntryModelExpense_ID",
                table: "ExpenseEntryNonCash",
                column: "ExpenseEntryModelExpense_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseEntryNonCashDetailAccounts_ExpenseEntryNCModelExpNCDtl_ID",
                table: "ExpenseEntryNonCashDetailAccounts",
                column: "ExpenseEntryNCModelExpNCDtl_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseEntryNonCashDetails_ExpenseEntryNCModelExpNC_ID",
                table: "ExpenseEntryNonCashDetails",
                column: "ExpenseEntryNCModelExpNC_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseEntryNonCashDetails_ExpenseEntryNCModelExpense_ID",
                table: "ExpenseEntryNonCashDetails",
                column: "ExpenseEntryNCModelExpense_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseEntryNonCashDetailAccounts");

            migrationBuilder.DropTable(
                name: "ExpenseEntryNonCashDetails");

            migrationBuilder.DropTable(
                name: "ExpenseEntryNonCash");
        }
    }
}
