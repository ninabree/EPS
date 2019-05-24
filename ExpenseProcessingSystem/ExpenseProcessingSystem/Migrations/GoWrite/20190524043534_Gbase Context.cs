using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations.GoWrite
{
    public partial class GbaseContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblRequest_Details",
                columns: table => new
                {
                    RequestID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RacfID = table.Column<string>(nullable: true),
                    RacfPassword = table.Column<string>(nullable: true),
                    RequestCreated = table.Column<DateTime>(nullable: false),
                    ReturnMessage = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    StatusDate = table.Column<DateTime>(nullable: false),
                    SystemAbbr = table.Column<string>(nullable: true),
                    Priority = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblRequest_Details", x => x.RequestID);
                });

            migrationBuilder.CreateTable(
                name: "tblRequest_Item",
                columns: table => new
                {
                    ItemID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SequenceNo = table.Column<int>(nullable: false),
                    ReturnFlag = table.Column<bool>(nullable: false),
                    Command = table.Column<string>(nullable: true),
                    ScreenCapture = table.Column<string>(nullable: true),
                    RequestID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblRequest_Item", x => x.ItemID);
                    table.ForeignKey(
                        name: "FK_tblRequest_Item_tblRequest_Details_RequestID",
                        column: x => x.RequestID,
                        principalTable: "tblRequest_Details",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tblRequest_Item_RequestID",
                table: "tblRequest_Item",
                column: "RequestID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblRequest_Item");

            migrationBuilder.DropTable(
                name: "tblRequest_Details");
        }
    }
}
