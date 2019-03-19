$(function () {
    /*//////////////////////////////////////////////////////////////////
    [ MODAL ]*/
    //set content to modal body and show dynamic modal
    //LOGIN
    $("#forgot_PW").click(function (e) {
        e.stopImmediatePropagation();
        ModalPopup('Modal', 'ForgotPW', 'Forgot Your Login Credentials?');
    });
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
    //DM
    $("#apprv_payee").click(function (e) {
        e.stopImmediatePropagation();
        if (!isSessionTimeout()) {
            var chkCount = $('input.tbl-chk[type="checkbox"]:checked').length;
            if (!(chkCount <= 0)) {
                var idsArr = Array();
                $('input.tbl-chk[type="checkbox"]:checked').each(function (i, v) {
                    idsArr.push($(v).attr('id'));
                });
                //controller name, method name, modal header
                ModalPopup2('Modal', 'DMApprovePayee', 'Approve Record/s', idsArr);
            } else {
                alert("No data record/s selected.");
            }
        }
    });

    $("#rej_payee").click(function (e) {
        e.stopImmediatePropagation();
        if (!isSessionTimeout()) {
            var chkCount = $('input.tbl-chk[type="checkbox"]:checked').length;
            if (!(chkCount <= 0)) {
                var idsArr = Array();
                $('input.tbl-chk[type="checkbox"]:checked').each(function (i, v) {
                    idsArr.push($(v).attr('id'));
                });
                //controller name, method name, modal header
                ModalPopup2('Modal', 'DMRejPayee', 'Reject Record/s', idsArr);
            } else {
                alert("No data record/s selected.");
            }
        }

    });

    $("#add_payee").click(function (e) {
        e.stopImmediatePropagation();
        if (!isSessionTimeout()) {
            //controller name, method name, modal header
            ModalPopup('Modal', 'DMAddPayee_Pending', 'Add New Record/s');
        }
    });

    $("#edit_payee").click(function (e) {
        e.stopImmediatePropagation();
        if (!isSessionTimeout()) {
            var chkCount = $('input.tbl-chk[type="checkbox"]:checked').length;
            if (!(chkCount <= 0)) {
                var idsArr = Array();
                $('input.tbl-chk[type="checkbox"]:checked').each(function (i, v) {
                    idsArr.push($(v).attr('id'));
                });
                //controller name, method name, modal header
                ModalPopup2('Modal', 'DMEditPayee', 'Edit Record/s', idsArr);
            } else {
                alert("No data record/s selected.");
            }
        }
    });

    $("#delete_payee").click(function (e) {
        e.stopImmediatePropagation();
        if (!isSessionTimeout()) {
            var chkCount = $('input.tbl-chk[type="checkbox"]:checked').length;
            if (!(chkCount <= 0)) {
                var idsArr = Array();
                $('input.tbl-chk[type="checkbox"]:checked').each(function (i, v) {
                    idsArr.push($(v).attr('id'));
                });
                //controller name, method name, modal header
                ModalPopup2('Modal', 'DMDeletePayee', 'Confirm deletion of record/s?', idsArr);
            } else {
                alert("No data record/s selected.");
            }
        }
    });

    $("#add_dept").click(function (e) {
        e.stopImmediatePropagation();
        if (!isSessionTimeout()) {
            //controller name, method name, modal header
            ModalPopup('Modal', 'DMAddDept', 'Add New Record/s');
        }
    });

    $("#edit_dept").click(function (e) {
        e.stopImmediatePropagation();
        if (!isSessionTimeout()) {
            var chkCount = $('input.tbl-chk[type="checkbox"]:checked').length;
            if (!(chkCount <= 0)) {
                var idsArr = Array();
                $('input.tbl-chk[type="checkbox"]:checked').each(function (i, v) {
                    idsArr.push($(v).attr('id'));
                });
                //controller name, method name, modal header
                ModalPopup2('Modal', 'DMEditDept', 'Edit Record/s', idsArr);
            } else {
                alert("No data record/s selected.");
            }
        }
    });

    $("#delete_dept").click(function (e) {
        e.stopImmediatePropagation();
        if (!isSessionTimeout()) {
            var chkCount = $('input.tbl-chk[type="checkbox"]:checked').length;
            if (!(chkCount <= 0)) {
                var idsArr = Array();
                $('input.tbl-chk[type="checkbox"]:checked').each(function (i, v) {
                    idsArr.push($(v).attr('id'));
                });
                //controller name, method name, modal header
                ModalPopup2('Modal', 'DMDeleteDept', 'Confirm deletion of record/s?', idsArr);
            } else {
                alert("No data record/s selected.");
            }
        }
    });

    //check session timout
    function isSessionTimeout() {
        var tmp = false;
        $.ajax({
            url: '/Account/checkSession',
            type: "POST",
            dataType: 'json',
            contentType: 'application/json',
            async: false,
            processData: false,
            cache: false,
            data: '{}',
            success: function (data) {
                //FOR SESSION TIMEOUT CHECK
                if (data == true) {
                    alert("Your session has timed out. Kindly re-login. Thank you.");
                    tmp = true;
                    location.href = "/Account/Login";
                }
            },
            error: function (xhr) {
                alert('error');
            }
        });
        return tmp;
    }
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
                modalDivBody.addClass("p-l-15 p-r-15");
                modalDivBody.html(data);

                $('#myModal').modal('show');
            },
            error: function (xhr) {
                    alert(xhr);
                }
        });
    }
    function ModalPopup2(modal, method, modalHeader, idsArr) {
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