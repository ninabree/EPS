var $TABLE = $('#table');
var $BTN = $('#export-btn');
var $EXPORT = $('#export');
$(document).on('click', '.table-remove', function (e) {
    e.stopImmediatePropagation();
    var tr = $(this).closest('tr');
    tr.remove();
}); 
$('.table-add').click(function () {
    var tblName = $('#dm-tbl').find(":selected").text();
    //depends on chosen Master Data Table
    var itemCount = tblName == "Payee" ? ($("[id^='NewPayee-Tr']").length) :
        tblName == "Department" ? ($("[id^='NewDept-Tr']").length) :
        tblName == "Check" ? ($("[id^='NewCheck-Tr']").length) :
        tblName == "Account" ? ($("[id^='NewAccount-Tr']").length) :
        tblName == "Value Added Tax" ? ($("[id^='NewVAT-Tr']").length) :
        tblName == "Fringe Benefit Tax" ? ($("[id^='NewFBT-Tr']").length) :
        tblName == "Expanded Withholding Tax" ? ($("[id^='NewEWT-Tr']").length) :
        tblName == "Currency" ? ($("[id^='NewCurr-Tr']").length) : 0;
    

    //ROW TEMPLATES
    var newPayeeRow = $('<tr id="NewPayee-Tr-' + itemCount + '">'
        +'<td><input class="w-full" id="NewPayeeVM_' + itemCount + '__Payee_Name" name="NewPayeeVM[' + itemCount + '].Payee_Name" type="text" value=""></td>'
        +'<td><input class="w-full" id="NewPayeeVM_' + itemCount + '__Payee_TIN" name="NewPayeeVM[' + itemCount + '].Payee_TIN" type="text" value=""></td>'
        +'<td><input class="w-full" id="NewPayeeVM_' + itemCount + '__Payee_Address" name="NewPayeeVM[' + itemCount + '].Payee_Address" type="text" value=""></td>'
        +'<td><input class="w-full" id="NewPayeeVM_' + itemCount + '__Payee_Type" name="NewPayeeVM[' + itemCount + '].Payee_Type" type="text" value=""></td>'
        +'<td><input data-val="true" data-val-required="The Payee_No field is required." id="NewPayeeVM_' + itemCount + '__Payee_No" name="NewPayeeVM[' + itemCount + '].Payee_No" type="text" value="0"></td>'
        +'<td>'
        +'<span class="table-remove glyphicon glyphicon-remove"></span>'
        +'</td>'
        + '</tr>');

    var newDeptRow = $('<tr id="NewDept-Tr-' + itemCount + '">'
        + '<td><input class="w-full" id="NewDeptVM_' + itemCount + '__Dept_Name" name="NewDeptVM[' + itemCount + '].Dept_Name" type="text" value=""></td>'
        + '<td><input class="w-full" id="NewDeptVM_' + itemCount + '__Dept_Code" name="NewDeptVM[' + itemCount + '].Dept_Code" type="text" value=""></td>'
        + '<td>'
        + '<span class="table-remove glyphicon glyphicon-remove"></span>'
        + '</td>'
        + '</tr>');
    var newCheckRow = $('<tr id="NewCheck-Tr-' + itemCount + '">'
        + '<td colspan = "3"> <input class="h-20 form-control text-box single-line" data-val="true" data-val-required="The Input Date field is required." id="NewCheckVM_' + itemCount + '__Check_Input_Date" name="NewCheckVM[' + itemCount + '].Check_Input_Date" type="datetime-local" value="2019-03-25T16:04:32.001"></td>'
        + '<td><input class="w-full" id="NewCheckVM_' + itemCount + '__Check_Series_From" name="NewCheckVM[' + itemCount + '].Check_Series_From" type="text" value=""></td>'
        + '<td><input class="w-full" id="NewCheckVM_' + itemCount + '__Check_Series_To" name="NewCheckVM[' + itemCount + '].Check_Series_To" type="text" value=""></td>'
        + '<td><input class="w-full" id="NewCheckVM_' + itemCount + '__Check_Name" name="NewCheckVM[' + itemCount + '].Check_Name" type="text" value=""></td>'
        + '<td><input class="w-full" id="NewCheckVM_' + itemCount + '__Check_Type" name="NewCheckVM[' + itemCount + '].Check_Type" type="text" value=""></td>'
        + '<td><span class="table-remove glyphicon glyphicon-remove"></span>'
        + '</td>'
        + '</tr>');
    var newAccRow = $('<tr id="NewAccount-Tr-' + itemCount + '">'
        + '<td><input class="w-full" id="NewAccountVM_' + itemCount + '__Account_Name" name="NewAccountVM[' + itemCount + '].Account_Name" type="text" value=""></td>'
        + '<td><input class="w-full" id="NewAccountVM_' + itemCount + '__Account_Code" name="NewAccountVM[' + itemCount + '].Account_Code" type="text" value=""></td>'
        + '<td><input class="w-full" data-val="true" data-val-required="The Account No field is required." id="NewAccountVM_' + itemCount + '__Account_No" name="NewAccountVM[' + itemCount + '].Account_No" type="text" value="0"></td>'
        + '<td><input class="w-full" id="NewAccountVM_' + itemCount + '__Account_Cust" name="NewAccountVM[' + itemCount + '].Account_Cust" type="text" value=""></td>'
        + '<td><input class="w-full" id="NewAccountVM_' + itemCount + '__Account_Div" name="NewAccountVM[' + itemCount + '].Account_Div" type="text" value=""></td>'
        + '<td><input class="w-full" id="NewAccountVM_' + itemCount + '__Account_Fund" name="NewAccountVM[' + itemCount + '].Account_Fund" type="text" value=""></td>'
        + '<td><span class="table-remove glyphicon glyphicon-remove"></span>'
        + '</td>'
        + '</tr>');
    var newVatRow = $('<tr id="NewVAT-Tr-' + itemCount + '">'
        + '<td><input class="w-full" id="NewVATVM_' + itemCount + '__VAT_Name" name="NewVATVM[' + itemCount + '].VAT_Name" type="text" value=""></td>'
        + '<td><input class="w-full" id="NewVATVM_' + itemCount + '__VAT_Rate" name="NewVATVM[' + itemCount + '].VAT_Rate" type="text" value=""></td>'
        + '<td><span class="table-remove glyphicon glyphicon-remove"></span>'
        + '</td>'
        + '</tr>');
    var newFbtRow = $('<tr id="NewFBT-Tr-' + itemCount + '">'
        + '<td><input class="w-full" id="NewFBTVM_' + itemCount + '__FBT_Name" name="NewFBTVM[' + itemCount + '].FBT_Name" type="text" value=""></td>'
        + '<td><input class="w-full" id="NewFBTVM_' + itemCount + '__FBT_Account" name="NewFBTVM[' + itemCount + '].FBT_Account" type="text" value=""></td>'
        + '<td><input class="w-full" id="NewFBTVM_' + itemCount + '__FBT_Formula" name="NewFBTVM[' + itemCount + '].FBT_Formula" type="text" value=""></td>'
        + '<td><input class="w-full" data-val="true" data-val-required="The FBT Tax Rate field is required." id="NewFBTVM_' + itemCount + '__FBT_Tax_Rate" name="NewFBTVM[' + itemCount + '].FBT_Tax_Rate" type="text" value="0"></td>'
        + '<td>'
        + '<span class="table-remove glyphicon glyphicon-remove"></span>'
        + '</td>'
        + '</tr>');
    var newEwtRow = $('<tr id="NewEWT-Tr-' + itemCount + '">'
        + '<td><input class="w-full" id="NewEWTVM_' + itemCount + '__EWT_Nature" name="NewEWTVM[' + itemCount + '].EWT_Nature" type="text" value=""></td>'
        + '<td><input class="w-full" data-val="true" data-val-required="The EWT Tax Rate field is required." id="NewEWTVM_' + itemCount + '__EWT_Tax_Rate" name="NewEWTVM[' + itemCount + '].EWT_Tax_Rate" type="text" value="0"></td>'
        + '<td><input class="w-full" id="NewEWTVM_' + itemCount + '__EWT_ATC" name="NewEWTVM[' + itemCount + '].EWT_ATC" type="text" value=""></td>'
        + '<td><input class="w-full" id="NewEWTVM_' + itemCount + '__EWT_Tax_Rate_Desc" name="NewEWTVM[' + itemCount + '].EWT_Tax_Rate_Desc" type="text" value=""></td>'
        + '<td>'
        + '<span class="table-remove glyphicon glyphicon-remove"></span>'
        + '</td>'
        + '</tr>');
    var newCurrRow = $('<tr id="NewCurr-Tr-' + itemCount + '">'
        + '<td><input class="w-full" id="NewCurrVM_' + itemCount + '__Curr_Name" name="NewCurrVM[' + itemCount + '].Curr_Name" type="text" value=""></td>'
        + '<td><input class="w-full" id="NewCurrVM_' + itemCount + '__Curr_CCY_Code" name="NewCurrVM[' + itemCount + '].Curr_CCY_Code" type="text" value=""></td>'
        + '<td>'
        + '<span class="table-remove glyphicon glyphicon-remove"></span>'
        + '</td>'
        + '</tr>');

    var $clone = tblName == "Payee" ? newPayeeRow :
        tblName == "Department" ? newDeptRow :
        tblName == "Check" ? newCheckRow :
        tblName == "Account" ? newAccRow :
        tblName == "Value Added Tax" ? newVatRow :
        tblName == "Fringe Benefit Tax" ? newFbtRow :
        tblName == "Expanded Withholding Tax" ? newEwtRow :
        tblName == "Currency" ? newCurrRow : $('');

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