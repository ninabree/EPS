using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class AddedGbaseErrorDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GbaseErrorCodes",
                columns: table => new
                {
                    GERR_ID = table.Column<string>(nullable: false),
                    GERR_NO = table.Column<string>(nullable: true),
                    GERR_STATUS = table.Column<string>(nullable: true),
                    GERR_MESSAGE = table.Column<string>(nullable: true),
                    GERR_COMMENT = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GbaseErrorCodes", x => x.GERR_ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GbaseErrorCodes");
        }
    }
}
