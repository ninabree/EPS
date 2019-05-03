using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class UpdateDM5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Pending_VAT_Rate",
                table: "DMVAT_Pending",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "VAT_Rate",
                table: "DMVAT",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "Pending_TR_Tax_Rate",
                table: "DMTR_Pending",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<float>(
                name: "TR_Tax_Rate",
                table: "DMTR",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<float>(
                name: "Pending_FBT_Tax_Rate",
                table: "DMFBT_Pending",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<float>(
                name: "FBT_Tax_Rate",
                table: "DMFBT",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Pending_VAT_Rate",
                table: "DMVAT_Pending",
                nullable: true,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<string>(
                name: "VAT_Rate",
                table: "DMVAT",
                nullable: true,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<int>(
                name: "Pending_TR_Tax_Rate",
                table: "DMTR_Pending",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<int>(
                name: "TR_Tax_Rate",
                table: "DMTR",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<int>(
                name: "Pending_FBT_Tax_Rate",
                table: "DMFBT_Pending",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<int>(
                name: "FBT_Tax_Rate",
                table: "DMFBT",
                nullable: false,
                oldClrType: typeof(float));
        }
    }
}
