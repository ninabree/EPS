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
    $(".apprv-rec").click(function (e) {
        e.stopImmediatePropagation();
        if (!isSessionTimeout()) {
            var methodName = getMethodName(this.id);
            var chkCount = $('input.tbl-chk[type="checkbox"]:checked').length;
            if (!(chkCount <= 0)) {
                var idsArr = Array();
                $('input.tbl-chk[type="checkbox"]:checked').each(function (i, v) {
                    idsArr.push($(v).attr('id'));
                });
                //controller name, method name, modal header
                ModalPopup2('Modal', 'DMApprove' + methodName, 'Approve Record/s', idsArr);
            } else {
                alert("No data record/s selected.");
            }
        }
    });

    $(".rej-rec").click(function (e) {
        e.stopImmediatePropagation();
        if (!isSessionTimeout()) {
            var methodName = getMethodName(this.id);
            var chkCount = $('input.tbl-chk[type="checkbox"]:checked').length;
            if (!(chkCount <= 0)) {
                var idsArr = Array();
                $('input.tbl-chk[type="checkbox"]:checked').each(function (i, v) {
                    idsArr.push($(v).attr('id'));
                });
                //controller name, method name, modal header
                ModalPopup2('Modal', 'DMRej' + methodName, 'Reject Record/s', idsArr);
            } else {
                alert("No data record/s selected.");
            }
        }

    });

    $(".add-rec").click(function (e) {
        e.stopImmediatePropagation();
        if (!isSessionTimeout()) {
            var methodName = getMethodName(this.id);
            ModalPopup('Modal', 'DMAdd' + methodName + '_Pending', 'Add New Record/s');
        }
    });

    $(".edit-rec").click(function (e) {
        e.stopImmediatePropagation();
        if (!isSessionTimeout()) {
            var methodName = getMethodName(this.id);

            var chkCount = $('input.tbl-chk[type="checkbox"]:checked').length;
            if (!(chkCount <= 0)) {
                var idsArr = Array();
                $('input.tbl-chk[type="checkbox"]:checked').each(function (i, v) {
                    idsArr.push($(v).attr('id'));
                });
                //controller name, method name, modal header
                ModalPopup2('Modal', 'DMEdit'+methodName+'_Pending', 'Edit Record/s', idsArr);
            } else {
                alert("No data record/s selected.");
            }
        }
    });

    $(".delete-rec").click(function (e) {
        e.stopImmediatePropagation();
        if (!isSessionTimeout()) {
            var methodName = getMethodName(this.id);
            var chkCount = $('input.tbl-chk[type="checkbox"]:checked').length;

            if (!(chkCount <= 0)) {
                var idsArr = Array();
                $('input.tbl-chk[type="checkbox"]:checked').each(function (i, v) {
                    idsArr.push($(v).attr('id'));
                });
                //controller name, method name, modal header
                ModalPopup2('Modal', 'DMDelete' + methodName + '_Pending', 'Confirm deletion of record/s?', idsArr);
            } else {
                alert("No data record/s selected.");
            }
        }
    });

    //-------------------------------------------FUNCTIONS-----------------------------------------------
    //get MethodName from button Id
    function getMethodName(id) {
        var splitid = id.split("_");
        var methodName = splitid[1].charAt(0).toUpperCase();
        methodName += splitid[1].substring(1);
        return methodName
    }
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
        var modalDivFooter = $('.modal-footer');

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
                modalDivFooter.find('#add_row_btn').remove();
                modalDivFooter.append('<button type="button" id="add_row_btn" class="btn table-add">Add Row</button>');
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
                modalDivBody.html(data);

                $('#myModal').modal('show');
            },
            fail: function (ex) {
                alert("fail");
            }
        });
    }
}); 