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
        tblName == "Department" ? ($("[id^='NewDept-Tr']").length) : 0;

    //ROW TEMPLATES
    var newPayeeRow = $('<tr id="NewPayee-Tr-' + itemCount + '">'
        +'<td><input id="NewPayeeVM_' + itemCount + '__Payee_Name" name="NewPayeeVM[' + itemCount + '].Payee_Name" type="text" value=""></td>'
        +'<td><input id="NewPayeeVM_' + itemCount + '__Payee_TIN" name="NewPayeeVM[' + itemCount + '].Payee_TIN" type="text" value=""></td>'
        +'<td><input id="NewPayeeVM_' + itemCount + '__Payee_Address" name="NewPayeeVM[' + itemCount + '].Payee_Address" type="text" value=""></td>'
        +'<td><input id="NewPayeeVM_' + itemCount + '__Payee_Type" name="NewPayeeVM[' + itemCount + '].Payee_Type" type="text" value=""></td>'
        +'<td><input data-val="true" data-val-required="The Payee_No field is required." id="NewPayeeVM_' + itemCount + '__Payee_No" name="NewPayeeVM[' + itemCount + '].Payee_No" type="text" value=""></td>'
        +'<td>'
            +'<span class="table-remove glyphicon glyphicon-remove"></span>'
        +'</td>'
        + '</tr>');

    var newDeptRow = $('<tr id="NewDept-Tr-' + itemCount + '">'
        + '<td><input id="NewDeptVM_' + itemCount + '__Dept_Name" name="NewDeptVM[' + itemCount + '].Dept_Name" type="text" value=""></td>'
        + '<td><input id="NewDeptVM_' + itemCount + '__Dept_Code" name="NewDeptVM[' + itemCount + '].Dept_Code" type="text" value=""></td>'
        + '<td>'
        + '<span class="table-remove glyphicon glyphicon-remove"></span>'
        + '</td>'
        + '</tr>');
    
    var $clone = tblName == "Payee" ? newPayeeRow :
        tblName == "Department" ? newDeptRow : $('');

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