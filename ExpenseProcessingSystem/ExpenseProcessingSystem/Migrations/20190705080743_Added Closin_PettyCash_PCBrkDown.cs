using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class AddedClosin_PettyCash_PCBrkDown : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Closing",
                columns: table => new
                {
                    Close_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Close_Date = table.Column<DateTime>(nullable: false),
                    Close_Open_Date = table.Column<DateTime>(nullable: false),
                    Close_Status = table.Column<int>(nullable: false),
                    Close_User = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Closing", x => x.Close_ID);
                });

            migrationBuilder.CreateTable(
                name: "PettyCash",
                columns: table => new
                {
                    PC_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PC_StartBal = table.Column<double>(nullable: false),
                    PC_Disbursed = table.Column<double>(nullable: false),
                    PC_EndBal = table.Column<double>(nullable: false),
                    PC_CloseUser = table.Column<int>(nullable: false),
                    PC_OpenUser = table.Column<int>(nullable: false),
                    PC_OpenConfirm = table.Column<bool>(nullable: false),
                    PC_ConfirmComment = table.Column<string>(nullable: true),
                    PC_Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PettyCash", x => x.PC_ID);
                });

            migrationBuilder.CreateTable(
                name: "PettyCashBreakDown",
                columns: table => new
                {
                    PCB_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PCB_OneThousand = table.Column<int>(nullable: false),
                    PCB_FiveHundred = table.Column<int>(nullable: false),
                    PCB_TwoHundred = table.Column<int>(nullable: false),
                    PCB_OneHundred = table.Column<int>(nullable: false),
                    PCB_Fifty = table.Column<int>(nullable: false),
                    PCB_Twenty = table.Column<int>(nullable: false),
                    PCB_Ten = table.Column<int>(nullable: false),
                    PCB_Five = table.Column<int>(nullable: false),
                    PCB_One = table.Column<int>(nullable: false),
                    PCB_TwentyFiveCents = table.Column<int>(nullable: false),
                    PCB_TenCents = table.Column<int>(nullable: false),
                    PCB_FiveCents = table.Column<int>(nullable: false),
                    PCB_OneCents = table.Column<int>(nullable: false),
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Closing");

            migrationBuilder.DropTable(
                name: "PettyCashBreakDown");

            migrationBuilder.DropTable(
                name: "PettyCash");
        }
    }
}
