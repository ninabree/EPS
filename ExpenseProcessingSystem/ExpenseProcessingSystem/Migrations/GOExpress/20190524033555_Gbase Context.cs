using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations.GOExpress
{
    public partial class GbaseContext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tblCM10",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SYSTEM_NAME = table.Column<string>(nullable: true),
                    GROUPCODE = table.Column<string>(nullable: true),
                    BRANCHNO = table.Column<string>(nullable: true),
                    OPE_KIND = table.Column<string>(nullable: true),
                    AUTO_APPROVED = table.Column<string>(nullable: true),
                    WARNING_OVERRIDE = table.Column<string>(nullable: true),
                    CCY_FORMAT = table.Column<string>(nullable: true),
                    OPE_BRANCH = table.Column<string>(nullable: true),
                    VALUE_DATE = table.Column<string>(nullable: true),
                    REFERENCE_TYPE = table.Column<string>(nullable: true),
                    REFERENCE_NO = table.Column<string>(nullable: true),
                    COMMENT = table.Column<string>(nullable: true),
                    SECTION = table.Column<string>(nullable: true),
                    REMARKS = table.Column<string>(nullable: true),
                    MEMO = table.Column<string>(nullable: true),
                    SCHEME_NO = table.Column<string>(nullable: true),
                    ENTRY11_TYPE = table.Column<string>(nullable: true),
                    ENTRY11_IBF_CODE = table.Column<string>(nullable: true),
                    ENTRY11_CCY = table.Column<string>(nullable: true),
                    ENTRY11_AMT = table.Column<string>(nullable: true),
                    ENTRY11_CUST = table.Column<string>(nullable: true),
                    ENTRY11_ACTCDE = table.Column<string>(nullable: true),
                    ENTRY11_ACT_TYPE = table.Column<string>(nullable: true),
                    ENTRY11_ACT_NO = table.Column<string>(nullable: true),
                    ENTRY11_EXCH_RATE = table.Column<string>(nullable: true),
                    ENTRY11_EXCH_CCY = table.Column<string>(nullable: true),
                    ENTRY11_FUND = table.Column<string>(nullable: true),
                    ENTRY11_CHECK_NO = table.Column<string>(nullable: true),
                    ENTRY11_AVAILABLE = table.Column<string>(nullable: true),
                    ENTRY11_ADVC_PRNT = table.Column<string>(nullable: true),
                    ENTRY11_DETAILS = table.Column<string>(nullable: true),
                    ENTRY11_DIVISION = table.Column<string>(nullable: true),
                    ENTRY11_INTER_AMT = table.Column<string>(nullable: true),
                    ENTRY11_INTER_RATE = table.Column<string>(nullable: true),
                    ENTRY22_TYPE = table.Column<string>(nullable: true),
                    ENTRY22_IBF_CODE = table.Column<string>(nullable: true),
                    ENTRY22_CCY = table.Column<string>(nullable: true),
                    ENTRY22_AMT = table.Column<string>(nullable: true),
                    ENTRY22_CUST = table.Column<string>(nullable: true),
                    ENTRY22_ACTCDE = table.Column<string>(nullable: true),
                    ENTRY22_ACT_TYPE = table.Column<string>(nullable: true),
                    ENTRY22_ACT_NO = table.Column<string>(nullable: true),
                    ENTRY22_EXCH_RATE = table.Column<string>(nullable: true),
                    ENTRY22_EXCH_CCY = table.Column<string>(nullable: true),
                    ENTRY22_FUND = table.Column<string>(nullable: true),
                    ENTRY22_CHECK_NO = table.Column<string>(nullable: true),
                    ENTRY22_AVAILABLE = table.Column<string>(nullable: true),
                    ENTRY22_ADVC_PRNT = table.Column<string>(nullable: true),
                    ENTRY22_DETAILS = table.Column<string>(nullable: true),
                    ENTRY22_DIVISION = table.Column<string>(nullable: true),
                    ENTRY22_INTER_AMT = table.Column<string>(nullable: true),
                    ENTRY22_INTER_RATE = table.Column<string>(nullable: true),
                    ENTRY33_TYPE = table.Column<string>(nullable: true),
                    ENTRY33_IBF_CODE = table.Column<string>(nullable: true),
                    ENTRY33_CCY = table.Column<string>(nullable: true),
                    ENTRY33_AMT = table.Column<string>(nullable: true),
                    ENTRY33_CUST = table.Column<string>(nullable: true),
                    ENTRY33_ACTCDE = table.Column<string>(nullable: true),
                    ENTRY33_ACT_TYPE = table.Column<string>(nullable: true),
                    ENTRY33_ACT_NO = table.Column<string>(nullable: true),
                    ENTRY33_EXCH_RATE = table.Column<string>(nullable: true),
                    ENTRY33_EXCH_CCY = table.Column<string>(nullable: true),
                    ENTRY33_FUND = table.Column<string>(nullable: true),
                    ENTRY33_CHECK_NO = table.Column<string>(nullable: true),
                    ENTRY33_AVAILABLE = table.Column<string>(nullable: true),
                    ENTRY33_ADVC_PRNT = table.Column<string>(nullable: true),
                    ENTRY33_DETAILS = table.Column<string>(nullable: true),
                    ENTRY33_DIVISION = table.Column<string>(nullable: true),
                    ENTRY33_INTER_AMT = table.Column<string>(nullable: true),
                    ENTRY33_INTER_RATE = table.Column<string>(nullable: true),
                    ENTRY44_TYPE = table.Column<string>(nullable: true),
                    ENTRY44_IBF_CODE = table.Column<string>(nullable: true),
                    ENTRY44_CCY = table.Column<string>(nullable: true),
                    ENTRY44_AMT = table.Column<string>(nullable: true),
                    ENTRY44_CUST = table.Column<string>(nullable: true),
                    ENTRY44_ACTCDE = table.Column<string>(nullable: true),
                    ENTRY44_ACT_TYPE = table.Column<string>(nullable: true),
                    ENTRY44_ACT_NO = table.Column<string>(nullable: true),
                    ENTRY44_EXCH_RATE = table.Column<string>(nullable: true),
                    ENTRY44_EXCH_CCY = table.Column<string>(nullable: true),
                    ENTRY44_FUND = table.Column<string>(nullable: true),
                    ENTRY44_CHECK_NO = table.Column<string>(nullable: true),
                    ENTRY44_AVAILABLE = table.Column<string>(nullable: true),
                    ENTRY44_ADVC_PRNT = table.Column<string>(nullable: true),
                    ENTRY44_DETAILS = table.Column<string>(nullable: true),
                    ENTRY44_DIVISION = table.Column<string>(nullable: true),
                    ENTRY44_INTER_AMT = table.Column<string>(nullable: true),
                    ENTRY44_INTER_RATE = table.Column<string>(nullable: true),
                    MAKER_EMPNO = table.Column<string>(nullable: true),
                    EMPNO = table.Column<string>(nullable: true),
                    DATESTAMP = table.Column<DateTime>(nullable: false),
                    TRANSNO = table.Column<string>(nullable: true),
                    XMLMSG = table.Column<string>(nullable: true),
                    RECSTATUS = table.Column<string>(nullable: true),
                    TIMESENT = table.Column<DateTime>(nullable: false),
                    TIMERESPOND = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tblCM10", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tblCM10");
        }
    }
}
