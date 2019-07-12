/*//////////////////////////////////////////////////////////////////
    [ UM ]*/

//to highlight a table row in UM
var rows = $('#um-tbl tbody tr');
rows.on('click', function (e) {
    /* Get current row */
    var row = $(this);
    /* Check if 'Ctrl', 'cmd' or 'Shift' keyboard key was pressed
     * 'Ctrl' => is represented by 'e.ctrlKey' or 'e.metaKey'
     * 'Shift' => is represented by 'e.shiftKey' */
    if ((e.ctrlKey || e.metaKey) || e.shiftKey) {
        /* If pressed highlight the other row that was clicked */
        //row.addClass('highlight');
    } else {
        var name = $(row).children(":eq(1)").text().split(', ');
        $('#NewAcc_User_UserName').prop("readonly", true);
        $('#NewAcc_User_UserName').val($(row).children(":first").text());
        $('#NewAcc_User_FName').val(name[1]);
        $('#NewAcc_User_LName').val(name[0]);
        $('#NewAcc_User_DeptID').val($(row).children(":eq(2)").attr('id'));
        $('#NewAcc_User_Email').val($(row).children(":eq(4)").text());
        $('#NewAcc_User_Role').val($(row).children(":eq(3)").text());
        $('#NewAcc_User_ID').val(parseInt($(row).children(":eq(12)").val()));
        $('#NewAcc_User_Comment').val($(row).children(":eq(5)").text());
        $('#NewAcc_User_InUse').prop('checked', ($(row).children(":eq(6)").find('input').is(":checked")) ? true : false);

        /* Otherwise just highlight one row and clean others */
        rows.removeClass('highlight');
        row.addClass('highlight');
    }
    /* This 'event' is used just to avoid that the table text 
     * gets selected (just for styling). */
    $(document).bind('selectstart dragstart', function (e) {
        e.preventDefault(); return false;
    });

    $('#clear-btn').click(function (e) {
        e.stopImmediatePropagation();
        $('#NewAcc_User_UserName').prop("readonly", false);
        $('#NewAcc_User_UserName').val("");
        $('#NewAcc_User_FName').val("");
        $('#NewAcc_User_LName').val("");
        $('#NewAcc_User_DeptID').val("0");
        $('#NewAcc_User_Email').val("");
        $('#NewAcc_User_Role').val("");
        $('#NewAcc_User_UserID').val("");
        $('#NewAcc_User_Comment').val("");
        $('#NewAcc_User_InUse').prop('checked', false);
        $('#validationSummary').empty();
        rows.removeClass('highlight');
        return false;
    });
});