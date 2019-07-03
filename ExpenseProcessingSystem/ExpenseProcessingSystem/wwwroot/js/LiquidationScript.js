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
    return +(value[0] + 'e' + (value[1] ? (+value[1] - exp) : -exp));
};

function reqBtnDisable(idx) {
    if ($('#LiquidationDetails_' + idx + '__ccyAbbrev').val() != "PHP") {
        $('#req_' + idx).find('.reqBtn').css("pointer-events", "none");
    } else {
        $('#req_' + idx).find('.reqBtn').css("pointer-events", "auto");
    }
};

function displayInfoCashBD(pid) {

    $('#txtLiqAccNo').val($('#' + pid).find('.hidAccNo').val());
    $('#txtLiqAccCode').val($('#' + pid).find('.hidAccCode').val());

    $('#ddlLiqPhpAccount2').empty();
    $('#ddlLiqPhpAccount2').append($('<option></option>').val(getXMLLiqValue("ACCOUNT11_1")).html(getXMLLiqValue("ACCOUNT11_1")));
    $('#ddlLiqPhpAccount2').append($('<option></option>').val(getXMLLiqValue("ACCOUNT11_2")).html(getXMLLiqValue("ACCOUNT11_2")));
    $('#lblAccount1_PHP').text(getXMLLiqValue("ACCOUNT1_PHP"));
    $('#lblAccount2_PHP').text(getXMLLiqValue("ACCOUNT2_PHP"));

    $('.tableLiqPhp').find('.txtLiqPhpInput').val(0);

    if ($('#' + pid).find('.hiddenLiqFlag').val() == 0) {
        return false;
    }

    var ret = pid.replace('item_', '');
    var cashBDInput = $('#divLiqCashBD_' + ret).find("input");
    var totalAmount = 0.0;

    for (var count = 0; count < cashBDInput.length / 3; count++) {
        $('#cashbdliqid_' + count).find('.txtLiqDenomination').val($('#LiquidationDetails_' + ret + '__liqCashBreakdown_' + count + '__cashDenomination').val());
        $('#cashbdliqid_' + count).find('.txtLiqNoPCS').val($('#LiquidationDetails_' + ret + '__liqCashBreakdown_' + count + '__cashNoPC').val());
        $('#cashbdliqid_' + count).find('.txtLiqAmount').val($('#LiquidationDetails_' + ret + '__liqCashBreakdown_' + count + '__cashAmount').val());

        totalAmount += parseFloat($('#LiquidationDetails_' + ret + '__liqCashBreakdown_' + count + '__cashAmount').val());
    }
    $('#txtCashBDTotal').val(totalAmount);

    assignDivValuesLiqPhp(pid)
};

function displayInfoIE(pid) {
    $('.lblIEInput').text("");
    $('.txtIEExRatePhp').text("");
    $('.txtIEInfoInput').val(0);
    $('.txtIEInput').val(0);

    $('#txtIECurrency').val($('#' + pid).find('.txtCcyAbbrev').val());
    var curr = "";
    if ($('#' + pid).find('.txtCcyAbbrev').val() == getXMLLiqValue("CURRENCY_Yen")) {
        curr = "_Yen";
    } else {
        curr = "_US";
    }
    for (var a = 1; a <= 9; a++) {
        $('#lblAccount' + a).text(getXMLLiqValue("ACCOUNT" + a + curr));
    }
    $('.lblParticular').text("S" + $('#' + pid).find('.txtGBaseRemark').val());
    $('.lblCurrency').text(getXMLLiqValue("CURRENCY" + curr));
    $('#ddlAccount11').empty();
    $('#ddlAccount11').append($('<option></option>').val(getXMLLiqValue("ACCOUNT11_1")).html(getXMLLiqValue("ACCOUNT11_1")));
    $('#ddlAccount11').append($('<option></option>').val(getXMLLiqValue("ACCOUNT11_2")).html(getXMLLiqValue("ACCOUNT11_2")));

    if ($('#' + pid).find('.hiddenLiqFlag').val() == "2") {
        assignDivValuesIE(pid);
    }
};

function printDivCashBD(divID) {

    //Set Cash Breakdown Liquidation table value(property) to table attribute value.
    var tableCnt = $('#cashBDLiqTable').find('tbody').find('tr');

    for (var count = 0; count < tableCnt.length; count++) {
        $('#cashbdliqid_' + count).find('.txtLiqNoPCS').attr('value', $('#cashbdliqid_' + count).find('.txtLiqNoPCS').val());
        $('#cashbdliqid_' + count).find('.txtLiqAmount').attr('value', $('#cashbdliqid_' + count).find('.txtLiqAmount').val());
    }

    var css = '<style>table, td, th {  ' +
        '  border: 1px solid #ddd;' +
        '  text-align: center;' +
        '}' +

        'table {' +
        '  border-collapse: collapse;' +
        '}' +

        'th, td {' +
        '  padding: 15px;' +
        '}' +

        'input {' +
        '	outline:none;' +
        '    border:none;' +
        '}</style>';
    
    var printContent = document.getElementById(divID);

    var WinPrint = window.open('', '', 'width=900,height=650');
    WinPrint.document.write(printContent.innerHTML);
    WinPrint.document.head.innerHTML = css;
    WinPrint.document.close();
    WinPrint.focus();
    WinPrint.print();
    WinPrint.close();

    //Rollback the Cash Breakdown Liquidation table attribute value to default(0).
    for (var count = 0; count < tableCnt.length; count++) {
        $('#cashbdliqid_' + count).find('.txtLiqNoPCS').attr('value', 0);
        $('#cashbdliqid_' + count).find('.txtLiqAmount').attr('value', 0);
    }
};

function AC(nStr) {
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

function appendInputIEtoDiv(ret) {
    for (var a = 0; a < 4; a++) {
        for (var b = 1; b < 4; b++) {
            for (var c = 1; c < 3; c++) {
                //ACCOUNT
                $('#divLiqIE_' + ret).append($('<input/>', {
                    id: 'LiquidationDetails_' + ret + '__liqInterEntity_' + a + '__Liq_AccountID_' + b + '_' + c,
                    type: 'hidden',
                    name: 'LiquidationDetails[' + ret + '].liqInterEntity[' + a + '].Liq_AccountID_' + b + '_' + c,
                    value: ""
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
                    value: ""
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
    }
};

function setIEValuetoDivInput(ret) {
    //ACCONT
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_1_1').val($('#lblAccount1').text());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_1_2').val($('#lblAccount2').text());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_AccountID_1_1').val($('#ddlAccount3').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_AccountID_1_2').val($('#ddlAccount11').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_AccountID_2_1').val($('#ddlAccount11').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_AccountID_2_2').val($('#lblAccount3').text());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_AccountID_1_1').val($('#lblAccount4').text());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_AccountID_1_2').val($('#lblAccount5').text());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_AccountID_1_1').val($('#lblAccount6').text());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_AccountID_1_2').val($('#lblAccount7').text());

    //INTER-ENTITY
    $('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_InterRate_1_2').val($('#lblIEInput9').text());

    //CURRENCY
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_CCY_1_1').val($('#txtIECurrency').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_CCY_1_2').val($('#txtIECurrency').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_CCY_1_1').val("PHP");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_CCY_1_2').val("PHP");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_CCY_2_1').val("PHP");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_CCY_2_2').val("PHP");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_CCY_1_1').val("PHP");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_CCY_1_2').val($('#txtIECurrency').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_CCY_1_1').val($('#txtIECurrency').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_CCY_1_2').val($('#txtIECurrency').val());

    //DEBIT/CREDIT
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_DebitCred_1_1').val("D");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_DebitCred_1_2').val("C");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_DebitCred_1_1').val("D");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_DebitCred_1_2').val("C");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_DebitCred_2_1').val("D");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_DebitCred_2_2').val("C");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_DebitCred_1_1').val("D");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_DebitCred_1_2').val("C");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_DebitCred_1_1').val("D");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_DebitCred_1_2').val("C");

    //AMOUNT
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_1_1').val($('#txtIEInput1').val().replace(',', ''));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_1_2').val($('#txtIEInput1').val().replace(',', ''));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_Amount_1_1').val($('#txtIEInput2').val().replace(',', ''));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_Amount_1_2').val($('#txtIEInput4').val().replace(',', ''));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_Amount_2_1').val($('#txtIEInput3').val().replace(',', ''));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_Amount_2_2').val($('#lblIEInput2').text().replace(',', ''));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_Amount_1_1').val($('#lblIEInput3').text().replace(',', ''));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_Amount_1_2').val($('#lblIEInput4').text().replace(',', ''));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_Amount_1_1').val($('#lblIEInput5').text().replace(',', ''));
    $('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_Amount_1_2').val($('#lblIEInput6').text().replace(',', ''));
};

function assignDivValuesIE(pid) {
    var ret = pid.replace('item_', '');

    //ACCONT
    $('#lblAccount1').text($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_1_1').val());
    $('#lblAccount2').text($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_1_2').val());
    $('#ddlAccount3').val($('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_AccountID_1_1').val());
    $('#ddlAccount11').val($('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_AccountID_1_2').val());
    $('#ddlAccount11').val($('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_AccountID_2_1').val());
    $('#lblAccount3').text($('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_AccountID_2_2').val());
    $('#lblAccount4').text($('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_AccountID_1_1').val());
    $('#lblAccount5').text($('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_AccountID_1_2').val());
    $('#lblAccount6').text($('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_AccountID_1_1').val());
    $('#lblAccount7').text($('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_AccountID_1_2').val());

    //INTER-ENTITY
    $('#lblIEInput9').text($('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_InterRate_1_2').val());

    //AMOUNT
    $('#lblIEInput1').text(AC(round($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_1_1').val())));
    $('#txtIEInput1').val(AC(round($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_1_2').val())));
    $('#txtIEInput2').val(AC(round($('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_Amount_1_1').val())));
    $('#txtIEInput4').val(AC(round($('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_Amount_1_2').val())));
    $('#txtIEInput3').val(AC(round($('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_Amount_2_1').val())));
    $('#lblIEInput2').text(AC(round($('#LiquidationDetails_' + ret + '__liqInterEntity_1__Liq_Amount_2_2').val())));
    $('#lblIEInput3').text(AC(round($('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_Amount_1_1').val())));
    $('#lblIEInput4').text(AC(round($('#LiquidationDetails_' + ret + '__liqInterEntity_2__Liq_Amount_1_2').val())));
    $('#lblIEInput5').text(AC(round($('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_Amount_1_1').val())));
    $('#lblIEInput6').text(AC(round($('#LiquidationDetails_' + ret + '__liqInterEntity_3__Liq_Amount_1_2').val())));

    $('#txtIEAmount').val($('#lblIEInput4').text().replace(',', ''));
    $('#txtIEExRate').val($('#lblIEInput9').text());
    $('#txtIEExRatePhp').val(AC(round(parseFloat($('#txtIEAmount').val() * $('#txtIEExRate').val()))));

    $('#lblIEInput7').text(AC(round($("#txtIEAmount").val(), 2)));
    $('#lblIEInput8').text(AC(round($("#txtIEAmount").val(), 2)));
};

function setLiqPhpValuetoDivInput(ret) {
    //ACCONT
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_1_1').val($('#ddlLiqPhpAccount1').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_1_2').val($('#ddlLiqPhpAccount2').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_2_1').val($('#ddlLiqPhpAccount2').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_2_2').val($('#lblAccount1_PHP').text());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_3_1').val($('#lblAccount2_PHP').text());
    
    //DEBIT/CREDIT
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_DebitCred_1_1').val("D");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_DebitCred_1_2').val("D");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_DebitCred_2_1').val("C");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_DebitCred_2_2').val("C");
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_DebitCred_3_1').val("C");

    //AMOUNT
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_1_1').val($('#txtLiqPhpInput1').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_1_2').val($('#txtLiqPhpInput2').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_2_1').val($('#txtLiqPhpInput3').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_2_2').val($('#txtLiqPhpInput4').val());
    $('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_3_1').val($('#txtLiqPhpInput5').val());
};

function assignDivValuesLiqPhp(pid) {
    var ret = pid.replace('item_', '');

    //ACCONT
    $('#ddlLiqPhpAccount1').val($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_1_1').val());
    $('#ddlLiqPhpAccount2').val($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_1_2').val());
    $('#ddlLiqPhpAccount2').val($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_2_1').val());
    $('#lblAccount1_PHP').text($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_2_2').val());
    $('#lblAccount2_PHP').text($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_AccountID_3_1').val());

    //AMOUNT
    $('#txtLiqPhpInput1').val($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_1_1').val());
    $('#txtLiqPhpInput2').val($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_1_2').val());
    $('#txtLiqPhpInput3').val($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_2_1').val());
    $('#txtLiqPhpInput4').val($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_2_2').val());
    $('#txtLiqPhpInput5').val($('#LiquidationDetails_' + ret + '__liqInterEntity_0__Liq_Amount_3_1').val());

    $('#txtLiqPhpInput6').val(AC(parseFloat($('#txtLiqPhpInput1').val()) + parseFloat($('#txtLiqPhpInput2').val())));
    $('#txtLiqPhpInput7').val(AC(parseFloat($('#txtLiqPhpInput3').val()) + parseFloat($('#txtLiqPhpInput4').val()) + parseFloat($('#txtLiqPhpInput5').val())));

};

$('.number-inputExceptionJPY').keyup(function () {
    $(this).val($(this).val().replace(/[^0-9\.]/g,''));
    if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }

    if ($('.highlight').find('.txtCcyAbbrev').val() == getXMLLiqValue("CURRENCY_Yen")) {
        $(this).val($(this).val().replace(/\D/g, ''));
    };
});

$('.number-input').keyup(function () {
    $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
    if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }
});

$('.number-inputNoDecimal').keyup(function () {
    $(this).val($(this).val().replace(/[^\d].+/, ""));
    if ((event.which < 48 || event.which > 57)) {
        event.preventDefault();
    }
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