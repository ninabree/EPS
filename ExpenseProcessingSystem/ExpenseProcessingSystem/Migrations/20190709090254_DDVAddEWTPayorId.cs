using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class DDVAddEWTPayorId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpDtl_Ewt_Payor_Name",
                table: "ExpenseEntryDetails");

          
            migrationBuilder.AddColumn<int>(
                name: "ExpDtl_Ewt_Payor_Name_ID",
                table: "ExpenseEntryDetails",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpDtl_Ewt_Payor_Name_ID",
                table: "ExpenseEntryDetails");

            migrationBuilder.AddColumn<string>(
                name: "ExpDtl_Ewt_Payor_Name",
                table: "ExpenseEntryDetails",
                nullable: true);

            
        }
    }
}
