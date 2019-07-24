using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class PrintStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Liq_VendorID",
                table: "LiquidationInterEntity",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ExpNCDtl_TR_ID",
                table: "ExpenseEntryNonCashDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExpNCDtl_Vendor_ID",
                table: "ExpenseEntryNonCashDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PrintStatus",
                columns: table => new
                {
                    PS_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PS_EntryID = table.Column<int>(nullable: false),
                    PS_EntryDtlID = table.Column<int>(nullable: false),
                    PS_Type = table.Column<int>(nullable: false),
                    PS_LOI = table.Column<bool>(nullable: false),
                    PS_BIR2307 = table.Column<bool>(nullable: false),
                    PS_CDD = table.Column<bool>(nullable: false),
                    PS_Check = table.Column<bool>(nullable: false),
                    PS_Voucher = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrintStatus", x => x.PS_ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrintStatus");

            migrationBuilder.DropColumn(
                name: "Liq_VendorID",
                table: "LiquidationInterEntity");

            migrationBuilder.DropColumn(
                name: "ExpNCDtl_TR_ID",
                table: "ExpenseEntryNonCashDetails");

            migrationBuilder.DropColumn(
                name: "ExpNCDtl_Vendor_ID",
                table: "ExpenseEntryNonCashDetails");
        }
    }
}
