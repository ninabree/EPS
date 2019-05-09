using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class UpdateDM7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "Budget_Created_Date",
            //    table: "Budget");

            //migrationBuilder.DropColumn(
            //    name: "Budget_Creator_ID",
            //    table: "Budget");

            //migrationBuilder.RenameColumn(
            //    name: "Budget_Last_Updated",
            //    table: "Budget",
            //    newName: "Budget_Last_Approval_Date");

            //migrationBuilder.RenameColumn(
            //    name: "Budget_Acc_ID",
            //    table: "Budget",
            //    newName: "Acc_ID");

            migrationBuilder.AddColumn<int>(
                name: "AccountGroup_Approver_ID",
                table: "DMAccountGroup",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AccountGroup_Code",
                table: "DMAccountGroup",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AccountGroup_Created_Date",
                table: "DMAccountGroup",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "AccountGroup_Creator_ID",
                table: "DMAccountGroup",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "AccountGroup_Last_Updated",
                table: "DMAccountGroup",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AccountGroup_Status",
                table: "DMAccountGroup",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AccountGroup_isActive",
                table: "DMAccountGroup",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AccountGroup_isDeleted",
                table: "DMAccountGroup",
                nullable: false,
                defaultValue: false);

            //migrationBuilder.AlterColumn<byte>(
            //    name: "Budget_Status",
            //    table: "Budget",
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Budget_Approver_ID",
            //    table: "Budget",
            //    nullable: true,
            //    oldClrType: typeof(int));

            //migrationBuilder.AlterColumn<double>(
            //    name: "Budget_Amount",
            //    table: "Budget",
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldNullable: true);

            //migrationBuilder.AddColumn<double>(
            //    name: "Budget_Current",
            //    table: "Budget",
            //    nullable: false,
            //    defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "DMAccountGroup_Pending",
                columns: table => new
                {
                    Pending_AccountGroup_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pending_AccountGroup_MasterID = table.Column<int>(nullable: false),
                    Pending_AccountGroup_Name = table.Column<string>(nullable: true),
                    Pending_AccountGroup_Code = table.Column<string>(nullable: true),
                    Pending_AccountGroup_Creator_ID = table.Column<int>(nullable: false),
                    Pending_AccountGroup_Approver_ID = table.Column<int>(nullable: false),
                    Pending_AccountGroup_Filed_Date = table.Column<DateTime>(nullable: false),
                    Pending_AccountGroup_Status = table.Column<string>(nullable: true),
                    Pending_AccountGroup_isDeleted = table.Column<bool>(nullable: false),
                    Pending_AccountGroup_isActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMAccountGroup_Pending", x => x.Pending_AccountGroup_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMVendorTRVAT_Pending",
                columns: table => new
                {
                    Pending_VTV_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Pending_VTV_Vendor_ID = table.Column<int>(nullable: false),
                    Pending_VTV_TR_ID = table.Column<int>(nullable: false),
                    Pending_VTV_VAT_ID = table.Column<int>(nullable: false),
                    Pending_VTV_Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMVendorTRVAT_Pending", x => x.Pending_VTV_ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DMAccountGroup_Pending");

            migrationBuilder.DropTable(
                name: "DMVendorTRVAT_Pending");

            migrationBuilder.DropColumn(
                name: "AccountGroup_Approver_ID",
                table: "DMAccountGroup");

            migrationBuilder.DropColumn(
                name: "AccountGroup_Code",
                table: "DMAccountGroup");

            migrationBuilder.DropColumn(
                name: "AccountGroup_Created_Date",
                table: "DMAccountGroup");

            migrationBuilder.DropColumn(
                name: "AccountGroup_Creator_ID",
                table: "DMAccountGroup");

            migrationBuilder.DropColumn(
                name: "AccountGroup_Last_Updated",
                table: "DMAccountGroup");

            migrationBuilder.DropColumn(
                name: "AccountGroup_Status",
                table: "DMAccountGroup");

            migrationBuilder.DropColumn(
                name: "AccountGroup_isActive",
                table: "DMAccountGroup");

            migrationBuilder.DropColumn(
                name: "AccountGroup_isDeleted",
                table: "DMAccountGroup");

            migrationBuilder.DropColumn(
                name: "Budget_Current",
                table: "Budget");

            migrationBuilder.RenameColumn(
                name: "Budget_Last_Approval_Date",
                table: "Budget",
                newName: "Budget_Last_Updated");

            migrationBuilder.RenameColumn(
                name: "Acc_ID",
                table: "Budget",
                newName: "Budget_Acc_ID");

            migrationBuilder.AlterColumn<string>(
                name: "Budget_Status",
                table: "Budget",
                nullable: true,
                oldClrType: typeof(byte));

            migrationBuilder.AlterColumn<int>(
                name: "Budget_Approver_ID",
                table: "Budget",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Budget_Amount",
                table: "Budget",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<DateTime>(
                name: "Budget_Created_Date",
                table: "Budget",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Budget_Creator_ID",
                table: "Budget",
                nullable: false,
                defaultValue: 0);
        }
    }
}
