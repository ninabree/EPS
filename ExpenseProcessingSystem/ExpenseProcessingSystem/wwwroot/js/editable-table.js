doubleLoad = {};
doubleLoad.editableTable = doubleLoad.editableTable || {};
doubleLoad.editableTable.debugMode = false;

doubleLoad.isFirstLoad = function (namesp, jsFile) {
    var isFirst = namesp.firstLoad === undefined;
    namesp.firstLoad = false;

    if (!isFirst) {
        console.log(
            "Warning: Javascript file is included twice: " +
            jsFile);
    }

    return isFirst;
};

$(document).ready(function () {
    if (!doubleLoad.isFirstLoad(doubleLoad.editableTable, "editable-table.js")) {
        return;
    }

    var $TABLE = $('#table');
    var $NC_TABLE = $('#NCPartialTbl');
    var $BTN = $('#export-btn');
    var $EXPORT = $('#export');
    $(document).on('click', '.table-remove', function (e) {
        e.stopImmediatePropagation();
        var tr = $(this).closest('tr');
        tr.remove();
    });

    function PopulateDMRows(tableName) {
        
    }
    
    $(document).on("click", ".table-add", function (e) {
        var tblName = $('#dm-tbl').find(":selected").text();

        var tblRow = $("tr[id^='New']");
        var itemCount = tblName == "" ? $("[id^='gRemarks-tr']").length : $(tblRow).length;
        var $clone = tblName == "Vendor" ? venRowFormat(itemCount, getTaxRatesOptions(itemCount), getVATsOptions(itemCount)) :
            tblName == "Department" ? deptRowFormat(itemCount) :
            tblName == "Check" ? checkRowFormat(itemCount) :
            tblName == "Account" ? accRowFormat(itemCount, getFBTOptions(itemCount), getAGOptions(itemCount), getCurrOptions(itemCount)) :
            tblName == "Value Added Tax" ? vatRowFormat(itemCount) :
            tblName == "Fringe Benefit Tax" ? fbtRowFormat(itemCount) :
            tblName == "Tax Rates" ? trRowFormat(itemCount) :
            tblName == "Currency" ? currRowFormat(itemCount) :
            tblName == "Account Group" ? agRowFormat(itemCount) :
            tblName == "Regular Employee" ? regEmpRowFormat(itemCount) :
            tblName == "Temporary Employee" ? tempRegRowFormat(itemCount) :
            tblName == "Customer" ? custRowFormat(itemCount) :
            tblName == "" ? newGbaseRemarksRow(itemCount) : $('');
        $TABLE.find('table').append($clone);
    });

    $('.table-remove').click(function () {
        $(this).parents('tr').detach();
    });

    $('.table-up').click(function () {
        var $row = $(this).parents('tr');
        if ($row.index() === 1) return; // Don't go above the header
        $row.prev().before($row.get(0));
    });

    $('.table-down').click(function () {
        var $row = $(this).parents('tr');
        $row.next().after($row.get(0));
    });
    
    // ------------------- [[ FUNCTIONS ]] ------------------

    //----------------- [[ DM Functions ]] ------------------
    function getTaxRatesOptions(itemCount) {
        var troptions = "";
        if (itemCount > 0) {
            $("#NewVendor-Tr-0 td:nth-child(4) label#lbl").each(function (index, item) {
                var input = $(item).find("input");
                var txtVal = $(item).find("label").text();
                troptions += '<label><input type="checkbox" class="trChk" id="' + input.attr("id") + '" name = "' + input.attr("name") + '" value = "' + input.val() + '" > ' + txtVal +'</label>';
            });
        }
        return troptions;
    }
    function getVATsOptions(itemCount) {
        var vatoptions = "";
        if (itemCount > 0) {
            $("#NewVendor-Tr-0 td:nth-child(5) label#lbl").each(function (index, item) {
                var input = $(item).find("input");
                var txtVal = $(item).find("label").text();
                vatoptions += '<label><input class="vatChk" id="' + input.attr("id") + '" name="' + input.attr("name") + '" type="checkbox" value="' + input.val() + '"> ' + txtVal + '</label>';
            });
        }
        return vatoptions;
    }

    function getFBTOptions(itemCount) {
        var fbtoptions = "";
        if (itemCount > 0) {
            $("#NewAccountVM_0__Account_FBT_MasterID option").each(function (index, item) {
                fbtoptions += '<option value="' + item.value + '">' + item.text + '</option>';
            });
        } 
        return fbtoptions;
    }
    function getAGOptions(itemCount) {
        var agoptions = "";
        if (itemCount > 0) {
            $("#NewAccountVM_0__Account_Group_MasterID option").each(function (index, item) {
                agoptions += '<option value="' + item.value + '">' + item.text + '</option>';
            });
        } else {
            agoptions = $("#accgrpList").val();
        }
        return agoptions;
    }
    function getCurrOptions(itemCount) {
        var curroptions = "";
        if (itemCount > 0) {
            $("#NewAccountVM_0__Account_Currency_MasterID option").each(function (index, item) {
                curroptions += '<option value="' + item.value + '">' + item.text + '</option>';
            });
        }
        return curroptions;
    }
    function venRowFormat(itemCount, trOptions, vatOptions) {
        return $('<tr id="NewVendor-Tr-' + itemCount + '">'
            + '<td><input class="w-full" id="NewVendorVM_' + itemCount + '__Vendor_Name" name="NewVendorVM[' + itemCount + '].Vendor_Name" type="text" value=""></td>'
            + '<td><input class="w-full" id="NewVendorVM_' + itemCount + '__Vendor_TIN" name="NewVendorVM[' + itemCount + '].Vendor_TIN" type="text" value=""></td>'
            + '<td><input class="w-full" id="NewVendorVM_' + itemCount + '__Vendor_Address" name="NewVendorVM[' + itemCount + '].Vendor_Address" type="text" value=""></td>'
            + '<td colspan="2">'
            + trOptions
            + '</td>'
            + '<td colspan="2">'
            + vatOptions
            + '</td>'
            + '<td>'
            + '<span class="table-remove glyphicon glyphicon-remove"></span>'
            + '<input id="NewVendorVM_' + itemCount + '__Vendor_Tax_Rates_ID" name="NewVendorVM[' + itemCount + '].Vendor_Tax_Rates_ID" type="hidden" value="">'
            + '<input id="NewVendorVM_' + itemCount + '__Vendor_VAT_ID" name="NewVendorVM[' + itemCount + '].Vendor_VAT_ID" type="hidden" value="">'
            + '</td>'
            + '</tr>');
    }
    function deptRowFormat(itemCount) {
        return $('<tr id="NewDept-Tr-' + itemCount + '">'
            + '<td><input class="w-full" id="NewDeptVM_' + itemCount + '__Dept_Name" name="NewDeptVM[' + itemCount + '].Dept_Name" type="text" value=""></td>'
            + '<td><input class="w-full" id="NewDeptVM_' + itemCount + '__Dept_Code" name="NewDeptVM[' + itemCount + '].Dept_Code" type="text" value=""></td>'
            + '<td><input class="w-full" id="NewDeptVM_' + itemCount + '__Dept_Budget_Unit" name="NewDeptVM[' + itemCount + '].Dept_Budget_Unit" type="text" value=""></td>'
            + '<td>'
            + '<span class="table-remove glyphicon glyphicon-remove"></span>'
            + '</td>'
            + '</tr>');
    }
    function checkRowFormat(itemCount) {
        var getDateTime = new Date();
        return $('<tr id="NewCheck-Tr-' + itemCount + '">'
            + '<td><input class="h-20 form-control text-box single-line" data-val="true" data-val-required="The Input Date field is required." id="NewCheckVM_' + itemCount + '__Check_Input_Date" name="NewCheckVM[' + itemCount + '].Check_Input_Date" class="check-datetime" type="date" value="' + getDateTime + '"></td>'
            + '<td><input class="w-full" id="NewCheckVM_' + itemCount + '__Check_Series_From" name="NewCheckVM[' + itemCount + '].Check_Series_From" type="text" value=""></td>'
            + '<td><input class="w-full" id="NewCheckVM_' + itemCount + '__Check_Series_To" name="NewCheckVM[' + itemCount + '].Check_Series_To" type="text" value=""></td>'
            + '<td><input class="w-full" id="NewCheckVM_' + itemCount + '__Check_Bank_Info" name="NewCheckVM[' + itemCount + '].Check_Bank_Info" type="text" value=""></td>'
            + '<td>'
            + '<span class="table-remove glyphicon glyphicon-remove"></span>'
            + '</td>'
            + '</tr>');
    }
    function accRowFormat(itemCount, fbtoptions, agoptions, curroptions) {
        return $('<tr id="NewAccount-Tr-' + itemCount + '">'
            + '<td><input class="w-full" id="NewAccountVM_' + itemCount + '__Account_Name" name="NewAccountVM[' + itemCount + '].Account_Name" type="text" value=""></td>'
            + '<td><input class="w-full" id="NewAccountVM_' + itemCount + '__Account_Code" name="NewAccountVM[' + itemCount + '].Account_Code" type="text" value=""></td>'
            + '<td><input class="w-full" id="NewAccountVM_' + itemCount + '__Account_No" name="NewAccountVM[' + itemCount + '].Account_No" type="text" value=""></td>'
            + '<td><input class="w-full" id="NewAccountVM_' + itemCount + '__Account_Cust" name="NewAccountVM[' + itemCount + '].Account_Cust" type="text" value=""></td>'
            + '<td><input class="w-full" id="NewAccountVM_' + itemCount + '__Account_Div" name="NewAccountVM[' + itemCount + '].Account_Div" type="text" value=""></td>'
            + '<td><input class="w-full" data-val="true" data-val-required="The Account Fund field is required." id="NewAccountVM_' + itemCount + '__Account_Fund" name="NewAccountVM[' + itemCount + '].Account_Fund" type="checkbox" value="true"></td>'
            + '<td colspan="2">'
            + '<select class="w-full input" data-val="true" data-val-required="The Account FBT field is required." id="NewAccountVM_' + itemCount + '__Account_FBT_MasterID" name="NewAccountVM[' + itemCount + '].Account_FBT_MasterID">'
            + fbtoptions
            + '</select>'
            + '</td>'
            + '<td colspan="2">'
            + '<select class="w-full input" data-val="true" data-val-required="The Account Group field is required." id="NewAccountVM_' + itemCount + '__Account_Group_MasterID" name="NewAccountVM[' + itemCount + '].Account_Group_MasterID">'
            + agoptions
            + '</select>'
            + '</td>'
            + '<td colspan="2">'
            + '<select class="w-full input" data-val="true" data-val-required="The Account Currency field is required." id="NewAccountVM_' + itemCount + '__Account_Currency_MasterID" name="NewAccountVM[' + itemCount + '].Account_Currency_MasterID">'
            + curroptions
            + '</select>'
            + '</td>'
            + '<td>'
            + '<span class="table-remove glyphicon glyphicon-remove"></span>'
            + '</td>'
            + '</tr>');
    }
    function vatRowFormat(itemCount) {
        return $('<tr id="NewVAT-Tr-' + itemCount + '">'
            + '<td><input class="w-full" id="NewVATVM_' + itemCount + '__VAT_Name" name="NewVATVM[' + itemCount + '].VAT_Name" type="text" value=""></td>'
            + '<td><input class="w-full" id="NewVATVM_' + itemCount + '__VAT_Rate" name="NewVATVM[' + itemCount + '].VAT_Rate" type="text" value=""></td>'
            + '<td><span class="table-remove glyphicon glyphicon-remove"></span>'
            + '</td>'
            + '</tr>');
    }
    function fbtRowFormat(itemCount) {
        return $('<tr id="NewFBT-Tr-' + itemCount + '">'
            + '<td><input class="w-full" id="NewFBTVM_' + itemCount + '__FBT_Name" name="NewFBTVM[' + itemCount + '].FBT_Name" type="text" value=""></td>'
            + '<td><input class="w-full" id="NewFBTVM_' + itemCount + '__FBT_Formula" name="NewFBTVM[' + itemCount + '].FBT_Formula" type="text" value=""></td>'
            + '<td><input class="w-full" data-val="true" data-val-required="The FBT Tax Rate field is required." id="NewFBTVM_' + itemCount + '__FBT_Tax_Rate" name="NewFBTVM[' + itemCount + '].FBT_Tax_Rate" type="text" value="0"></td>'
            + '<td>'
            + '<span class="table-remove glyphicon glyphicon-remove"></span>'
            + '</td>'
            + '</tr>');
    }
    function trRowFormat(itemCount) {
        return $('<tr id="NewTR-Tr-' + itemCount + '">'
            + '<td><input class="w-full" id="NewTRVM_' + itemCount + '__TR_WT_Title" name="NewTRVM[' + itemCount + '].TR_WT_Title" type="text" value=""></td>'
            + '<td><input class="w-full" id="NewTRVM_' + itemCount + '__TR_Nature" name="NewTRVM[' + itemCount + '].TR_Nature" type="text" value=""></td>'
            + '<td><input class="w-full" data-val="true" data-val-required="The Tax Rate field is required." id="NewTRVM_' + itemCount + '__TR_Tax_Rate" name="NewTRVM[' + itemCount + '].TR_Tax_Rate" type="text" value="0"></td>'
            + '<td><input class="w-full" id="NewTRVM_' + itemCount + '__TR_ATC" name="NewTRVM[' + itemCount + '].TR_ATC" type="text" value=""></td>'
            + '<td><input class="w-full" id="NewTRVM_' + itemCount + '__TR_Nature_Income_Payment" name="NewTRVM[' + itemCount + '].TR_Nature_Income_Payment" type="text" value=""></td>'
            + '<td>'
            + '<span class="table-remove glyphicon glyphicon-remove"></span>'
            + '</td>'
            + '</tr>');
    }
    function currRowFormat(itemCount) {
        return $('<tr id="NewCurr-Tr-' + itemCount + '">'
            + '<td><input class="w-full" id="NewCurrVM_' + itemCount + '__Curr_Name" name="NewCurrVM[' + itemCount + '].Curr_Name" type="text" value=""></td>'
            + '<td><input class="w-full" id="NewCurrVM_' + itemCount + '__Curr_CCY_ABBR" name="NewCurrVM[' + itemCount + '].Curr_CCY_ABBR" type="text" value=""></td>'
            + '<td>'
            + '<span class="table-remove glyphicon glyphicon-remove"></span>'
            + '</td>'
            + '</tr>');
    }
    function agRowFormat(itemCount) {
        return $('<tr id="NewAccountGroup-Tr-' + itemCount + '">'
            + '<td><input class="w-full" id="NewAccountGroupVM_' + itemCount + '__AccountGroup_Name" name="NewAccountGroupVM[' + itemCount + '].AccountGroup_Name" type="text" value=""></td>'
            + '<td><input class="w-full" id="NewAccountGroupVM_' + itemCount + '__AccountGroup_Code" name="NewAccountGroupVM[' + itemCount + '].AccountGroup_Code" type="text" value=""></td>'
            + '<td>'
            + '<span class="table-remove glyphicon glyphicon-remove"></span>'
            + '</td>'
            + '</tr>');
    }
    function regEmpRowFormat(itemCount) {
        return $('<tr id="NewEmp-Tr-' + itemCount + '">'
            + '<td><input class="w-full" id="NewEmpVM_' + itemCount + '__Emp_Name" name="NewEmpVM[' + itemCount + '].Emp_Name" type="text" value=""></td>'
            + '<td><input class="w-full" id="NewEmpVM_' + itemCount + '__Emp_Acc_No" name="NewEmpVM[' + itemCount + '].Emp_Acc_No" type="text" value=""></td>'
            + '<td>'
            + '<span class="table-remove glyphicon glyphicon-remove"></span>'
            + '</td>'
            + '</tr>');
    }
    function tempRegRowFormat(itemCount) {
        return $('<tr id="NewEmp-Tr-' + itemCount + '">'
            + '<td><input class="w-full" id="NewEmpVM_' + itemCount + '__Emp_Name" name="NewEmpVM[' + itemCount + '].Emp_Name" type="text" value=""></td>'
            + '<td>'
            + '<span class="table-remove glyphicon glyphicon-remove"></span>'
            + '</td>'
            + '</tr>');
    }
    function custRowFormat(itemCount) {
        return $('<tr id="NewCust-Tr-' + itemCount + '">'
            + '<td><input class="w-full" id="NewCustVM_' + itemCount + '__Cust_Name" name="NewCustVM[' + itemCount + '].Cust_Name" type="text" value=""></td>'
            + '<td><input class="w-full" id="NewCustVM_' + itemCount + '__Cust_Abbr" name="NewCustVM[' + itemCount + '].Cust_Abbr" type="text" value=""></td>'
            + '<td><input class="w-full" id="NewCustVM_' + itemCount + '__Cust_No" name="NewCustVM[' + itemCount + '].Cust_No" type="text" value=""></td>'
            + '<td>'
            + '<span class="table-remove glyphicon glyphicon-remove"></span>'
            + '</td>'
            + '</tr>');
    }
    function newGbaseRemarksRow(itemCount) {
        return $('<tr id="gRemarks-tr-' + itemCount + '">'
            + '<td><input type="text" class="gDocuType" /></td>'
            + '<td><input type="text" class="gInvoiceNo" /></td>'
            + '<td><input type="text" class="gDescription" /></td>'
            + '<td><input type="number" min="0" class="gAmount" style="width:100%"  /></td>'
            + '<td><div class="flex-c"><span class="table-remove glyphicon glyphicon-remove"></span></div></td></tr>');
    }
    // A few jQuery helpers for exporting only
    jQuery.fn.pop = [].pop;
    jQuery.fn.shift = [].shift;

    $BTN.click(function () {
        var $rows = $TABLE.find('tr:not(:hidden)');
        var headers = [];
        var data = [];

        // Get the headers (add special header logic here)
        $($rows.shift()).find('th:not(:empty)').each(function () {
            headers.push($(this).text().toLowerCase());
        });

        // Turn all existing rows into a loopable array
        $rows.each(function () {
            var $td = $(this).find('td');
            var h = {};

            // Use the headers from earlier to name our hash keys
            headers.forEach(function (header, i) {
                h[header] = $td.eq(i).text();
            });

            data.push(h);
        });

        // Output the result
        $EXPORT.text(JSON.stringify(data));
    });
});
