using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class NetworkDBUpdateDataMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DMPayee_Pending_DMPayee_Payee_ID",
                table: "DMPayee_Pending");

            migrationBuilder.DropIndex(
                name: "IX_DMPayee_Pending_Payee_ID",
                table: "DMPayee_Pending");

            migrationBuilder.DropColumn(
                name: "Payee_ID",
                table: "DMPayee_Pending");

            migrationBuilder.RenameColumn(
                name: "Pending_Payee_ID",
                table: "DMPayee_Pending",
                newName: "Pending_Payee_MasterID");

            migrationBuilder.AddColumn<int>(
                name: "Payee_MasterID",
                table: "DMPayee",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Payee_MasterID",
                table: "DMPayee");

            migrationBuilder.RenameColumn(
                name: "Pending_Payee_MasterID",
                table: "DMPayee_Pending",
                newName: "Pending_Payee_ID");

            migrationBuilder.AddColumn<int>(
                name: "Payee_ID",
                table: "DMPayee_Pending",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DMPayee_Pending_Payee_ID",
                table: "DMPayee_Pending",
                column: "Payee_ID");

            migrationBuilder.AddForeignKey(
                name: "FK_DMPayee_Pending_DMPayee_Payee_ID",
                table: "DMPayee_Pending",
                column: "Payee_ID",
                principalTable: "DMPayee",
                principalColumn: "Payee_ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
