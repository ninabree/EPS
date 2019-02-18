using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class AddAccountTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Acc_UserID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Acc_UserName = table.Column<string>(nullable: true),
                    Acc_Password = table.Column<string>(nullable: true),
                    Acc_DeptID = table.Column<int>(nullable: false),
                    Acc_Email = table.Column<string>(nullable: true),
                    Acc_Role = table.Column<string>(nullable: true),
                    Acc_Comment = table.Column<string>(nullable: true),
                    Acc_InUse = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.Acc_UserID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
