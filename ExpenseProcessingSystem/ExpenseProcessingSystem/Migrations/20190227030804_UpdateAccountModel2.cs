using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class UpdateAccountModel2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Acc_Approver_ID",
                table: "Account",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Acc_Created_Date",
                table: "Account",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Acc_Creator_ID",
                table: "Account",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Acc_Last_Updated",
                table: "Account",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Acc_Status",
                table: "Account",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Acc_Approver_ID",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Acc_Created_Date",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Acc_Creator_ID",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Acc_Last_Updated",
                table: "Account");

            migrationBuilder.DropColumn(
                name: "Acc_Status",
                table: "Account");
        }
    }
}
