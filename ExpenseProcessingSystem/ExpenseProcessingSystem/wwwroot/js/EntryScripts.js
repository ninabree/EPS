$(function () {
    var computeFunction = function (event) {
        if (event.target.classList.contains("comVar")) {
            computeValues(event.target.parentNode.parentNode);
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

        ajaxFormCall("/Home/GenerateVoucher").done(function (response) {
            $("#iframePreview").contents().find('html').html(response);
        });
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
    $("table").on("change", "input.chkVat", function (e) {
        var pNode = $(this.parentNode)[0].parentNode;

        var itemNo = pNode.id; //jquery obj
        var chkVatVal = $(this).is(':checked');
        if (chkVatVal) {
            $("#" + itemNo).find(".txtVat").attr("disabled", false);
        } else {
            $("#" + itemNo).find(".txtVat").attr("disabled", true);
        }
    });

    $("table").on("change", "input.chkEwt", function (e) {
        var pNode = $(this.parentNode)[0].parentNode;

        var itemNo = pNode.id; //jquery obj
        var chkEwtVal = $(this).is(':checked');
        if (chkEwtVal) {
            $("#" + itemNo).find(".txtEwt").attr("disabled", false);
        } else {
            $("#" + itemNo).find(".txtEwt").attr("disabled", true);
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
        var vendorId = { vendorID: $("#vendorName").val() };
        var payeeID = $("#payeeType option:selected").val();
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
        } else if (payeeID == "2") {
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
        var isInter = $("#" + pNode.id).find("#EntryDDV_" + pNode.id.substring(5) + "__inter_entity").is(':checked');
        var amounts = $("");
        var grossAmt = 0;
        var origGrossAmt = 0;
        //check if Inter Entity
        if (isInter) {
            var grossAmt = $("#" + pNode.id + " td .txtGross").val();
        } else {
            amounts = $("#" + pNode.id + " .amount");
            grossAmt = 0;
            origGrossAmt = $("#" + pNode.id + " .txtGross").val();
            for (var i = 0; i < amounts.length; i++) {
                grossAmt += Number(amounts[i].value);
            }

            //For PCS,SS - resetting of Cash breakdown list.
            if ($(".hiddenScreencode").val() == "PCV" || $(".hiddenScreencode").val() == "SS") {
                if (origGrossAmt != grossAmt) {
                    var ret = pNode.id.replace('item_', '');
                    $('#divCashBD_' + ret).empty();
                    $('#EntryCV_' + ret + '__modalInputFlag').val(0);
                }
            }
        }

        $("#" + pNode.id + " td .txtGross").val(grossAmt);

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

                $("#" + itemNo).find(".txtCredEwt").val(ewt);
                $("#" + itemNo).find(".txtCredCash").val(netEwt);
            } else {
                var ewtAmount = grossAmt * (Number($("#" + itemNo).find(".txtEwt option:selected").text()) / 100);
                $("#" + itemNo).find(".txtCredEwt").val(roundNumber(ewtAmount, 2));
                $("#" + itemNo).find(".txtCredCash").val(roundNumber((grossAmt - ewtAmount), 2));
            }
        } else {
            $("#" + itemNo).find(".txtCredEwt").val(0);
            $("#" + itemNo).find(".txtCredCash").val(grossAmt);
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
        $("#credTotal").val(Number(ewtSubTotal + cashSubTotal));
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