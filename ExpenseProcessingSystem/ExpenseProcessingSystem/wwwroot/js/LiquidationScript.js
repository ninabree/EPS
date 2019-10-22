function round(value, exp) {
    if ($('.highlight').find('.txtCcyAbbrev').val() == getXMLLiqValue("CURRENCY_Yen")) {
        exp = 0;
    };

    if (typeof exp === 'undefined' || +exp === 0)
        return Math.round(value);

    value = +value;
    exp = +exp;

    if (isNaN(value) || !(typeof exp === 'number' && exp % 1 === 0))
        return NaN;

    // Shift
    value = value.toString().split('e');
    value = Math.round(+(value[0] + 'e' + (value[1] ? (+value[1] + exp) : exp)));

    // Shift back
    value = value.toString().split('e');
    return +(value[0] + 'e' + (value[1] ? (+value[1] - exp) : -exp));
};

function roundExceptionJPY(value, exp) {
    if (typeof exp === 'undefined' || +exp === 0)
        return Math.round(value);

    value = +value;
    exp = +exp;

    if (isNaN(value) || !(typeof exp === 'number' && exp % 1 === 0))
        return NaN;

    // Shift
    value = value.toString().split('e');
    value = Math.round(+(value[0] + 'e' + (value[1] ? (+value[1] + exp) : exp)));

    // Shift back
    value = value.toString().split('e');

    var res = +(value[0] + 'e' + (value[1] ? (+value[1] - exp) : -exp));
    if (parseFloat(res) % 1 == 0 && $('.highlight').find('.currMasterID').val() != getXMLLiqValue("CURRENCY_Yen")) {
        res = res + ".00";
    }
    return res;
};

function reqBtnDisable(idx) {
    if ($('#LiquidationDetails_' + idx + '__ccyAbbrev').val() != $('#phpAbbrv').val()) {
        $('#req_' + idx).find('.reqBtn').css("pointer-events", "none");
    } else {
        $('#req_' + idx).find('.reqBtn').css("pointer-events", "auto");
    }
    var pid = "#item_" + idx;
    if (!$(pid).find(".chkEwt").is(':checked')) {
        if ($(pid).find(".chkVat").is(':checked')) {
            $(pid).find('.reqEWTBtn').css("pointer-events", "auto");
        } else {
            $(pid).find('.reqEWTBtn').css("pointer-events", "none");
        }
    } else {
        $(pid).find('.reqEWTBtn').css("pointer-events", "auto");
    }

    if (!$(pid).find(".chkVat").is(':checked')) {
        if ($(pid).find(".chkEwt").is(':checked')) {
            $(pid).find('.reqEWTBtn').css("pointer-events", "auto");
        } else {
            $(pid).find('.reqEWTBtn').css("pointer-events", "none");
        }
    } else {
        $(pid).find('.reqEWTBtn').css("pointer-events", "auto");
    }
};

function removeDecimalYEN(idx) {
    if ($('#LiquidationDetails_' + idx + '__ccyAbbrev').val() == $('#yenAbbrv').val()) {
        $('#item_' + idx).find('.txtGross').val(AC($('#item_' + idx).find('.txtGross').val()).replace(".00", ""));
        $('#item_' + idx).find('.txtCredCash').val(AC($('#item_' + idx).find('.txtCredCash').val()).replace(".00", ""));
    } else {
        $('#item_' + idx).find('.txtGross').val(AC($('#item_' + idx).find('.txtGross').val()));
        $('#item_' + idx).find('.txtCredCash').val(AC($('#item_' + idx).find('.txtCredCash').val()));
    }
}

function displayInfoCashBD(pid) {

    $('#txtLiqAccNo').val($('#' + pid).find('.hidAccNo').val());
    $('#txtLiqAccCode').val($('#' + pid).find('.hidAccCode').val());

    $('#ddlLiqPhpAccount2').empty();
    $('#ddlLiqPhpAccount2').append($('<option></option>').val($('#lblAccount1_PHP optgroup[label=' + getXMLLiqValue('ACCOUNT11_1') + '] option:first').val()).html($('#lblAccount1_PHP optgroup[label=' + getXMLLiqValue('ACCOUNT11_1') + '] option:first').text()));
    $('#ddlLiqPhpAccount2').append($('<option></option>').val($('#lblAccount1_PHP optgroup[label=' + getXMLLiqValue('ACCOUNT11_2') + '] option:first').val()).html($('#lblAccount1_PHP optgroup[label=' + getXMLLiqValue('ACCOUNT11_2') + '] option:first').text()));

    $('#lblAccount1_PHP optgroup[label=' + getXMLLiqValue("ACCOUNT1_PHP") + '] option:first').prop('selected', true);
    //$('#lblAccount2_PHP optgroup[label=' + getXMLLiqValue("ACCOUNT2_PHP") + '] option:first').prop('selected', true);

    $('.tableLiqPhp').find('.txtLiqPhpInput').val(0);
    $('#ddlLiqPhpAccount1').val($("#ddlLiqPhpAccount1 option:first").val());
    $('#ddlLiqPhpAccount2').val($("#ddlLiqPhpAccount2 option:first").val());
    $('#txtLiqTaxRate').val($("#txtLiqTaxRate option:first").val());

    var ret = pid.replace('item_', '');

    if ($('#' + pid).find('.ewtID').val() > 0) {
        $('#txtLiqTaxRate').removeAttr('disabled');
        $('#txtLiqPhpInput4').removeAttr('disabled');
        getVendorTaxRate($('#' + pid).find('.txtPayor').val(), ret);
    } else {
        $('#txtLiqTaxRate').empty();
        $('#txtLiqTaxRate').append($('<option/>', {
            value: 0,
            text: "0%"
        }));
        $('#txtLiqTaxRate').attr('disabled', 'disabled');
        $('#txtLiqPhpInput4').attr('disabled', 'disabled');
    }
    

    if ($('#' + pid).find('.hiddenLiqFlag').val() == 0) {
        return false;
    }

    var cashBDInput = $('#divLiqCashBD_' + ret).find("input");
    var totalAmount = 0.0;

    for (var count = 0; count < cashBDInput.length / 3; count++) {
        $('#cashbdliqid_' + (20 + count)).find('.txtLiqDenomination').val($('#LiquidationDetails_' + ret + '__liqCashBreakdown_' + count + '__cashDenomination').val().replace(".00", ""));
        $('#cashbdliqid_' + (20 + count)).find('.txtLiqNoPCS').val($('#LiquidationDetails_' + ret + '__liqCashBreakdown_' + count + '__cashNoPC').val());
        $('#cashbdliqid_' + (20 + count)).find('.txtLiqAmount').val(AC($('#LiquidationDetails_' + ret + '__liqCashBreakdown_' + count + '__cashAmount').val().replace(".00", "")));

        totalAmount += parseFloat(RC($('#LiquidationDetails_' + ret + '__liqCashBreakdown_' + count + '__cashAmount').val()));
    }
    $('#txtCashBDTotal').val(AC(totalAmount));

    assignDivValuesLiqPhp(pid)
};

function displayInfoIE(pid) {

    $('.lblIEInput').text("");
    $('.txtIEExRatePhp').text("");
    $('.txtIEInfoInput').val(0);
    $('.txtIEInput').val(0);

    $('#txtIECurrency').val($('#' + pid).find('.txtCcyAbbrev').val());
    $('#txtIEAmount').val(AC($('#' + pid).find('.txtGross').val()));
    var curr = "";
    if ($('#' + pid).find('.currMasterID').val() == getXMLLiqValue("CURRENCY_Yen")) {
        curr = "_Yen";
    } else {
        curr = "_US";
    }

    for (var a = 1; a <= 9; a++) {
        $('#lblAccount' + a + ' optgroup[label=' + getXMLLiqValue('ACCOUNT' + a + curr) + '] option:first').prop('selected', true);
    }

    $('.lblParticular').text("S" + $('#' + pid).find('.txtGBaseRemark').val());
    $('.lblCurrency').text($('#' + pid).find('.txtCcyAbbrev').val());
    $('#ddlAccount11').empty();

    $('#ddlAccount11').append($('<option></option>').val($('#lblAccount1 optgroup[label=' + getXMLLiqValue('ACCOUNT11_1') + '] option:first').val()).html($('#lblAccount1 optgroup[label=' + getXMLLiqValue('ACCOUNT11_1') + '] option:first').text()));
    $('#ddlAccount11').append($('<option></option>').val($('#lblAccount1 optgroup[label=' + getXMLLiqValue('ACCOUNT11_2') + '] option:first').val()).html($('#lblAccount1 optgroup[label=' + getXMLLiqValue('ACCOUNT11_2') + '] option:first').text()));

    if ($('#' + pid).find('.hiddenLiqFlag').val() == "2") {
        assignDivValuesIE(pid);
    }
};

function printDivCashLiqBD(divID) {

    //Set Cash Breakdown Liquidation table value(property) to table attribute value.
    var tableCntLiq = $('#cashBDLiqTable').find('tbody').find('tr');
    for (var count = 0; count < tableCntLiq.length - 1; count++) {
        $('#cashbdliqid_' + (20 + count)).find('.txtLiqNoPCS').attr('value', $('#cashbdliqid_' + (20 + count)).find('.txtLiqNoPCS').val());
        $('#cashbdliqid_' + (20 + count)).find('.txtLiqAmount').attr('value', $('#cashbdliqid_' + (20 + count)).find('.txtLiqAmount').val());
    }
    $('#txtCashBDTotal').attr('value', $('#txtCashBDTotal').val());
    var d = new Date($('#LiqEntryDetails_Liq_Created_Date').val());
    $('#cashbdDatePrintLiq').attr('value', (d.getMonth() + 1) + '/' + d.getDate() + '/' + d.getFullYear());
    $('#cashbdAmountPrintLiq').attr('value', $('#txtCashBDTotal').val());
    $('#paymentForLiq').attr('value', "S" + $('.highlight').find('.txtGBaseRemark').val());

    var cssjs = '<style>.tablePrintLiq, .tdPrintLiq, .thPrintLiq {  ' +
        '  border: 1px solid #ddd;' +
        '  text-align: center;' +
        '  width: 60%;' +
        '  height: auto;' +
        '  table-layout: fixed;' +
        '}' +

        'table {' +
        '  border-collapse: collapse;' +
        '}' +

        '.tdPrintLiq, .thPrintLiq {' +
        '  width: 20%;' +
        '}' +

        'input {' +
        '	outline:none;' +
        '    border:none;' +
        '}' +
        '</style>';
    
    var printContent = document.getElementById(divID);

    var WinPrint = window.open('', '', 'width=900,height=650');
    WinPrint.document.write(printContent.innerHTML);
    WinPrint.document.head.innerHTML = cssjs;
    WinPrint.document.close();
    WinPrint.focus();
    WinPrint.print();
    WinPrint.close();

    //Rollback the Cash Breakdown Liquidation table attribute value to default(0).
    for (var count = 0; count < tableCnt.length - 1; count++) {
        $('#cashbdliqid_' + (20 + count)).find('.txtLiqNoPCS').attr('value', 0);
        $('#cashbdliqid_' + (20 + count)).find('.txtLiqAmount').attr('value', 0);
    }
    $('#txtCashBDTotal').attr('value', 0)
};

function AC(nStr) {
    if (parseFloat(nStr) % 1 == 0 && $('.highlight').find('.currMasterID').val() != getXMLLiqValue("CURRENCY_Yen")) {
        nStr = nStr + ".00";
    }

    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }

    return x1 + x2;
};

function RC(nStr) {
    return nStr.replace(/\,/g, "");
};

function appendInputIEtoDiv(ret) {
    for (var a = 0; a < 4; a++) {
        for (var b = 1; b < 4; b++) {
            for (var c = 1; c < 3; c++) {
                //ACCOUNT
                $('#divLiqIE_' + ret).append($('<input/>', {
                    id: 'LiquidationDetails_' + ret + '__liqInterEntity_' + a + '__Liq_AccountID_' + b + '_' + c,
                    type: 'hidden',
                    name: 'LiquidationDetails[' + ret + '].liqInterEntity[' + a + '].Liq_AccountID_' + b + '_' + c,
                    value: 0
                }));
                //AMOUNT
                $('#divLiqIE_' + ret).append($('<input/>', {
                    id: 'LiquidationDetails_' + ret + '__liqInterEntity_' + a + '__Liq_Amount_' + b + '_' + c,
                    type: 'hidden',
                    name: 'LiquidationDetails[' + ret + '].liqInterEntity[' + a + '].Liq_Amount_' + b + '_' + c,
                    value: 0
                }));
                //CURRENCY
                $('#divLiqIE_' + ret).append($('<input/>', {
                    id: 'LiquidationDetails_' + ret + '__liqInterEntity_' + a + '__Liq_CCY_' + b + '_' + c,
                    type: 'hidden',
                    name: 'LiquidationDetails[' + ret + '].liqInterEntity[' + a + '].Liq_CCY_' + b + '_' + c,
                    value: 0
                }));
                //DEBIT/CREDIT
                $('#divLiqIE_' + ret).append($('<input/>', {
                    id: 'LiquidationDetails_' + ret + '__liqInterEntity_' + a + '__Liq_DebitCred_' + b + '_' + c,
                    type: 'hidden',
                    name: 'LiquidationDetails[' + ret + '].liqInterEntity[' + a + '].Liq_DebitCred_' + b + '_' + c,
                    value: ""
                }));
                //INTER-RATE
                $('#divLiqIE_' + ret).append($('<input/>', {
                    id: 'LiquidationDetails_' + ret + '__liqInterEntity_' + a + '__Liq_InterRate_' + b + '_' + c,
                    type: 'hidden',
                    name: 'LiquidationDetails[' + ret + '].liqInterEntity[' + a + '].Liq_InterRate_' + b + '_' + c,
                    value: 0
                }));
            }
        }
        //TAX RATE
        $('#divLiqIE_' + ret).append($('<input/>', {
            id: 'LiquidationDetails_' + ret + '__liqInterEntity_' + a + '__Liq_Tax_Rate',
            type: 'hidden',
            name: 'LiquidationDetails[' + ret + '].liqInterEntity[' + a + '].Liq_Tax_Rate',
            value: 0
        }));
        //Vendor ID
        $('#divLiqIE_' + ret).append($('<input/>', {
            id: 'LiquidationDetails_' + ret + '__liqInterEntity_' + a + '__Liq_VendorID',
            type: 'hidden',
            name: 'LiquidationDetails[' + ret + '].liqInterEntity[' + a + '].Liq_VendorID',
            value: 0
        }));
    }
};

function setIEValuetoDivInput(ret) {
    //ACCONT
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_1_1').val($('#lblAccount1').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_1_2').val($('#lblAccount2').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_AccountID_1_1').val($('#ddlAccount3').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_AccountID_1_2').val($('#ddlAccount11').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_AccountID_2_1').val($('#ddlAccount11').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_AccountID_2_2').val($('#lblAccount3').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_AccountID_1_1').val($('#lblAccount4').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_AccountID_1_2').val($('#lblAccount5').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_AccountID_1_1').val($('#lblAccount6').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_AccountID_1_2').val($('#lblAccount7').val());

    //INTER-ENTITY
    $('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_InterRate_1_2').val($('#lblIEInput9').text());

    //CURRENCY
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_CCY_1_1').val($('#item_' + ret).find('.currID').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_CCY_1_2').val($('#item_' + ret).find('.currID').val());

    $.getJSON('getCurrency', { masterID: getXMLLiqValue("CURRENCY_PHP") }, function (data) {
        $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_CCY_1_1').val(data["curr_ID"]);
        $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_CCY_1_2').val(data["curr_ID"]);
        $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_CCY_2_1').val(data["curr_ID"]);
        $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_CCY_2_2').val(data["curr_ID"]);
        $('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_CCY_1_1').val(data["curr_ID"]);
    }); 


    $('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_CCY_1_2').val($('#item_' + ret).find('.currID').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_CCY_1_1').val($('#item_' + ret).find('.currID').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_CCY_1_2').val($('#item_' + ret).find('.currID').val());

    //DEBIT/CREDIT
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_DebitCred_1_1').val("D");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_DebitCred_1_2').val("C");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_DebitCred_1_1').val("D");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_DebitCred_1_2').val("D");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_DebitCred_2_1').val("C");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_DebitCred_2_2').val("C");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_DebitCred_1_1').val("D");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_DebitCred_1_2').val("C");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_DebitCred_1_1').val("D");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_DebitCred_1_2').val("C");

    //AMOUNT
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_1_1').val($('#txtIEInput1').val().replace(/\,/g, ''));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_1_2').val($('#txtIEInput1').val().replace(/\,/g, ''));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_Amount_1_1').val($('#txtIEInput2').val().replace(/\,/g, ''));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_Amount_1_2').val($('#txtIEInput3').val().replace(/\,/g, ''));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_Amount_2_1').val($('#txtIEInput4').val().replace(/\,/g, ''));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_Amount_2_2').val($('#lblIEInput2').val().replace(/\,/g, ''));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_Amount_1_1').val($('#lblIEInput3').text().replace(/\,/g, ''));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_Amount_1_2').val($('#lblIEInput4').text().replace(/\,/g, ''));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_Amount_1_1').val($('#lblIEInput5').text().replace(/\,/g, ''));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_Amount_1_2').val($('#lblIEInput6').text().replace(/\,/g, ''));
};

function assignDivValuesIE(pid) {
    var ret = pid.replace('item_', '');

    //ACCONT
    $('#lblAccount1').val($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_1_1').val());
    $('#lblAccount2').val($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_1_2').val());
    $('#ddlAccount3').val($('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_AccountID_1_1').val());
    $('#ddlAccount11').val($('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_AccountID_1_2').val());
    $('#ddlAccount11').val($('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_AccountID_2_1').val());
    $('#lblAccount3').val($('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_AccountID_2_2').val());
    $('#lblAccount4').val($('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_AccountID_1_1').val());
    $('#lblAccount5').val($('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_AccountID_1_2').val());
    $('#lblAccount6').val($('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_AccountID_1_1').val());
    $('#lblAccount7').val($('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_AccountID_1_2').val());

    //INTER-RATE
    $('#lblIEInput9').text($('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_InterRate_1_2').val());

    //AMOUNT
    $('#lblIEInput1').text(AC(round($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_1_1').val(), 2)));
    $('#txtIEInput1').val(AC(round($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_1_2').val(), 2)));
    $('#txtIEInput2').val(AC($('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_Amount_1_1').val()));
    $('#txtIEInput3').val(AC($('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_Amount_1_2').val()));
    $('#txtIEInput4').val(AC($('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_Amount_2_1').val()));
    $('#lblIEInput2').val(AC($('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_Amount_2_2').val()));
    $('#lblIEInput3').text(AC($('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_Amount_1_1').val()));
    $('#lblIEInput4').text(AC(round($('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_Amount_1_2').val(), 2)));
    $('#lblIEInput5').text(AC(round($('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_Amount_1_1').val(), 2)));
    $('#lblIEInput6').text(AC(round($('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_Amount_1_2').val(), 2)));

    $('#txtIEAmount').val(AC($('#lblIEInput4').text()));
    $('#txtIEExRate').val($('#lblIEInput9').text());
    $('#txtIEExRatePhp').val(AC(roundExceptionJPY(parseFloat(RC($('#txtIEAmount').val())) * parseFloat(RC($('#txtIEExRate').val())), 2)));

    $('#lblIEInput7').text($("#txtIEInput1").val());
    $('#lblIEInput8').text($("#txtIEInput1").val());
};

function setLiqPhpValuetoDivInput(ret) {
    //ACCONT
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_1_1').val($('#ddlLiqPhpAccount1').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_1_2').val($('#ddlLiqPhpAccount2').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_2_1').val($('#ddlLiqPhpAccount2').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_2_2').val($('#lblAccount1_PHP').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_3_1').val($('#lblAccount2_PHP').val());
    
    //DEBIT/CREDIT
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_DebitCred_1_1').val("D");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_DebitCred_1_2').val("D");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_DebitCred_2_1').val("C");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_DebitCred_2_2').val("C");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_DebitCred_3_1').val("C");

    //AMOUNT
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_1_1').val(RC($('#txtLiqPhpInput1').val()));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_1_2').val(RC($('#txtLiqPhpInput2').val()));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_2_1').val(RC($('#txtLiqPhpInput3').val()));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_2_2').val(RC($('#txtLiqPhpInput4').val()));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_3_1').val(RC($('#txtLiqPhpInput5').val()));

    //Tax RATE
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Tax_Rate').val($('#txtLiqTaxRate').val());
    //Vendor ID
    if ($('#txtLiqVendor').val() == null) {
        $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_VendorID').val(0);
    } else {
        $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_VendorID').val($('#txtLiqVendor').val());
    }
};

function assignDivValuesLiqPhp(pid) {
    var ret = pid.replace('item_', '');

    //Vendor ID
    $('#txtLiqVendor').val($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_VendorID').val());

    //ACCONT
    $('#ddlLiqPhpAccount1').val($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_1_1').val());
    $('#ddlLiqPhpAccount2').val($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_1_2').val());
    $('#ddlLiqPhpAccount2').val($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_2_1').val());
    $('#lblAccount1_PHP').val($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_2_2').val());
    $('#lblAccount2_PHP').val($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_3_1').val());

    //AMOUNT
    $('#txtLiqPhpInput1').val(AC($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_1_1').val()));
    $('#txtLiqPhpInput2').val(AC($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_1_2').val()));
    $('#txtLiqPhpInput3').val(AC($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_2_1').val()));
    $('#txtLiqPhpInput4').val(AC($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_2_2').val()));
    //$('#txtLiqPhpInput5').val($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_3_1').val());

    $('#txtLiqPhpInput6').val(AC(round(parseFloat(RC($('#txtLiqPhpInput1').val())) + parseFloat(RC($('#txtLiqPhpInput2').val())), 2)));

    $('#txtLiqPhpInput7').val(AC(round(parseFloat(RC($('#txtLiqPhpInput3').val())) + parseFloat(RC($('#txtLiqPhpInput4').val())) + parseFloat(RC($('#txtLiqPhpInput5').val())), 2)));

    //Tax RATE
    setDivTaxRateToLiqPhp(ret);
};

function assignAccCodeLiqPhp() {
    $.getJSON('getAllAccount', function (data) {
        $.each(data, function (index, item) {
            if (item["account_ID"] == $('#ddlLiqPhpAccount1').val()) {
                $('#txtLiqAccCode1').val(item["account_Code"]);
            }
            if (item["account_ID"] == $('#ddlLiqPhpAccount2').val()) {
                $('#txtLiqAccCode2').val(item["account_Code"]);
            }
            if (item["account_ID"] == $('#lblAccount1_PHP').val()) {
                $('#txtLiqAccCode3').val(item["account_Code"]);
            }
            if (item["account_ID"] == $('#lblAccount2_PHP').val()) {
                $('#txtLiqAccCode4').val(item["account_Code"]);
            }
        });
    }); 
};

function getVendorTaxRate(vendorID, ret) {
    $('#txtLiqTaxRate').empty();
    if ($('#hiddenMode').val() == "Edit") {
        $.getJSON('getVendorTaxRate', { vendorID: vendorID }, function (data) {
            $.each(data, function (index, item) {
                $('#txtLiqTaxRate').append($('<option/>', {
                    value: item["tR_ID"],
                    text: item["tR_WT_Title"]
                }));
            });
            if ($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Tax_Rate').val() > 0) {
                setDivTaxRateToLiqPhp(ret);
            } else {
                $('#txtLiqTaxRate').val($('#item_' + ret).find('.ewtID').val());
            };
        });
    } else {
        $.getJSON('getAllTaxRate', function (data) {
            $.each(data, function (index, item) {
                $('#txtLiqTaxRate').append($('<option/>', {
                    value: item["tR_ID"],
                    text: item["tR_WT_Title"]
                }));
            });
            if ($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Tax_Rate').val() > 0) {
                setDivTaxRateToLiqPhp(ret);
            } else {
                $('#txtLiqTaxRate').val($('#item_' + ret).find('.ewtID').val());
            };
        });
    }
};

function setDivTaxRateToLiqPhp(ret) {
    $('#txtLiqTaxRate').val($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Tax_Rate').val());
};

$('.number-inputExceptionJPY').keyup(function () {
    $(this).val($(this).val().replace(/[^0-9\.]/g, '').replace("-", ""));
    if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }

    if ($('.highlight').find('.currMasterID').val() == getXMLLiqValue("CURRENCY_Yen")) {
        $(this).val($(this).val().replace(/\D/g, '').replace("-", ""));
    };
});

$('.number-input').keyup(function () {
    $(this).val($(this).val().replace(/[^0-9\.]/g, '').replace("-", ""));
    if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }
});

$('.number-inputNoDecimal').keyup(function () {
    $(this).val($(this).val().replace(/[^\d].+/, "").replace("-", ""));
    if ((event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }
});

$('.number-inputNoDecNoBlank').keyup(function () {
    $(this).val($(this).val().replace(/[^\d].+/, "").replace("-", ""));
    if ((event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }
    $(this).val($(this).val().replace("e", "").replace("E", ""));
    if ((event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }
    if ($(this).val() == "") {
        $(this).val(0).replace("-", "");
    };

});

$('.number-only-change').change(function () {
    if ($(this).val() == "" || isNaN($(this).val())) {
        $(this).val(0);
    }
    $(this).val(round($(this).val(), 2));
});

$('.number-only-ExcRate-change').change(function () {
    if ($(this).val() == "" || isNaN($(this).val())) {
        $(this).val(0);
    }
    $(this).val(round($(this).val(), 4));
});

$('.IEBDOPettyInput').keyup(function () {
    IEBDOPettyInputChange();
});

function IEBDOPettyInputChange() {
    if ($('#txtIEInput3').val() > 0) {
        $('#txtIEInput4').val(0);
        $('#txtIEInput4').attr('disabled', 'disabled');
        $('#txtIEInput4').css('background-color', 'transparent');

        $('#txtIEInput3').removeAttr('disabled');
        $('#txtIEInput3').css('background-color', '#FFFBD5');
    }
    if ($('#txtIEInput4').val() > 0) {
        $('#txtIEInput3').val(0);
        $('#txtIEInput3').attr('disabled', 'disabled');
        $('#txtIEInput3').css('background-color', 'transparent');

        $('#txtIEInput4').removeAttr('disabled');
        $('#txtIEInput4').css('background-color', '#FFFBD5');
    }
    if ($('#txtIEInput3').val() == 0 && $('#txtIEInput4').val() == 0) {
        $('.IEBDOPettyInput').css('background-color', '#FFFBD5');
        $('.IEBDOPettyInput').removeAttr('disabled');
    }
}

$('.LiqPhpBDOPettyInput').keyup(function () {
    LiqPhpBDOPettyInputChange();
});

function LiqPhpBDOPettyInputChange() {
    if ($('#txtLiqPhpInput2').val() > 0) {
        $('#txtLiqPhpInput3').val(0);
        $('#txtLiqPhpInput3').attr('disabled', 'disabled');
        $('#txtLiqPhpInput3').css('background-color', 'transparent');
    
        $('#txtLiqPhpInput2').removeAttr('disabled');
        $('#txtLiqPhpInput2').css('background-color', '#FFFBD5');
    } 
    if ($('#txtLiqPhpInput3').val() > 0) {
        $('#txtLiqPhpInput2').val(0);
        $('#txtLiqPhpInput2').attr('disabled', 'disabled');
        $('#txtLiqPhpInput2').css('background-color', 'transparent');

        $('#txtLiqPhpInput3').removeAttr('disabled');
        $('#txtLiqPhpInput3').css('background-color', '#FFFBD5');
    } 
    if ($('#txtLiqPhpInput2').val() == 0 && $('#txtLiqPhpInput3').val() == 0) {
        $('.LiqPhpBDOPettyInput').css('background-color', '#FFFBD5');
        $('.LiqPhpBDOPettyInput').removeAttr('disabled');
    }
}

function HideCashBreakdownTable() {
    if ($('#ddlLiqPhpAccount2').val() != $('#lblAccount1_PHP optgroup[label=' + getXMLLiqValue('ACCOUNT11_2') + '] option:first').val()) {
        $('#cashBDLiqTable').css('display', 'none');
        $('.printCashBDBtn').css('display', 'none');
    } else {
        $('#cashBDLiqTable').show();
        $('.printCashBDBtn').show();
    }
}

function ChangeOptionMarkForPrint() {
    if (parseFloat(RC($('#txtLiqPhpInput2').val())) > 0) {
        $('#printOptExpenseLiq').html("&#9645;");
        $('#printOptReverseLiq').html("&#8999;");
    }
    if (parseFloat(RC($('#txtLiqPhpInput3').val())) > 0){
        $('#printOptExpenseLiq').html("&#8999;");
        $('#printOptReverseLiq').html("&#9645;");
    }
}

function ShowHideCDDLiqButton(pid) {
    var ret = pid.replace('item_', '');
    if (parseFloat(RC($('#lblIEInput7').text())) == 0) {
        $('.cddISLiqBtn').css('display', 'none');
    } else {
        $('.cddISLiqBtn').show();
    }
}