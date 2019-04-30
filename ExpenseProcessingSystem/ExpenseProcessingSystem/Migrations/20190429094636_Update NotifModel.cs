using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class UpdateNotifModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeNotif_Pending");

            migrationBuilder.RenameColumn(
                name: "Vendor_Last_Updated",
                table: "HomeNotif",
                newName: "Notif_Last_Updated");

            migrationBuilder.RenameColumn(
                name: "Vendor_Created_Date",
                table: "HomeNotif",
                newName: "Notif_Created_Date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Notif_Last_Updated",
                table: "HomeNotif",
                newName: "Vendor_Last_Updated");

            migrationBuilder.RenameColumn(
                name: "Notif_Created_Date",
                table: "HomeNotif",
                newName: "Vendor_Created_Date");

            migrationBuilder.CreateTable(
                name: "HomeNotif_Pending",
                columns: table => new
                {
                    Notif_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Notif_Application_ID = table.Column<int>(nullable: false),
                    Notif_Date_Filed = table.Column<DateTime>(nullable: false),
                    Notif_Message = table.Column<string>(nullable: true),
                    Notif_Status = table.Column<bool>(nullable: false),
                    Notif_Type_Status = table.Column<string>(nullable: true),
                    Notif_User_ID = table.Column<int>(nullable: false),
                    Notif_Verifr_Apprvr_ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeNotif_Pending", x => x.Notif_ID);
                });
        }
    }
}
