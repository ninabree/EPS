$(function () {
    var computeFunction = function (event) {
        if (event.target.classList.contains("comVar")) {
            computeValues(event.target.parentNode.parentNode);
        }
        if (event.target.classList.contains("comVarDDV")) {
            computeValuesDDV(event.target.parentNode.parentNode);
        }
        var payee = $("#vendorName option:selected").text();
        var amount = $("#grossTotal").val();
        var checkNo = $("#checkNo").val();
        var voucherNo = $("#voucherNo").val();
        var date = $("#entryDate").val();

        var data = {
            "headvm": {
                "header_Logo": null,
                "header_Name": null,
                "header_TIN": null,
                "header_Address": null
            },
            "payeeID": 0,
            "payee": payee,
            "amount": amount,
            "amountString": null,
            "checkNo": checkNo,
            "voucherNo": voucherNo,
            "makeriD": 0,
            "maker": null,
            "approver": null,
            "verifier_1": null,
            "verifier_2": null,
            "date": date
        }

        var screen = $("#_screen").val();

        var myform = document.getElementById("inputForm");
        var data = new FormData(myform);

        if (screen == "cv" || screen == "ddv") {

            $.ajax({
                type: 'POST',
                url: "/Home/VoucherCV",
                data: $("#inputForm").serialize(),
                success: function (result) {
                    $("#iframePreview").contents().find('html').html(result)
                }
            });
        }

        if (screen == "ddv") {

            $.ajax({
                type: 'POST',
                url: "/Home/VoucherDDV",
                data: $("#inputForm").serialize(),
                success: function (result) {
                    $("#iframePreview").contents().find('html').html(result)
                }
            });
        }
    }

    $(".tabContent").keypress(
        function (event) {
            if (event.which == '13') {
                event.preventDefault();
                var $canfocus = $(':tabbable:visible')
                var index = $canfocus.index(document.activeElement) + 1;
                if (index >= $canfocus.length) index = 0;
                $canfocus.eq(index).focus();
            }
        });

    $("#entry-controls").on("click", "#printVoucher", function (e) {
        $("#iframePreview").get(0).contentWindow.print();
        return false;
    });

    $("table").on("change", "input.chkVat", function (e) {
        var pNode = $(this.parentNode)[0].parentNode;

        var itemNo = pNode.id; //jquery obj
        var chkVatVal = $(this).is(':checked');
        if (chkVatVal) {
            $("#" + itemNo).find(".txtVat").attr("disabled", false);
            $('#' + itemNo).find('.reqEWTBtn').css("pointer-events", "auto");
        } else {
            $("#" + itemNo).find(".txtVat").attr("disabled", true);
            if (!$("#" + itemNo).find(".chkEwt").is(':checked')) {
                $('#' + itemNo).find('.reqEWTBtn').css("pointer-events", "none");
            }
        }
    });

    $("table").on("change", "input.chkEwt", function (e) {
        var pNode = $(this.parentNode)[0].parentNode;

        var itemNo = pNode.id; //jquery obj
        var chkEwtVal = $(this).is(':checked');
        if (chkEwtVal) {
            $("#" + itemNo).find(".txtEwt").attr("disabled", false);
            $('#' + itemNo).find('.reqEWTBtn').css("pointer-events", "auto");
        } else {
            $("#" + itemNo).find(".txtEwt").attr("disabled", true);
            if (!$("#" + itemNo).find(".chkVat").is(':checked')) {
                $('#' + itemNo).find('.reqEWTBtn').css("pointer-events", "none");
            }
        }
    });

    document.addEventListener("change", computeFunction, false);

    $("#modalDiv").on("click", "#saveGbase", function (e) {
        if ($("#parentIdAmortization").length) {
            return;
        }
        if ($("#parentIdPCVSSTable").length) {
            return;
        }
        var parent = $("#" + $("#parentId").val());

        var trs = $("#gBaseTable").find("tbody").find("tr");
        var htmlText = "";
        
        //to stop form submit if incomplete
        if (!checkFormComplete(trs, "GBase Remarks")) {
            return;
        }

        parent.find(":hidden").remove();

        var rowNo = $("#parentId").val().substring(7);
        var isDDV = $("#expenseType").val() == "DDV";
        for (var i = 0; i < trs.length; i++) {
            var docuType = $("#" + trs[i].id).find(".gDocuType").val();
            var invNo = $("#" + trs[i].id).find(".gInvoiceNo").val();
            var desc = $("#" + trs[i].id).find(".gDescription").val();
            var amount = $("#" + trs[i].id).find(".gAmount").val();
            if (isDDV) {
                htmlText += '<input class="docType" id="EntryDDV_' + rowNo + '__gBaseRemarksDetails_' + i + '__docType" name="EntryDDV[' + rowNo + '].gBaseRemarksDetails[' + i + '].docType" type="hidden" value="' + docuType + '">';
                htmlText += '<input class="desc" id="EntryDDV_' + rowNo + '__gBaseRemarksDetails_' + i + '__desc" name="EntryDDV[' + rowNo + '].gBaseRemarksDetails[' + i + '].desc" type="hidden" value="' + desc + '">';
                htmlText += '<input class="invNo" id="EntryDDV_' + rowNo + '__gBaseRemarksDetails_' + i + '__invNo" name="EntryDDV[' + rowNo + '].gBaseRemarksDetails[' + i + '].invNo" type="hidden" value="' + invNo + '">';
                htmlText += '<input class="amount" data-val="true" data-val-number="The field amount must be a number." data-val-required="The amount field is required." id="EntryDDV_' + rowNo + '__gBaseRemarksDetails_' + i + '__amount" name="EntryDDV[' + rowNo + '].gBaseRemarksDetails[' + i + '].amount" type="hidden" value="' + amount + '">';
            } else {
                htmlText += '<input class="docType" id="EntryCV_' + rowNo + '__gBaseRemarksDetails_' + i + '__docType" name="EntryCV[' + rowNo + '].gBaseRemarksDetails[' + i + '].docType" type="hidden" value="' + docuType + '">';
                htmlText += '<input class="desc" id="EntryCV_' + rowNo + '__gBaseRemarksDetails_' + i + '__desc" name="EntryCV[' + rowNo + '].gBaseRemarksDetails[' + i + '].desc" type="hidden" value="' + desc + '">';
                htmlText += '<input class="invNo" id="EntryCV_' + rowNo + '__gBaseRemarksDetails_' + i + '__invNo" name="EntryCV[' + rowNo + '].gBaseRemarksDetails[' + i + '].invNo" type="hidden" value="' + invNo + '">';
                htmlText += '<input class="amount" data-val="true" data-val-number="The field amount must be a number." data-val-required="The amount field is required." id="EntryCV_' + rowNo + '__gBaseRemarksDetails_' + i + '__amount" name="EntryCV[' + rowNo + '].gBaseRemarksDetails[' + i + '].amount" type="hidden" value="' + amount + '">';
            }
        }
        $("#" + $("#parentId").val()).append(htmlText);
        $('#myModal').modal('hide');
        computeValues($("#item_" + rowNo)[0]);
        computeFunction(e);
    });
    $("#vendorName").on("change", function (e) {
        if ($('.hiddenScreencode').val() == "PCV" || $('.hiddenScreencode').val() == "PC") {
            return false;
        }
        var vendorId = { vendorID: $("#vendorName").val() };
        var payeeID = $("#payee_type").val();
        if ($("#payeeTypeSel").length > 0) {
            payeeID = $("#payeeTypeSel option:selected").val();
        }
        if (payeeID == "1") {
            ajaxCall("/Home/getVendorVatList", vendorId)
                .done(function (vatData) {
                    if (vatData.length) {
                        ajaxCall("/Home/getVendorTRList", vendorId)
                            .done(function (ewtData) {
                                if (ewtData.length) {
                                    $(".txtVat").empty();
                                    $(".txtEwt").empty();

                                    for (var i = 0; i < vatData.length; i++) {
                                        var option = $("<option></option>").attr("value", vatData[i].vaT_ID).text(vatData[i].vaT_Rate);
                                        $(".txtVat").append(option);
                                    }

                                    for (var i = 0; i < ewtData.length; i++) {
                                        var option = $("<option></option>").attr("value", ewtData[i].tR_ID).text(ewtData[i].tR_Tax_Rate);
                                        $(".txtEwt").append(option);
                                    }
                                } else {
                                    alert("oops something went wrong!");
                                }
                            });
                    } else {
                        alert("oops something went wrong!");
                    }
                });
        } else if (payeeID == "2" || payeeID == "5") {
            ajaxCall("/Home/getAllVatList")
                .done(function (vatData) {
                    if (vatData.length) {
                        ajaxCall("/Home/getAllTRList")
                            .done(function (ewtData) {
                                if (ewtData.length) {
                                    $(".txtVat").empty();
                                    $(".txtEwt").empty();

                                    for (var i = 0; i < vatData.length; i++) {
                                        var option = $("<option></option>").attr("value", vatData[i].value).text(vatData[i].text);
                                        $(".txtVat").append(option);
                                    }

                                    for (var i = 0; i < ewtData.length; i++) {
                                        var option = $("<option></option>").attr("value", ewtData[i].value).text(ewtData[i].text);
                                        $(".txtEwt").append(option);
                                    }
                                } else {
                                    alert("oops something went wrong!");
                                }
                            });
                    } else {
                        alert("oops something went wrong!");
                    }
                });
        }
    });
    
    $('.btnEntryAction').click(function (e) {
        e.stopImmediatePropagation();
        loadingEffectStart();
        var msg = "";
        var warning = [];
        var command = $(this).val();
        if (command == "approver") {
            msg = "Approval";
            warning = response;
            OpenConfirmationPopup(msg, command, warning);
            $('#divConfirmWindow').fadeIn(100);
            loadingEffectStop();

        } else if (command == "verifier") {
            msg = "Verify";
            warning.push("TEST WARning");
            OpenConfirmationPopup(msg, command, warning);
            $('#divConfirmWindow').fadeIn(100);
            loadingEffectStop();
        } else {
            msg = command;
            OpenConfirmationPopup(msg, command, warning);
            $('#divConfirmWindow').fadeIn(100);
            loadingEffectStop();
        }

        return false;
    });
    /////--------------------functions--------------------------------
    function checkFormComplete(trs, formName) {
        var isComplete = true;
        if (trs.length <= 0) {
            alert("Can't submit empty form.");
            return;
        } else {
            $(trs).each(function (i, obj) {
                $(obj).find("input").each(function (i, val) {
                    if ($(val).val() == "") {
                        isComplete = false;
                        alert(formName + " entries should not be empty.");
                        return false;
                    }
                });
                if (!isComplete) {
                    return false;
                }
            });
        }
        return isComplete;
    }

    function computeValues(parent) {
        var pNode = parent;
        var amounts = $("");
        var grossAmt = 0;
        var origGrossAmt = 0;
        var origCredAmt = $("#" + itemNo).find(".txtCredCash").val();

        amounts = $("#" + pNode.id + " .amount");
        grossAmt = 0;
        origGrossAmt = $("#" + pNode.id + " .txtGross").val();
        for (var i = 0; i < amounts.length; i++) {
            grossAmt += Number(amounts[i].value);
        }

        $("#" + pNode.id + " td .txtGross").val(roundNumber(grossAmt,2));
        //if ($(".hiddenScreencode").val() == "SS") {
        //    $("#" + pNode.id).find(".hidDebit").val(grossAmt);

        //    return false;
        //}
        //$("#" + pNode.id + " .txtGross").attr("value", grossAmt);

        var itemNo = pNode.id; //jquery obj
        var chkEwtVal = $("#" + itemNo).find(".chkEwt").is(':checked');
        var vatable = $("#" + itemNo).find(".chkVat").is(':checked');
        if (chkEwtVal) {
            if (vatable) {
                var vatRate = (Number($("#" + itemNo).find(".txtVat option:selected").text()) / 100);
                var ewtRate = (Number($("#" + itemNo).find(".txtEwt option:selected").text()) / 100);
                var netVat = roundNumber(grossAmt / (1 + vatRate),2);
                var ewt = roundNumber(netVat * ewtRate,2);
                var netEwt = grossAmt - ewt;

                $("#" + itemNo).find(".txtCredEwt").val(roundNumber(ewt, 2));
                $("#" + itemNo).find(".txtCredCash").val(roundNumber(netEwt, 2));
                if ($(".hiddenScreencode").val() == "SS") {
                    $("#" + itemNo).find(".txtCredEwt").val(0);
                    $("#" + pNode.id).find(".txtGross").val(roundNumber(netEwt, 2));
                }
            } else {
                var ewtAmount = grossAmt * (Number($("#" + itemNo).find(".txtEwt option:selected").text()) / 100);
                $("#" + itemNo).find(".txtCredEwt").val(roundNumber(ewtAmount, 2));
                $("#" + itemNo).find(".txtCredCash").val(roundNumber((grossAmt - ewtAmount), 2));
                if ($(".hiddenScreencode").val() == "SS") {
                    $("#" + itemNo).find(".txtCredEwt").val(0);
                    $("#" + pNode.id).find(".txtGross").val(roundNumber((grossAmt - ewtAmount), 2));
                }
            }
        } else {
            $("#" + itemNo).find(".txtCredEwt").val(0);
            $("#" + itemNo).find(".txtCredCash").val(roundNumber(grossAmt, 2));
            if ($(".hiddenScreencode").val() == "SS") {
                $("#" + pNode.id).find(".txtGross").val(roundNumber(grossAmt, 2));
            }
        }

        //For PCS,SS - resetting of Cash breakdown list.
        if ($(".hiddenScreencode").val() == "PCV" || $(".hiddenScreencode").val() == "SS") {
            if (origCredAmt != $("#" + itemNo).find(".txtCredCash").val()) {
                var ret = pNode.id.replace('item_', '');
                $('#divCashBD_' + ret).empty();
                $('#EntryCV_' + ret + '__modalInputFlag').val(0);
            }
        }

        var gross = $(".txtGross");
        var credEwt = $(".txtCredEwt");
        var credCash = $(".txtCredCash");

        var grossTotal = 0;
        var ewtSubTotal = 0;
        var cashSubTotal = 0;

        for (var i = 0; i < gross.length; i++) {
            grossTotal += Number(gross[i].value);
        }

        for (var i = 0; i < credEwt.length; i++) {
            ewtSubTotal += Number(credEwt[i].value);
        }

        for (var i = 0; i < credCash.length; i++) {
            cashSubTotal += Number(credCash[i].value);
        }

        $("#grossTotal").val(grossTotal);
        $("#credEwtTotal").val(ewtSubTotal);
        $("#credCashTotal").val(cashSubTotal);
        $("#credTotal").val(roundNumber(Number(ewtSubTotal + cashSubTotal),2));
    }

    function ajaxCall(url, data) {
        return $.ajax({
            url: url,
            type: "POST",
            data: data,
        });
    }

    function ajaxFormCall(url) {
        return $.ajax({
            url: url,
            type: "POST",
        });
    }
});