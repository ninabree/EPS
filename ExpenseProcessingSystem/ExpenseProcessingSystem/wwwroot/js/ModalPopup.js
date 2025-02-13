﻿$(function () {
    /*//////////////////////////////////////////////////////////////////
    [ MODAL ]*/
    //set content to modal body and show dynamic modal
    //LOGIN

    $("#inputTable").on("click", ".gRemarks", function (e) {
        e.stopImmediatePropagation();
        if (!isSessionTimeout()) {
            var idsArr = Array();
            var pNode = $(this.parentNode)[0].id;
            var screen = $("#_screen").val();
            idsArr.push(pNode);
            ModalPopup2('Modal', 'EntryGbaseRemarks', 'Gbase Remarks', idsArr, screen);

            var modalDivFooter = $('.modal-footer');
        }
    });

    $("#forgot_PW").click(function (e) {
        e.stopImmediatePropagation();
        ModalPopup('Modal', 'ForgotPW', 'Forgot Your Login Credentials?');
    });
    //Entry_DDV
    //$("#reversal_entry").click(function () {
    //    //controller name, method name, modal header
    //    ModalPopup('Modal', 'ReversalEntryModal', 'Reversal Entry');
    //});

    //DM
    $(document).on("click", ".apprv-rec", function (e) {
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

    $(document).on("click", ".rej-rec", function (e) {
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
    $(document).on("click", ".add-rec", function (e) {
        e.stopImmediatePropagation();
        if (!isSessionTimeout()) {
            var methodName = getMethodName(this.id);
            ModalPopup('Modal', 'DMAdd' + methodName + '_Pending', 'Add New Record/s');
        }
    });

    $(document).on("click", ".edit-rec", function (e) {
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

    $(document).on("click", ".delete-rec", function (e) {
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
        var tblName = $('#dm-tbl').find(":selected").text();
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
                //when in BCS and NCC, NO add row btn
                if (tblName != "BIR Cert Signatory")
                {
                    modalDivFooter.append('<button type="button" id="add_row_btn" class="btn table-add">Add Row</button>');
                }
                modalDivBody.html(data);

                $('#myModal').modal('show');
                //$("[aria-describedby='validationSummary']").remove();
                //$("div.ui-dialog.ui-corner-all.ui-widget.ui-widget-content.ui-front.msg_dlg.ui-draggable.ui-resizable").remove();
            },
            error: function (xhr) {
                    alert(xhr);
                }
        });
    }
    function ModalPopup2(modal, method, modalHeader, idsArr, screen) {
        var modalDivBody = $('.modal-body');
        var modalDivHeader = $('.modal-header');
        var modalDivFooter = $('.modal-footer');
        var isReadOnly = $("#" + idsArr[0]).find(".gRemarks").hasClass("gRemarksReadOnly");

        $.ajax({
            url: "RedirectCont2/",
            type: "POST",
            data: { Cont: modal, Method: method, IdsArr: idsArr }, //set controller and method name to show
            success: function (data) {
                //remove prev contents
                modalDivBody.empty();
                modalDivHeader.find('h4').remove();
                modalDivFooter.find('#add_row_btn').remove();

                //set modal header title
                modalDivHeader.append('<h4 class="modal-title">' + modalHeader + '</h4>');
                modalDivBody.html(data);

                if (method == 'EntryGbaseRemarks') {
                    $("#saveBtn").addClass("gBaseSaveBtn");

                    gbaseRemarksSet(idsArr[0], screen)

                    if ($("#add_row_btn").length <= 0 && !isReadOnly) {
                        modalDivFooter.append('<button type="button" id="add_row_btn" class="btn table-add">Add Row</button>');
                    } else if (isReadOnly) {
                        modalDivFooter.find("#add_row_btn").remove();
                        modalDivFooter.find("#saveGbase").remove();
                    }
                }

                $('#myModal').modal('show');
            },
            error: function (xhr) {
                alert('error');
            }

        });
    }

    function gbaseRemarksSet(id, screen) {
        var rowNo = id.substring(7);
        var noOfItem = ($("#" + id).find('input').length)/ 4;
        var tblDiv = $("#table").find("table");

        var screenCode = { cv: "#EntryCV_", ddv: "#EntryDDV_", cv_ViewOnly: "#EntryCV_", ddv_ViewOnly: "#EntryDDV_", liq_Dtl: "#LiquidationDetails_"};

        if (noOfItem > 0) {
            tblDiv.find('tbody').find('tr').remove();

            for (var i = 0; i < noOfItem; i++) {
                var docuType = $("#" + id).find(screenCode[screen] + rowNo + "__gBaseRemarksDetails_" + i + "__docType").val();
                var desc = $("#" + id).find(screenCode[screen] + rowNo + "__gBaseRemarksDetails_" + i + "__desc").val();
                var invNo = $("#" + id).find(screenCode[screen] + rowNo + "__gBaseRemarksDetails_" + i + "__invNo").val();
                var amount = $("#" + id).find(screenCode[screen] + rowNo + "__gBaseRemarksDetails_" + i + "__amount").val();

                var newGbaseRemarksRow = '<tr id="gRemarks-tr-' + i + '">'
                    + '<td><input type="text" class="gDocuType" value="' + docuType + '" /></td>'
                    + '<td><input type="text" class="gInvoiceNo" value="' + invNo + '" /></td>'
                    + '<td><input type="text" class="gDescription" value="' + desc + '" /></td>'
                    + '<td><input type="text" min="0" class="gAmount NumberOnlyOneDecimal" style="width:100%" value="' + amount + '"  /></td>'
                    + '<td><div class="flex-c">';
                if (!$("#" + id).find(".txtgbaseRemarks").prop('disabled') &&
                    $("#" + id).find(".hiddenScreencode").val() != "Liquidation_SS") {
                    if (i != 0) {
                        newGbaseRemarksRow = newGbaseRemarksRow + '<span class="table-remove glyphicon glyphicon-remove"></span>';
                    }
                }
                newGbaseRemarksRow = newGbaseRemarksRow + '</div></td></tr>';

                tblDiv.append($(newGbaseRemarksRow));
            }
        }
    }
}); 