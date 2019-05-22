using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class AddInterEntityModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.RenameColumn(
            //    name: "AccountGroup_MasterID",
            //    table: "Budget",
            //    newName: "Budget_AccountGroup_MasterID");

            migrationBuilder.AddColumn<string>(
                name: "ExpDtl_Ewt_Payor_Name",
                table: "ExpenseEntryDetails",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ExpDtl_Inter_Entity",
                table: "ExpenseEntryDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ExpenseEntryInterEntity",
                columns: table => new
                {
                    Inter_ID = table.Column<string>(nullable: false),
                    Inter_Particular_Title = table.Column<string>(nullable: true),
                    Inter_Currency1_ABBR = table.Column<string>(nullable: true),
                    Inter_Currency2_ABBR = table.Column<string>(nullable: true),
                    Inter_Currency1_Amount = table.Column<string>(nullable: true),
                    Inter_Currency2_Amount = table.Column<string>(nullable: true),
                    Inter_Rate = table.Column<string>(nullable: true),
                    ExpenseEntryDetailModelExpDtl_ID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseEntryInterEntity", x => x.Inter_ID);
                    table.ForeignKey(
                        name: "FK_ExpenseEntryInterEntity_ExpenseEntryDetails_ExpenseEntryDetailModelExpDtl_ID",
                        column: x => x.ExpenseEntryDetailModelExpDtl_ID,
                        principalTable: "ExpenseEntryDetails",
                        principalColumn: "ExpDtl_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseEntryInterEntity_ExpenseEntryDetailModelExpDtl_ID",
                table: "ExpenseEntryInterEntity",
                column: "ExpenseEntryDetailModelExpDtl_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseEntryInterEntity");

            migrationBuilder.DropColumn(
                name: "ExpDtl_Ewt_Payor_Name",
                table: "ExpenseEntryDetails");

            migrationBuilder.DropColumn(
                name: "ExpDtl_Inter_Entity",
                table: "ExpenseEntryDetails");

            migrationBuilder.RenameColumn(
                name: "Budget_AccountGroup_MasterID",
                table: "Budget",
                newName: "AccountGroup_MasterID");
        }
    }
}
