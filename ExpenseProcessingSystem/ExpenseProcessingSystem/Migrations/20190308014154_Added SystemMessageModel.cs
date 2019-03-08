using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class AddedSystemMessageModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemMessageModels",
                columns: table => new
                {
                    Msg_Code = table.Column<string>(nullable: false),
                    Msg_Type = table.Column<string>(nullable: true),
                    Msg_Content = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemMessageModels", x => x.Msg_Code);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemMessageModels");
        }
    }
}
