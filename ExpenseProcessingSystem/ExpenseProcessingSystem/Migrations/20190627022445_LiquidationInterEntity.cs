using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class LiquidationInterEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LiquidationInterEntity",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Liq_DebitCred_1_1 = table.Column<string>(nullable: true),
                    Liq_AccountID_1_1 = table.Column<string>(nullable: true),
                    Liq_InterRate_1_1 = table.Column<double>(nullable: false),
                    Liq_CCY_1_1 = table.Column<string>(nullable: true),
                    Liq_Amount_1_1 = table.Column<double>(nullable: false),
                    Liq_DebitCred_1_2 = table.Column<string>(nullable: true),
                    Liq_AccountID_1_2 = table.Column<string>(nullable: true),
                    Liq_InterRate_1_2 = table.Column<double>(nullable: false),
                    Liq_CCY_1_2 = table.Column<string>(nullable: true),
                    Liq_Amount_1_2 = table.Column<double>(nullable: false),
                    Liq_DebitCred_2_1 = table.Column<string>(nullable: true),
                    Liq_AccountID_2_1 = table.Column<string>(nullable: true),
                    Liq_InterRate_2_1 = table.Column<double>(nullable: false),
                    Liq_CCY_2_1 = table.Column<string>(nullable: true),
                    Liq_Amount_2_1 = table.Column<double>(nullable: false),
                    Liq_DebitCred_2_2 = table.Column<string>(nullable: true),
                    Liq_AccountID_2_2 = table.Column<string>(nullable: true),
                    Liq_InterRate_2_2 = table.Column<double>(nullable: false),
                    Liq_CCY_2_2 = table.Column<string>(nullable: true),
                    Liq_Amount_2_2 = table.Column<double>(nullable: false),
                    LiquidationEntryDetailModelLiq_DtlID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LiquidationInterEntity", x => x.id);
                    table.ForeignKey(
                        name: "FK_LiquidationInterEntity_LiquidationEntryDetails_LiquidationEntryDetailModelLiq_DtlID",
                        column: x => x.LiquidationEntryDetailModelLiq_DtlID,
                        principalTable: "LiquidationEntryDetails",
                        principalColumn: "Liq_DtlID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LiquidationInterEntity_LiquidationEntryDetailModelLiq_DtlID",
                table: "LiquidationInterEntity",
                column: "LiquidationEntryDetailModelLiq_DtlID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LiquidationInterEntity");
        }
    }
}
