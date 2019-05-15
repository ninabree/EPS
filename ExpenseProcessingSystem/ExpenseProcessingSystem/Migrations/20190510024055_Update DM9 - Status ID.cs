using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseProcessingSystem.Migrations
{
    public partial class UpdateDM9StatusID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pending_VTV_Status",
                table: "DMVendorTRVAT_Pending");

            migrationBuilder.DropColumn(
                name: "Pending_Vendor_Status",
                table: "DMVendor_Pending");

            migrationBuilder.DropColumn(
                name: "Vendor_Status",
                table: "DMVendor");

            migrationBuilder.DropColumn(
                name: "Pending_VAT_Status",
                table: "DMVAT_Pending");

            migrationBuilder.DropColumn(
                name: "VAT_Status",
                table: "DMVAT");

            migrationBuilder.DropColumn(
                name: "Pending_TR_Status",
                table: "DMTR_Pending");

            migrationBuilder.DropColumn(
                name: "TR_Status",
                table: "DMTR");

            migrationBuilder.DropColumn(
                name: "Pending_FBT_Status",
                table: "DMFBT_Pending");

            migrationBuilder.DropColumn(
                name: "FBT_Status",
                table: "DMFBT");

            migrationBuilder.DropColumn(
                name: "Pending_Emp_Status",
                table: "DMEmp_Pending");

            migrationBuilder.DropColumn(
                name: "Emp_Status",
                table: "DMEmp");

            migrationBuilder.DropColumn(
                name: "Pending_Dept_Status",
                table: "DMDept_Pending");

            migrationBuilder.DropColumn(
                name: "Dept_Status",
                table: "DMDept");

            migrationBuilder.DropColumn(
                name: "Pending_Cust_Status",
                table: "DMCust_Pending");

            migrationBuilder.DropColumn(
                name: "Cust_Status",
                table: "DMCust");

            migrationBuilder.DropColumn(
                name: "Pending_Curr_Status",
                table: "DMCurrency_Pending");

            migrationBuilder.DropColumn(
                name: "Curr_Status",
                table: "DMCurrency");

            migrationBuilder.DropColumn(
                name: "Pending_Check_Status",
                table: "DMCheck_Pending");

            migrationBuilder.DropColumn(
                name: "Check_Status",
                table: "DMCheck");

            migrationBuilder.DropColumn(
                name: "Pending_BCS_Status",
                table: "DMBCS_Pending");

            migrationBuilder.DropColumn(
                name: "BCS_Status",
                table: "DMBCS");

            migrationBuilder.DropColumn(
                name: "Pending_AccountGroup_Status",
                table: "DMAccountGroup_Pending");

            migrationBuilder.DropColumn(
                name: "AccountGroup_Status",
                table: "DMAccountGroup");

            migrationBuilder.DropColumn(
                name: "Pending_Account_Status",
                table: "DMAccount_Pending");

            migrationBuilder.DropColumn(
                name: "Account_Status",
                table: "DMAccount");

            migrationBuilder.AddColumn<int>(
                name: "Pending_VTV_Status_ID",
                table: "DMVendorTRVAT_Pending",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Pending_Vendor_Status_ID",
                table: "DMVendor_Pending",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Vendor_Status_ID",
                table: "DMVendor",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Pending_VAT_Status_ID",
                table: "DMVAT_Pending",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VAT_Status_ID",
                table: "DMVAT",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Pending_TR_Status_ID",
                table: "DMTR_Pending",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TR_Status_ID",
                table: "DMTR",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Pending_FBT_Status_ID",
                table: "DMFBT_Pending",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FBT_Status_ID",
                table: "DMFBT",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Pending_Emp_Status_ID",
                table: "DMEmp_Pending",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Emp_Status_ID",
                table: "DMEmp",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Pending_Dept_Status_ID",
                table: "DMDept_Pending",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Dept_Status_ID",
                table: "DMDept",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Pending_Cust_Status_ID",
                table: "DMCust_Pending",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Cust_Status_ID",
                table: "DMCust",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Pending_Curr_Status_ID",
                table: "DMCurrency_Pending",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Curr_Status_ID",
                table: "DMCurrency",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Pending_Check_Status_ID",
                table: "DMCheck_Pending",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Check_Status_ID",
                table: "DMCheck",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Pending_BCS_Status_ID",
                table: "DMBCS_Pending",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BCS_Status_ID",
                table: "DMBCS",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Pending_AccountGroup_Status_ID",
                table: "DMAccountGroup_Pending",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AccountGroup_Status_ID",
                table: "DMAccountGroup",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Pending_Account_Status_ID",
                table: "DMAccount_Pending",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Account_Status_ID",
                table: "DMAccount",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pending_VTV_Status_ID",
                table: "DMVendorTRVAT_Pending");

            migrationBuilder.DropColumn(
                name: "Pending_Vendor_Status_ID",
                table: "DMVendor_Pending");

            migrationBuilder.DropColumn(
                name: "Vendor_Status_ID",
                table: "DMVendor");

            migrationBuilder.DropColumn(
                name: "Pending_VAT_Status_ID",
                table: "DMVAT_Pending");

            migrationBuilder.DropColumn(
                name: "VAT_Status_ID",
                table: "DMVAT");

            migrationBuilder.DropColumn(
                name: "Pending_TR_Status_ID",
                table: "DMTR_Pending");

            migrationBuilder.DropColumn(
                name: "TR_Status_ID",
                table: "DMTR");

            migrationBuilder.DropColumn(
                name: "Pending_FBT_Status_ID",
                table: "DMFBT_Pending");

            migrationBuilder.DropColumn(
                name: "FBT_Status_ID",
                table: "DMFBT");

            migrationBuilder.DropColumn(
                name: "Pending_Emp_Status_ID",
                table: "DMEmp_Pending");

            migrationBuilder.DropColumn(
                name: "Emp_Status_ID",
                table: "DMEmp");

            migrationBuilder.DropColumn(
                name: "Pending_Dept_Status_ID",
                table: "DMDept_Pending");

            migrationBuilder.DropColumn(
                name: "Dept_Status_ID",
                table: "DMDept");

            migrationBuilder.DropColumn(
                name: "Pending_Cust_Status_ID",
                table: "DMCust_Pending");

            migrationBuilder.DropColumn(
                name: "Cust_Status_ID",
                table: "DMCust");

            migrationBuilder.DropColumn(
                name: "Pending_Curr_Status_ID",
                table: "DMCurrency_Pending");

            migrationBuilder.DropColumn(
                name: "Curr_Status_ID",
                table: "DMCurrency");

            migrationBuilder.DropColumn(
                name: "Pending_Check_Status_ID",
                table: "DMCheck_Pending");

            migrationBuilder.DropColumn(
                name: "Check_Status_ID",
                table: "DMCheck");

            migrationBuilder.DropColumn(
                name: "Pending_BCS_Status_ID",
                table: "DMBCS_Pending");

            migrationBuilder.DropColumn(
                name: "BCS_Status_ID",
                table: "DMBCS");

            migrationBuilder.DropColumn(
                name: "Pending_AccountGroup_Status_ID",
                table: "DMAccountGroup_Pending");

            migrationBuilder.DropColumn(
                name: "AccountGroup_Status_ID",
                table: "DMAccountGroup");

            migrationBuilder.DropColumn(
                name: "Pending_Account_Status_ID",
                table: "DMAccount_Pending");

            migrationBuilder.DropColumn(
                name: "Account_Status_ID",
                table: "DMAccount");

            migrationBuilder.AddColumn<string>(
                name: "Pending_VTV_Status",
                table: "DMVendorTRVAT_Pending",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pending_Vendor_Status",
                table: "DMVendor_Pending",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Vendor_Status",
                table: "DMVendor",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pending_VAT_Status",
                table: "DMVAT_Pending",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VAT_Status",
                table: "DMVAT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pending_TR_Status",
                table: "DMTR_Pending",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TR_Status",
                table: "DMTR",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pending_FBT_Status",
                table: "DMFBT_Pending",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FBT_Status",
                table: "DMFBT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pending_Emp_Status",
                table: "DMEmp_Pending",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Emp_Status",
                table: "DMEmp",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pending_Dept_Status",
                table: "DMDept_Pending",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Dept_Status",
                table: "DMDept",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pending_Cust_Status",
                table: "DMCust_Pending",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cust_Status",
                table: "DMCust",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pending_Curr_Status",
                table: "DMCurrency_Pending",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Curr_Status",
                table: "DMCurrency",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pending_Check_Status",
                table: "DMCheck_Pending",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Check_Status",
                table: "DMCheck",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pending_BCS_Status",
                table: "DMBCS_Pending",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BCS_Status",
                table: "DMBCS",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pending_AccountGroup_Status",
                table: "DMAccountGroup_Pending",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountGroup_Status",
                table: "DMAccountGroup",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Pending_Account_Status",
                table: "DMAccount_Pending",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Account_Status",
                table: "DMAccount",
                nullable: true);
        }
    }
}
