using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class Added_Grwite_TransList_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TL_ExpenseDtlID",
                table: "ExpenseTransLists",
                newName: "TL_GoExpHist_ID");

            migrationBuilder.CreateTable(
                name: "GwriteTransLists",
                columns: table => new
                {
                    GW_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    GW_GWrite_ID = table.Column<int>(nullable: false),
                    GW_TransID = table.Column<int>(nullable: false),
                    GW_Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GwriteTransLists", x => x.GW_ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GwriteTransLists");

            migrationBuilder.RenameColumn(
                name: "TL_GoExpHist_ID",
                table: "ExpenseTransLists",
                newName: "TL_ExpenseDtlID");
        }
    }
}
