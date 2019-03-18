using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class AddPendingPayeeTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DMPayee_Pending",
                columns: table => new
                {
                    Pending_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pending_Payee_ID = table.Column<int>(nullable: false),
                    Pending_Payee_Name = table.Column<string>(nullable: true),
                    Pending_Payee_TIN = table.Column<string>(nullable: true),
                    Pending_Payee_Address = table.Column<string>(nullable: true),
                    Pending_Payee_Type = table.Column<string>(nullable: true),
                    Pending_Payee_No = table.Column<int>(nullable: false),
                    Pending_Payee_Creator_ID = table.Column<int>(nullable: false),
                    Pending_Payee_Filed_Date = table.Column<DateTime>(nullable: false),
                    Pending_Payee_Status = table.Column<string>(nullable: true),
                    Payee_ID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMPayee_Pending", x => x.Pending_ID);
                    table.ForeignKey(
                        name: "FK_DMPayee_Pending_DMPayee_Payee_ID",
                        column: x => x.Payee_ID,
                        principalTable: "DMPayee",
                        principalColumn: "Payee_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DMPayee_Pending_Payee_ID",
                table: "DMPayee_Pending",
                column: "Payee_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DMPayee_Pending");
        }
    }
}
