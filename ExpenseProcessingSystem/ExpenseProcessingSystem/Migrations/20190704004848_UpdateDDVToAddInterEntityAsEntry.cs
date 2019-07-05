using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class UpdateDDVToAddInterEntityAsEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpenseEntryInterEntity",
                table: "ExpenseEntryInterEntity");

            migrationBuilder.DropColumn(
                name: "Inter_ID",
                table: "ExpenseEntryInterEntity");

            migrationBuilder.DropColumn(
                name: "Inter_Currency1_ABBR",
                table: "ExpenseEntryInterEntity");

            migrationBuilder.DropColumn(
                name: "Inter_Currency1_Amount",
                table: "ExpenseEntryInterEntity");

            migrationBuilder.DropColumn(
                name: "Inter_Currency2_ABBR",
                table: "ExpenseEntryInterEntity");

            migrationBuilder.DropColumn(
                name: "Inter_Currency2_Amount",
                table: "ExpenseEntryInterEntity");

            migrationBuilder.DropColumn(
                name: "Inter_Particular_Title",
                table: "ExpenseEntryInterEntity");

            migrationBuilder.DropColumn(
                name: "Inter_Rate",
                table: "ExpenseEntryInterEntity");

            migrationBuilder.AddColumn<int>(
                name: "ExpDtl_DDVInter_ID",
                table: "ExpenseEntryInterEntity",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<float>(
                name: "ExpDtl_DDVInter_Amount1",
                table: "ExpenseEntryInterEntity",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ExpDtl_DDVInter_Amount2",
                table: "ExpenseEntryInterEntity",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<bool>(
                name: "ExpDtl_DDVInter_Check1",
                table: "ExpenseEntryInterEntity",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ExpDtl_DDVInter_Check2",
                table: "ExpenseEntryInterEntity",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<float>(
                name: "ExpDtl_DDVInter_Conv_Amount1",
                table: "ExpenseEntryInterEntity",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "ExpDtl_DDVInter_Conv_Amount2",
                table: "ExpenseEntryInterEntity",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "ExpDtl_DDVInter_Curr1_ID",
                table: "ExpenseEntryInterEntity",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExpDtl_DDVInter_Curr2_ID",
                table: "ExpenseEntryInterEntity",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "ExpDtl_DDVInter_Rate",
                table: "ExpenseEntryInterEntity",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "ExpenseEntryAmortizations",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpenseEntryInterEntity",
                table: "ExpenseEntryInterEntity",
                column: "ExpDtl_DDVInter_ID");

            migrationBuilder.CreateTable(
                name: "ExpenseEntryInterEntityParticular",
                columns: table => new
                {
                    InterPart_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InterPart_Particular_Title = table.Column<string>(nullable: true),
                    ExpenseEntryInterEntityModelExpDtl_DDVInter_ID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseEntryInterEntityParticular", x => x.InterPart_ID);
                    table.ForeignKey(
                        name: "FK_ExpenseEntryInterEntityParticular_ExpenseEntryInterEntity_ExpenseEntryInterEntityModelExpDtl_DDVInter_ID",
                        column: x => x.ExpenseEntryInterEntityModelExpDtl_DDVInter_ID,
                        principalTable: "ExpenseEntryInterEntity",
                        principalColumn: "ExpDtl_DDVInter_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseEntryInterEntityAccs",
                columns: table => new
                {
                    InterAcc_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InterAcc_Acc_ID = table.Column<int>(nullable: false),
                    InterAcc_Curr_ID = table.Column<int>(nullable: false),
                    InterAcc_Amount = table.Column<float>(nullable: false),
                    InterAcc_Rate = table.Column<float>(nullable: false),
                    InterAcc_Type_ID = table.Column<int>(nullable: false),
                    ExpenseEntryInterEntityParticularInterPart_ID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseEntryInterEntityAccs", x => x.InterAcc_ID);
                    table.ForeignKey(
                        name: "FK_ExpenseEntryInterEntityAccs_ExpenseEntryInterEntityParticular_ExpenseEntryInterEntityParticularInterPart_ID",
                        column: x => x.ExpenseEntryInterEntityParticularInterPart_ID,
                        principalTable: "ExpenseEntryInterEntityParticular",
                        principalColumn: "InterPart_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseEntryInterEntityAccs_ExpenseEntryInterEntityParticularInterPart_ID",
                table: "ExpenseEntryInterEntityAccs",
                column: "ExpenseEntryInterEntityParticularInterPart_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseEntryInterEntityParticular_ExpenseEntryInterEntityModelExpDtl_DDVInter_ID",
                table: "ExpenseEntryInterEntityParticular",
                column: "ExpenseEntryInterEntityModelExpDtl_DDVInter_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseEntryInterEntityAccs");

            migrationBuilder.DropTable(
                name: "ExpenseEntryInterEntityParticular");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExpenseEntryInterEntity",
                table: "ExpenseEntryInterEntity");

            migrationBuilder.DropColumn(
                name: "ExpDtl_DDVInter_ID",
                table: "ExpenseEntryInterEntity");

            migrationBuilder.DropColumn(
                name: "ExpDtl_DDVInter_Amount1",
                table: "ExpenseEntryInterEntity");

            migrationBuilder.DropColumn(
                name: "ExpDtl_DDVInter_Amount2",
                table: "ExpenseEntryInterEntity");

            migrationBuilder.DropColumn(
                name: "ExpDtl_DDVInter_Check1",
                table: "ExpenseEntryInterEntity");

            migrationBuilder.DropColumn(
                name: "ExpDtl_DDVInter_Check2",
                table: "ExpenseEntryInterEntity");

            migrationBuilder.DropColumn(
                name: "ExpDtl_DDVInter_Conv_Amount1",
                table: "ExpenseEntryInterEntity");

            migrationBuilder.DropColumn(
                name: "ExpDtl_DDVInter_Conv_Amount2",
                table: "ExpenseEntryInterEntity");

            migrationBuilder.DropColumn(
                name: "ExpDtl_DDVInter_Curr1_ID",
                table: "ExpenseEntryInterEntity");

            migrationBuilder.DropColumn(
                name: "ExpDtl_DDVInter_Curr2_ID",
                table: "ExpenseEntryInterEntity");

            migrationBuilder.DropColumn(
                name: "ExpDtl_DDVInter_Rate",
                table: "ExpenseEntryInterEntity");

            migrationBuilder.DropColumn(
                name: "status",
                table: "ExpenseEntryAmortizations");

            migrationBuilder.AddColumn<string>(
                name: "Inter_ID",
                table: "ExpenseEntryInterEntity",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Inter_Currency1_ABBR",
                table: "ExpenseEntryInterEntity",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Inter_Currency1_Amount",
                table: "ExpenseEntryInterEntity",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Inter_Currency2_ABBR",
                table: "ExpenseEntryInterEntity",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Inter_Currency2_Amount",
                table: "ExpenseEntryInterEntity",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Inter_Particular_Title",
                table: "ExpenseEntryInterEntity",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Inter_Rate",
                table: "ExpenseEntryInterEntity",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExpenseEntryInterEntity",
                table: "ExpenseEntryInterEntity",
                column: "Inter_ID");
        }
    }
}
