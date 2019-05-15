using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class UpdateDM6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DMAccountGroup",
                columns: table => new
                {
                    AccountGroup_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountGroup_MasterID = table.Column<int>(nullable: false),
                    AccountGroup_Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMAccountGroup", x => x.AccountGroup_ID);
                });

            migrationBuilder.CreateTable(
                name: "DMVendorTRVAT",
                columns: table => new
                {
                    VTV_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VTV_Vendor_ID = table.Column<int>(nullable: false),
                    VTV_TR_ID = table.Column<int>(nullable: false),
                    VTV_VAT_ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMVendorTRVAT", x => x.VTV_ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DMAccountGroup");

            migrationBuilder.DropTable(
                name: "DMVendorTRVAT");
        }
    }
}
