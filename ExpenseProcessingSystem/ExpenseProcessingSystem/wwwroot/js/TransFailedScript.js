$(document).ready(function () {
    var MSG1 = "There is a pending GOExpress tranctions that not yet processed in EXPRESS system. please wait for a while and try again later."
    var MSG2 = "Selected Transaction was processed by other user. Please refresh the page then try again."

    var WARNING1 = "This action will also re-send related transactions that has an ERROR status."
    var WARNING2 = "This action will also revert related transactions that has POSTED/RE-SENDING COMPLETE status."
    var WARNING3 = "This action will also re-send related transactions that has REVERSING ERROR status."

    $('table').on('click', '.btnCommand', function () {
        var pid = "#" + $(this.parentNode)[0].parentNode.id;
        var msg = "";
        var warning = [];
        //ExecuteAction(pid);
        if ($(pid).find('.actionID').val() == 1) {
            msg = "RE-SEND"
            warning.push(WARNING1);
        } else if ($(pid).find('.actionID').val() == 2) {
            msg = "REVERSE"
            warning.push(WARNING2);
        } else if ($(pid).find('.actionID').val() == 3) {
            msg = "RE-SEND REVERSING ERROR"
            warning.push(WARNING3);
        }
        ShowConfirmPopup(msg, warning, $(pid).find('.actionID').val());
        $('#selectedPID').val(pid);
    });

    $("table").on("click", ".btnStatus", function (e) {
        var pid = "#" + $(this.parentNode)[0].parentNode.id;
        $('#GBaseMessageModal').modal('show');
        $('#gbaseMsgArea').val($(pid).find('.gbaseMsg').val().replace(/<[^>]*>/g, ' ').replace('    ***', '\n***'));
    });

    $('.divConfirmfooter').on('click', '#btnProceedConfirm', function (e) {
        ExecuteAction($('#selectedPID').val());
        $('.btnCloseConfirm').trigger('click');
    });

    $('table').on('click', '#btnPageRefresh', function (e) {
        window.location.reload();
    });

    function ExecuteAction(pid) {
        loadingEffectStart();
        var currEntryID = $(pid).find('.TransEntryID').val();
        var currIsLiq = $(pid).find('.TransIsLiq').val();
        var currActCmd = $(pid).find('.actionID').val();
        //Return FALSE if there is a pending status in GOExpress that not yet updated in EXPRESS system side.
        $.getJSON('IsPendingTransactionInGOExpress', { "": "" }, function (data) {
            if (data) {
                ShowMessagePopup(MSG1);
                loadingEffectStop();
                return false;
            } else {
                //Return FALSE if selected transactions were not same status as listed in the screen.
                var errFlag = 0;
                $.getJSON('IsSameTransactionStatus', {
                    entryID: currEntryID,
                    IsLiq: currIsLiq
                }, function (data) {
                    var cnt = $('.TransEntryID').length;
                    for (var i = 0; i < cnt; i++) {
                        var transNo = $('#tbl_' + i).find('.TransListID').val();
                        var statudID = $('#tbl_' + i).find('.statusID').val();

                        $.each(data, function (index, item) {
                            if (item.tF_TRANS_LIST_ID == transNo && item.tF_STATUS_ID != statudID) {
                                ShowMessagePopup(MSG2);
                                loadingEffectStop();
                                errFlag = 1;
                                return false;
                            }
                        });
                    }
                    if (errFlag == 0) {
                        //Process Action
                        $.getJSON('ProcessActionButton', {
                            entryID: currEntryID,
                            actCmd: currActCmd,
                            IsLiq: currIsLiq
                        }, function (data) {
                            if (data) {
                                window.location.reload();
                            }
                        });
                    }
                });
            }
        });
        loadingEffectStop();
    }

    function ShowMessagePopup(msg) {
        $('#alert').text(msg);
        $('#divAlertWindow').show();
    }

    function ShowConfirmPopup(msg, warning, command) {
        OpenConfirmationPopup(msg, command, warning);
        $('#divConfirmWindow').fadeIn(100);
    }
});