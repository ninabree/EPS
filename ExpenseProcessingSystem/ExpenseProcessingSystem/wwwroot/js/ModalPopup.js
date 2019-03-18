$(function () {
    /*//////////////////////////////////////////////////////////////////
    [ MODAL ]*/
    //set content to modal body and show dynamic modal
    //Entry_DDV
    $("#reversal_entry").click(function () {
        //controller name, method name, modal header
        ModalPopup('Modal', 'ReversalEntryModal', 'Reversal Entry');
    });

    $(".adjust_btn").click(function () {
        var id = this.id;
        var splitid = id.split("_");
        //controller name, method name, modal header
        ModalPopup('Modal', 'BudgetAdjustmentModal', 'Budget Adjustment');
    });

    $("#apprv_payee").click(function (e) {
        e.stopImmediatePropagation();
        var chkCount = $('input.tbl-chk[type="checkbox"]:checked').length;
        if (!(chkCount <= 0)) {
            var idsArr = Array();
            $('input.tbl-chk[type="checkbox"]:checked').each(function (i, v) {
                idsArr.push($(v).attr('id'));
            });
            //controller name, method name, modal header
            ModalPopup('Modal', 'DMApprovePayee', 'Approve Record/s', idsArr);
        } else {
            alert("No data record/s selected.");
        }
    });

    $("#rej_payee").click(function (e) {
        e.stopImmediatePropagation();
        var chkCount = $('input.tbl-chk[type="checkbox"]:checked').length;
        if (!(chkCount <= 0)) {
            var idsArr = Array();
            $('input.tbl-chk[type="checkbox"]:checked').each(function (i, v) {
                idsArr.push($(v).attr('id'));
            });
            //controller name, method name, modal header
            ModalPopup('Modal', 'DMRejPayee', 'Reject Record/s', idsArr);
        } else {
            alert("No data record/s selected.");
        }
    });
    $("#add_payee").click(function (e) {
        e.stopImmediatePropagation();
        //controller name, method name, modal header
        ModalPopup('Modal', 'DMAddPayee_Pending', 'Add New Record/s');
    });

    $("#edit_payee").click(function (e) {
        e.stopImmediatePropagation();
        var chkCount = $('input.tbl-chk[type="checkbox"]:checked').length;
        if (!(chkCount <= 0)) {
            var idsArr = Array();
            $('input.tbl-chk[type="checkbox"]:checked').each(function (i, v) {
                idsArr.push($(v).attr('id'));
            });
            //controller name, method name, modal header
            ModalPopup('Modal', 'DMEditPayee', 'Edit Record/s', idsArr);
        } else {
            alert("No data record/s selected.");
        }
    });

    $("#delete_payee").click(function (e) {
        e.stopImmediatePropagation();
        var chkCount = $('input.tbl-chk[type="checkbox"]:checked').length;
        if (!(chkCount <= 0)) {
            var idsArr = Array();
            $('input.tbl-chk[type="checkbox"]:checked').each(function (i, v) {
                idsArr.push($(v).attr('id'));
            });
            //controller name, method name, modal header
            ModalPopup('Modal', 'DMDeletePayee', 'Confirm deletion of record/s?', idsArr);
        } else {
            alert("No data record/s selected.");
        }
    });

    $("#add_dept").click(function (e) {
        e.stopImmediatePropagation();
        //controller name, method name, modal header
        ModalPopup('Modal', 'DMAddDept', 'Add New Record/s');
    });

    $("#edit_dept").click(function (e) {
        e.stopImmediatePropagation();
        var chkCount = $('input.tbl-chk[type="checkbox"]:checked').length;
        if (!(chkCount <= 0)) {
            var idsArr = Array();
            $('input.tbl-chk[type="checkbox"]:checked').each(function (i, v) {
                idsArr.push($(v).attr('id'));
            });
            //controller name, method name, modal header
            ModalPopup('Modal', 'DMEditDept', 'Edit Record/s', idsArr);
        } else {
            alert("No data record/s selected.");
        }
    });

    $("#delete_dept").click(function (e) {
        e.stopImmediatePropagation();
        var chkCount = $('input.tbl-chk[type="checkbox"]:checked').length;
        if (!(chkCount <= 0)) {
            var idsArr = Array();
            $('input.tbl-chk[type="checkbox"]:checked').each(function (i, v) {
                idsArr.push($(v).attr('id'));
            });
            //controller name, method name, modal header
            ModalPopup('Modal', 'DMDeleteDept', 'Confirm deletion of record/s?', idsArr);
        } else {
            alert("No data record/s selected.");
        }
    });

    //clicks the hidden add row button inside the table
    //$("#add_row_btn").click(function () {
    //    $(".table-add").click();
    //});

    //set modal contents and show
    function ModalPopup(modal, method, modalHeader) {
        var modalDivBody = $('.modal-body');
        var modalDivHeader = $('.modal-header');
        //var modal1 = (a.indexOf('Add') > -1) ? $('#myModal') : $('#myModal2');
        $.ajax({
            url: "RedirectCont/",
            type: "POST",
            data: { Cont: modal, Method: method }, //set controller and method name to show
            success: function (data) {
                //remove prev contents
                modalDivBody.empty();
                modalDivHeader.find('h4').remove();

                //set modal header title
                modalDivHeader.append('<h4 class="modal-title">' + modalHeader + '</h4>');
                modalDivBody.html(data);

                $('#myModal').modal('show');
            },
            fail: function (ex) {
                alert("fail");
            }
        });
    }
    function ModalPopup(modal, method, modalHeader, idsArr) {
        var modalDivBody = $('.modal-body');
        var modalDivHeader = $('.modal-header');
        var modalDivFooter = $('.modal-footer');
        
        $.ajax({
            url: "RedirectCont2/",
            type: "POST",
            data: { Cont: modal, Method: method, IdsArr: idsArr }, //set controller and method name to show
            success: function (data) {
                //remove prev contents
                modalDivBody.empty();
                modalDivHeader.find('h4').remove();

                //set modal header title
                modalDivHeader.append('<h4 class="modal-title">' + modalHeader + '</h4>');
                if (RegExp('\\bAdd\\b', 'i').test(modalHeader) == true) {
                    modalDivFooter.find('#add_row_btn').remove();
                    modalDivFooter.append('<button type="button" id="add_row_btn" class="btn table-add">Add Row</button>');
                }
                modalDivBody.html(data);

                $('#myModal').modal('show');
            },
            fail: function (ex) {
                alert("fail");
            }
        });
    }
}); 