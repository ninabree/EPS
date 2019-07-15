using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class ApplyAnnotationsToLiquidationInterEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AlterColumn<int>(
                name: "Liq_CCY_3_2",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Liq_CCY_3_1",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Liq_CCY_2_2",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Liq_CCY_2_1",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Liq_CCY_1_2",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Liq_CCY_1_1",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Liq_AccountID_3_2",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Liq_AccountID_3_1",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Liq_AccountID_2_2",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Liq_AccountID_2_1",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Liq_AccountID_1_2",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Liq_AccountID_1_1",
                table: "LiquidationInterEntity",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AlterColumn<string>(
                name: "Liq_CCY_3_2",
                table: "LiquidationInterEntity",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Liq_CCY_3_1",
                table: "LiquidationInterEntity",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Liq_CCY_2_2",
                table: "LiquidationInterEntity",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Liq_CCY_2_1",
                table: "LiquidationInterEntity",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Liq_CCY_1_2",
                table: "LiquidationInterEntity",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Liq_CCY_1_1",
                table: "LiquidationInterEntity",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Liq_AccountID_3_2",
                table: "LiquidationInterEntity",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Liq_AccountID_3_1",
                table: "LiquidationInterEntity",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Liq_AccountID_2_2",
                table: "LiquidationInterEntity",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Liq_AccountID_2_1",
                table: "LiquidationInterEntity",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Liq_AccountID_1_2",
                table: "LiquidationInterEntity",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Liq_AccountID_1_1",
                table: "LiquidationInterEntity",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateTable(
                name: "PettyCashBreakDown",
                columns: table => new
                {
                    PCB_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PCB_Fifty = table.Column<int>(nullable: false),
                    PCB_Five = table.Column<int>(nullable: false),
                    PCB_FiveCents = table.Column<int>(nullable: false),
                    PCB_FiveHundred = table.Column<int>(nullable: false),
                    PCB_One = table.Column<int>(nullable: false),
                    PCB_OneCents = table.Column<int>(nullable: false),
                    PCB_OneHundred = table.Column<int>(nullable: false),
                    PCB_OneThousand = table.Column<int>(nullable: false),
                    PCB_Ten = table.Column<int>(nullable: false),
                    PCB_TenCents = table.Column<int>(nullable: false),
                    PCB_Twenty = table.Column<int>(nullable: false),
                    PCB_TwentyFiveCents = table.Column<int>(nullable: false),
                    PCB_TwoHundred = table.Column<int>(nullable: false),
                    PC_ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PettyCashBreakDown", x => x.PCB_ID);
                    table.ForeignKey(
                        name: "FK_PettyCashBreakDown_PettyCash_PC_ID",
                        column: x => x.PC_ID,
                        principalTable: "PettyCash",
                        principalColumn: "PC_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PettyCashBreakDown_PC_ID",
                table: "PettyCashBreakDown",
                column: "PC_ID");
        }
    }
}
