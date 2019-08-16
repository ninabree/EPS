using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class ApplyAnnotationsToLiqInterEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Liq_TaxRate",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Liq_TaxRate",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
