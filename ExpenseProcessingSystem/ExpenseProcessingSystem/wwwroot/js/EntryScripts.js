$(function () {
    var computeFunction = function (event) {
        if (event.target.classList.contains("comVar")) {
           computeValues(event.target.parentNode.parentNode);
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
    //$(".modal-footer").on("click", "button.gBaseSaveBtn", function (e) {
    //    e.stopImmediatePropagation();
    //    alert();
    //});
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
        var parent = $("#" + $("#parentId").val());

        var trs = $("#gBaseTable").find("tbody").find("tr");
        var htmlText = "";

        if ($(this).hasClass("btn float-r gBaseSaveBtn")) {
            //to stop form submit if incomplete
            if (!checkFormComplete(trs, "GBase Remarks")) {
                return;
            }
        }

        parent.find(":hidden").remove();

        var rowNo = $("#parentId").val().substring(7);

        for (var i = 0; i < trs.length; i++) {
            var docuType = $("#" + trs[i].id).find(".gDocuType").val();
            var invNo = $("#" + trs[i].id).find(".gInvoiceNo").val();
            var desc = $("#" + trs[i].id).find(".gDescription").val();
            var amount = $("#" + trs[i].id).find(".gAmount").val();

            htmlText += '<input class="docType" id="EntryCV_' + rowNo + '__gBaseRemarksDetails_' + i + '__docType" name="EntryCV[' + rowNo + '].gBaseRemarksDetails[' + i + '].docType" type="hidden" value="' + docuType + '">';
            htmlText += '<input class="desc" id="EntryCV_' + rowNo + '__gBaseRemarksDetails_' + i + '__desc" name="EntryCV[' + rowNo + '].gBaseRemarksDetails[' + i + '].desc" type="hidden" value="' + desc + '">';
            htmlText += '<input class="invNo" id="EntryCV_' + rowNo + '__gBaseRemarksDetails_' + i + '__invNo" name="EntryCV[' + rowNo + '].gBaseRemarksDetails[' + i + '].invNo" type="hidden" value="' + invNo + '">';
            htmlText += '<input class="amount" data-val="true" data-val-number="The field amount must be a number." data-val-required="The amount field is required." id="EntryCV_' + rowNo + '__gBaseRemarksDetails_' + i + '__amount" name="EntryCV[' + rowNo + '].gBaseRemarksDetails[' + i + '].amount" type="hidden" value="' + amount + '">';
        }
        $("#" + $("#parentId").val()).append(htmlText);
        $('#myModal').modal('hide');
        +
            +        computeValues($("#item_" + rowNo)[0]);
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

        var amounts = $("#" + pNode.id + " .amount");
        var grossAmt = 0;

        for (var i = 0; i < amounts.length; i++) {
            grossAmt += Number(amounts[i].value);
        }

        $("#" + pNode.id + " .txtGross").attr("value", grossAmt);

        var itemNo = pNode.id; //jquery obj
        var chkEwtVal = $("#" + itemNo).find(".chkEwt").is(':checked');

        if (chkEwtVal) {
            var ewtAmount = grossAmt * (Number($("#" + itemNo).find(".txtEwt option:selected").text()) / 100);
            $("#" + itemNo).find(".txtCredEwt").val(roundNumber(ewtAmount, 2));
            $("#" + itemNo).find(".txtCredCash").val(roundNumber((grossAmt - ewtAmount), 2));
        } else {
            $("#" + itemNo).find(".txtCredEwt").val(0);
            $("#" + itemNo).find(".txtCredCash").val(grossAmt);
        }

        var chkVatVal = $("#" + itemNo).find(".chkVat").is(':checked');

        if (chkVatVal) {
            var vatAmount = grossAmt * (Number($("#" + itemNo).find(".txtVat option:selected").text()) / 100);
            $("#" + itemNo).find(".txtCredEwt").val(roundNumber(vatAmount, 2));
            $("#" + itemNo).find(".txtCredCash").val(roundNumber((grossAmt - vatAmount), 2));
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

});