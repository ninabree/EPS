using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class UpdatedNotifModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SystemMessageModels",
                table: "SystemMessageModels");

            migrationBuilder.DropColumn(
                name: "Notif_Created_Date",
                table: "HomeNotif");

            migrationBuilder.DropColumn(
                name: "Notif_Status",
                table: "HomeNotif");

            migrationBuilder.DropColumn(
                name: "Notif_Type_Status",
                table: "HomeNotif");

            migrationBuilder.RenameColumn(
                name: "Notif_Verifr_Apprvr_ID",
                table: "HomeNotif",
                newName: "Notif_UserFor_ID");

            migrationBuilder.RenameColumn(
                name: "Notif_User_ID",
                table: "HomeNotif",
                newName: "Notif_Status_ID");

            migrationBuilder.RenameColumn(
                name: "Notif_Last_Updated",
                table: "HomeNotif",
                newName: "Notif_Date");

            migrationBuilder.RenameColumn(
                name: "Notif_Application_ID",
                table: "HomeNotif",
                newName: "Notif_Application_Type_ID");

            migrationBuilder.AlterColumn<string>(
                name: "Msg_Code",
                table: "SystemMessageModels",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "Msg_ID",
                table: "SystemMessageModels",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Notif_Application_Maker_ID",
                table: "HomeNotif",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SystemMessageModels",
                table: "SystemMessageModels",
                column: "Msg_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SystemMessageModels",
                table: "SystemMessageModels");

            migrationBuilder.DropColumn(
                name: "Msg_ID",
                table: "SystemMessageModels");

            migrationBuilder.DropColumn(
                name: "Notif_Application_Maker_ID",
                table: "HomeNotif");

            migrationBuilder.RenameColumn(
                name: "Notif_UserFor_ID",
                table: "HomeNotif",
                newName: "Notif_Verifr_Apprvr_ID");

            migrationBuilder.RenameColumn(
                name: "Notif_Status_ID",
                table: "HomeNotif",
                newName: "Notif_User_ID");

            migrationBuilder.RenameColumn(
                name: "Notif_Date",
                table: "HomeNotif",
                newName: "Notif_Last_Updated");

            migrationBuilder.RenameColumn(
                name: "Notif_Application_Type_ID",
                table: "HomeNotif",
                newName: "Notif_Application_ID");

            migrationBuilder.AlterColumn<string>(
                name: "Msg_Code",
                table: "SystemMessageModels",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Notif_Created_Date",
                table: "HomeNotif",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Notif_Status",
                table: "HomeNotif",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notif_Type_Status",
                table: "HomeNotif",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SystemMessageModels",
                table: "SystemMessageModels",
                column: "Msg_Code");
        }
    }
}
