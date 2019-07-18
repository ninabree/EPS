using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class Update_Closing_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PettyCashBreakDown");

            migrationBuilder.RenameColumn(
                name: "PC_Date",
                table: "PettyCash",
                newName: "PC_OpenDate");

            migrationBuilder.AddColumn<int>(
                name: "PCB_Fifty",
                table: "PettyCash",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PCB_Five",
                table: "PettyCash",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PCB_FiveCents",
                table: "PettyCash",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PCB_FiveHundred",
                table: "PettyCash",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PCB_ID",
                table: "PettyCash",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PCB_One",
                table: "PettyCash",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PCB_OneCents",
                table: "PettyCash",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PCB_OneHundred",
                table: "PettyCash",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PCB_OneThousand",
                table: "PettyCash",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PCB_Ten",
                table: "PettyCash",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PCB_TenCents",
                table: "PettyCash",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PCB_Twenty",
                table: "PettyCash",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PCB_TwentyFiveCents",
                table: "PettyCash",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PCB_TwoHundred",
                table: "PettyCash",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PC_CloseDate",
                table: "PettyCash",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Close_Date",
                table: "Closing",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<string>(
                name: "Close_Type",
                table: "Closing",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PCB_Fifty",
                table: "PettyCash");

            migrationBuilder.DropColumn(
                name: "PCB_Five",
                table: "PettyCash");

            migrationBuilder.DropColumn(
                name: "PCB_FiveCents",
                table: "PettyCash");

            migrationBuilder.DropColumn(
                name: "PCB_FiveHundred",
                table: "PettyCash");

            migrationBuilder.DropColumn(
                name: "PCB_ID",
                table: "PettyCash");

            migrationBuilder.DropColumn(
                name: "PCB_One",
                table: "PettyCash");

            migrationBuilder.DropColumn(
                name: "PCB_OneCents",
                table: "PettyCash");

            migrationBuilder.DropColumn(
                name: "PCB_OneHundred",
                table: "PettyCash");

            migrationBuilder.DropColumn(
                name: "PCB_OneThousand",
                table: "PettyCash");

            migrationBuilder.DropColumn(
                name: "PCB_Ten",
                table: "PettyCash");

            migrationBuilder.DropColumn(
                name: "PCB_TenCents",
                table: "PettyCash");

            migrationBuilder.DropColumn(
                name: "PCB_Twenty",
                table: "PettyCash");

            migrationBuilder.DropColumn(
                name: "PCB_TwentyFiveCents",
                table: "PettyCash");

            migrationBuilder.DropColumn(
                name: "PCB_TwoHundred",
                table: "PettyCash");

            migrationBuilder.DropColumn(
                name: "PC_CloseDate",
                table: "PettyCash");

            migrationBuilder.DropColumn(
                name: "Close_Type",
                table: "Closing");

            migrationBuilder.RenameColumn(
                name: "PC_OpenDate",
                table: "PettyCash",
                newName: "PC_Date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Close_Date",
                table: "Closing",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

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
