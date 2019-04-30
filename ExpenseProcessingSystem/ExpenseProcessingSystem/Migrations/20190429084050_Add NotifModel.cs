using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class AddNotifModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notif");

            migrationBuilder.CreateTable(
                name: "HomeNotif",
                columns: table => new
                {
                    Notif_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Notif_Application_ID = table.Column<int>(nullable: false),
                    Notif_User_ID = table.Column<int>(nullable: false),
                    Notif_Verifr_Apprvr_ID = table.Column<int>(nullable: false),
                    Notif_Message = table.Column<string>(nullable: true),
                    Vendor_Created_Date = table.Column<DateTime>(nullable: false),
                    Vendor_Last_Updated = table.Column<DateTime>(nullable: false),
                    Notif_Status = table.Column<bool>(nullable: false),
                    Notif_Type_Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeNotif", x => x.Notif_ID);
                });

            migrationBuilder.CreateTable(
                name: "HomeNotif_Pending",
                columns: table => new
                {
                    Notif_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Notif_Application_ID = table.Column<int>(nullable: false),
                    Notif_User_ID = table.Column<int>(nullable: false),
                    Notif_Verifr_Apprvr_ID = table.Column<int>(nullable: false),
                    Notif_Message = table.Column<string>(nullable: true),
                    Notif_Date_Filed = table.Column<DateTime>(nullable: false),
                    Notif_Status = table.Column<bool>(nullable: false),
                    Notif_Type_Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HomeNotif_Pending", x => x.Notif_ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HomeNotif");

            migrationBuilder.DropTable(
                name: "HomeNotif_Pending");

            migrationBuilder.CreateTable(
                name: "Notif",
                columns: table => new
                {
                    Notif_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Notif_Application_ID = table.Column<int>(nullable: false),
                    Notif_Apprvr_ID = table.Column<int>(nullable: false),
                    Notif_Date = table.Column<DateTime>(nullable: false),
                    Notif_Link_Address = table.Column<string>(nullable: true),
                    Notif_Message = table.Column<string>(nullable: true),
                    Notif_Status = table.Column<bool>(nullable: false),
                    Notif_Type_Screen = table.Column<string>(nullable: true),
                    Notif_Type_Status = table.Column<string>(nullable: true),
                    Notif_User_ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notif", x => x.Notif_ID);
                });
        }
    }
}
