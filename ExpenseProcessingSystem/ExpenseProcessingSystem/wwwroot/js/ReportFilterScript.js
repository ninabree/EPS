$(document).ready(function () {

    var reportType = $('#ddlReportType').val();
    // set fields
    var radioPeriod1 = $('#radioPeriodOption1_' + reportType);
    var radioPeriod2 = $('#radioPeriodOption2_' + reportType);
    var radioPeriod3 = $('#radioPeriodOption3_' + reportType);
    var radioPeriodOption = $('.radioPeriodOption_' + reportType);
    var ddlMonth = $('#ddlMonth_' + reportType);
    var ddlYear = $('#ddlYear_' + reportType);
    var ddlMonthTo = $('#ddlMonthTo_' + reportType);
    var ddlYearTo = $('#ddlYearTo_' + reportType);
    var ddlFileFormat = $('#ddlFileFormat');
    var ddlSignatory = $('#ddlSignatory_' + reportType);
    var dtPeriodFrom = $('#PeriodFrom_' + reportType);
    var dtPeriodTo = $('#PeriodTo_' + reportType);
    var txtCheckNoFrom = $('#CheckNoFrom_' + reportType);
    var txtCheckNoTo = $('#CheckNoTo_' + reportType);
    var txtVoucherNoFrom = $('#VoucherNoFrom_' + reportType);
    var txtVoucherNoTo = $('#VoucherNoTo_' + reportType);
    var txtTransNoFrom = $('#TransNoFrom_' + reportType);
    var txtTransNoTo = $('#TransNoTo_' + reportType);
    var txtSubjName = $('#SubjName_' + reportType);
    var btnGenerateFile = $("#btnGenerateFile");
    var btnGeneratePreview = $("#btnGeneratePreview");
    var lblValidation = $('#ValidationSummary');
    var dt = new Date();
    var today = dt.getFullYear() + "-" + ('0' + (dt.getMonth() + 1)).slice(-2) + "-" + ('0' + (dt.getDate())).slice(-2);
    var divChkTax = $('#divChkTax');

    function UpdateFileds() {
        reportType = $('#ddlReportType').val();
        // set fields
        radioPeriod1 = $('#radioPeriodOption1_' + reportType);
        radioPeriod2 = $('#radioPeriodOption2_' + reportType);
        radioPeriod3 = $('#radioPeriodOption3_' + reportType);
        radioPeriodOption = $('.radioPeriodOption_' + reportType);
        ddlMonth = $('#ddlMonth_' + reportType);
        ddlYear = $('#ddlYear_' + reportType);
        ddlMonthTo = $('#ddlMonthTo_' + reportType);
        ddlYearTo = $('#ddlYearTo_' + reportType);
        //ddlFileFormat = $('#ddlFileFormat_' + reportType);
        ddlSignatory = $('#ddlSignatory_' + reportType);
        dtPeriodFrom = $('#PeriodFrom_' + reportType);
        dtPeriodTo = $('#PeriodTo_' + reportType);
        txtCheckNoFrom = $('#CheckNoFrom_' + reportType);
        txtCheckNoTo = $('#CheckNoTo_' + reportType);
        txtVoucherNoFrom = $('#VoucherNoFrom_' + reportType);
        txtVoucherNoTo = $('#VoucherNoTo_' + reportType);
        txtTransNoFrom = $('#TransNoFrom_' + reportType);
        txtTransNoTo = $('#TransNoTo_' + reportType);
        txtSubjName = $('#SubjName_' + reportType);
        //divChkTax = $('#divChkTax_' + reportType);
    }

    $("#ddlReportType").change(function () {
        var ReportType = $(this).val();
        var select = $("#ddlSubType");
        if (ReportType == '') {
            select.empty();
            select.attr("disabled", "disabled");
            select.append($('<option/>', {
                value: 0,
                text: "----Select Report Sub-Type----"
            }));
        }

        //Default fields filter
        UpdateFileds();

        //radioPeriod1.attr("disabled", "disabled");
        //radioPeriod3.attr("disabled", "disabled");
        //ddlMonth.attr("disabled", "disabled");
        //ddlYear.attr("disabled", "disabled");
        //ddlMonthTo.attr("disabled", "disabled");
        //ddlYearTo.attr("disabled", "disabled");
        ddlFileFormat.attr("disabled", "disabled");
        //ddlSignatory.attr("disabled", "disabled");
        //dtPeriodFrom.attr("disabled", "disabled");
        //dtPeriodTo.attr("disabled", "disabled");
        //txtCheckNoFrom.attr("disabled", "disabled");
        //txtCheckNoTo.attr("disabled", "disabled");
        //txtVoucherNoFrom.attr("disabled", "disabled");
        //txtVoucherNoTo.attr("disabled", "disabled");
        //txtTransNoFrom.attr("disabled", "disabled");
        //txtTransNoTo.attr("disabled", "disabled");
        //txtSubjName.attr("disabled", "disabled");
        btnGenerateFile.attr("disabled", "disabled");
        btnGeneratePreview.attr("disabled", "disabled");
        //divChkTax.css('pointer-events', 'none');
        //divChkTax.css('background-color', '#ccc');
        //$('.chkTaxRate').prop("checked", false);
        //radioPeriod1.prop('checked', false);
        //radioPeriod2.prop('checked', false);
        //radioPeriod3.prop('checked', false);
        //txtCheckNoFrom.val("");
        //txtCheckNoTo.val("");
        //txtVoucherNoFrom.val("");
        //txtVoucherNoTo.val("");
        //txtTransNoFrom.val("");
        //txtTransNoTo.val("");
        //txtSubjName.val("");
        //ddlMonth.val(dt.getMonth() + 1);
        //ddlYear.val(dt.getFullYear());
        //ddlMonthTo.val(dt.getMonth() + 1);
        //ddlYearTo.val(dt.getFullYear());
        ddlFileFormat.val($("#ddlFileFormat option:first").val());
        ddlSignatory.val($("#ddlSignatory_" + reportType + " option:first").val());
        lblValidation.hide();
        //dtPeriodFrom.val(today);
        //dtPeriodTo.val(today);
        ddlFileFormat.children('option').show();
        ddlFileFormat.children('option[value=4]').css('display', 'none');

        $('#iframePreview').attr('src', '');

        if (ReportType == 2) {

        } else if (ReportType == 3) {


        } else if (ReportType == 5) {


        } else if (ReportType == 6) {
            ddlFileFormat.children('option').css('display', 'none');
            ddlFileFormat.children('option[value=4]').show();
            ddlFileFormat.val($("#ddlFileFormat option[value=4]").val());

        } else if (ReportType == 7) {
            ResetTransactionList();
            ddlFileFormat.children('option').css('display', 'none');
            ddlFileFormat.children('option[value=1]').show();

        } else if (ReportType == 8) {
            ddlFileFormat.children('option').css('display', 'none');
            ddlFileFormat.children('option[value=1]').show();
            ddlFileFormat.val($("#ddlFileFormat option[value=1]").val());
            $('#ddlSubType').trigger('change');
        } else if (ReportType == 9 || ReportType == 10) {
            ddlFileFormat.children('option').css('display', 'none');
            ddlFileFormat.children('option[value=1]').show();
            ddlFileFormat.val($("#ddlFileFormat option[value=1]").val());
        } else if (ReportType == 13) {
            ddlFileFormat.children('option').css('display', 'none');
            ddlFileFormat.children('option[value=2]').show();
            ddlFileFormat.val($("#ddlFileFormat option[value=2]").val());
        }
        if (ReportType != 0 || ReportType != '') {
            btnGenerateFile.removeAttr("disabled");
            btnGeneratePreview.removeAttr("disabled");
            ddlFileFormat.removeAttr("disabled");

            $.getJSON('GetReportSubType', { ReportTypeID: ReportType }, function (data) {
                select.empty();
                if (data.length == 0) {
                    select.attr("disabled", "disabled");
                    select.append($('<option/>', {
                        value: 0,
                        text: "----Select Report Sub-Type----"
                    }));
                } else {
                    select.removeAttr("disabled");
                }
                $.each(data, function (index, itemData) {
                    select.append($('<option/>', {
                        value: itemData.id,
                        text: itemData.subTypeName
                    }));
                });
            });
        }
    });

    $('#ddlSubType').change(function () {
        if ($("#ddlReportType").val() != 7) {
            return false;
        }
        ResetTransactionList();

        if ($(this).val() == 0) {
            return false;
        }
        if ($(this).val() == 1) {
            txtCheckNoFrom.removeAttr("disabled");
            txtCheckNoTo.removeAttr("disabled");
            txtVoucherNoFrom.removeAttr("disabled");
            txtVoucherNoTo.removeAttr("disabled");
        }

        if ($(this).val() == 2 || $(this).val() == 3 || $(this).val() == 4) {
            txtVoucherNoFrom.removeAttr("disabled");
            txtVoucherNoTo.removeAttr("disabled");
        }

        txtTransNoFrom.removeAttr("disabled");
        txtTransNoTo.removeAttr("disabled");
        txtSubjName.removeAttr("disabled");
    });

    function ResetTransactionList() {
        radioPeriod1.prop('checked', false);
        radioPeriod3.prop('checked', false);
        ddlMonth = $('#ddlMonth_7');
        ddlYear = $('#ddlYear_7');
        ddlMonthTo = $('#ddlMonthTo_7');
        ddlYearTo = $('#ddlYearTo_7');
        ddlSignatory = $('#ddlSignatory_7');
        dtPeriodFrom = $('#PeriodFrom_7');
        dtPeriodTo = $('#PeriodTo_7');
        txtCheckNoFrom = $('#CheckNoFrom_7');
        txtCheckNoTo = $('#CheckNoTo_7');
        txtVoucherNoFrom = $('#VoucherNoFrom_7');
        txtVoucherNoTo = $('#VoucherNoTo_7');
        txtTransNoFrom = $('#TransNoFrom_7');
        txtTransNoTo = $('#TransNoTo_7');
        txtSubjName = $('#SubjName_7');

        radioPeriod1.removeAttr("disabled");
        radioPeriod3.removeAttr("disabled");
        dtPeriodFrom.attr("disabled", "disabled");
        dtPeriodTo.attr("disabled", "disabled");
        ddlYear.attr("disabled", "disabled");
        ddlMonth.attr("disabled", "disabled");
        ddlMonthTo.attr("disabled", "disabled");
        ddlYearTo.attr("disabled", "disabled");
        txtCheckNoFrom.attr("disabled", "disabled");
        txtCheckNoTo.attr("disabled", "disabled");
        txtVoucherNoFrom.attr("disabled", "disabled");
        txtVoucherNoTo.attr("disabled", "disabled");
        txtTransNoFrom.attr("disabled", "disabled");
        txtTransNoTo.attr("disabled", "disabled");
        txtSubjName.attr("disabled", "disabled");
        txtCheckNoFrom.val("");
        txtCheckNoTo.val("");
        txtVoucherNoFrom.val("");
        txtVoucherNoTo.val("");
        txtTransNoFrom.val("");
        txtTransNoTo.val("");
        txtSubjName.val("");
        dtPeriodFrom.val(today);
        dtPeriodTo.val(today);
        ddlMonth.val(dt.getMonth() + 1);
        ddlYear.val(dt.getFullYear());
        ddlMonthTo.val(dt.getMonth() + 1);
        ddlYearTo.val(dt.getFullYear());
    };

    $('.radioPeriodOption_3').change(function () {
        radioAction();
    });
    $('.radioPeriodOption_4').change(function () {
        radioAction();
    });
    $('.radioPeriodOption_7').change(function () {
        radioAction();
    });
    $('.radioPeriodOption_8').change(function () {
        radioAction();
    });
    $('.radioPeriodOption_10').change(function () {
        radioAction();
    });
    function radioAction() {
        UpdateFileds();
        //Default fields filter
        ddlMonth.attr("disabled", "disabled");
        ddlYear.attr("disabled", "disabled");
        ddlMonthTo.attr("disabled", "disabled");
        ddlYearTo.attr("disabled", "disabled");
        dtPeriodFrom.attr("disabled", "disabled");
        dtPeriodTo.attr("disabled", "disabled");

        switch ($('.radioPeriodOption_' + $('#ddlReportType').val() + ':checked').val()) {
            case "1":
                ddlYear.removeAttr("disabled");
                ddlMonth.removeAttr("disabled");
                ddlYearTo.removeAttr("disabled");
                ddlMonthTo.removeAttr("disabled");
                break;
            case "3":
                dtPeriodFrom.removeAttr("disabled");
                dtPeriodTo.removeAttr("disabled");
                break;
        }
    }

    $('#chkAllTax').change(function () {
        if ($('#chkAllTax').prop("checked")) {
            $('.chkTaxRate').prop("checked", true);
        } else {
            $('.chkTaxRate').prop("checked", false);
        }
    });

    $('.number-inputNoDecimal').keyup(function () {
        $(this).val($(this).val().replace(/[^\d].+/, ""));
        if ((event.which < 48 || event.which > 57)) {
            event.preventDefault();
        }
    });

    $('.deselectRadio').click(function () {
        UpdateFileds();
        radioPeriodOption.prop('checked', false);
        dtPeriodFrom.val(today);
        dtPeriodTo.val(today);
        ddlMonth.val(dt.getMonth() + 1);
        ddlYear.val(dt.getFullYear());
        ddlMonthTo.val(dt.getMonth() + 1);
        ddlYearTo.val(dt.getFullYear());
        dtPeriodFrom.attr("disabled", "disabled");
        dtPeriodTo.attr("disabled", "disabled");
        ddlMonth.attr("disabled", "disabled");
        ddlYear.attr("disabled", "disabled");
        ddlMonthTo.attr("disabled", "disabled");
        ddlYearTo.attr("disabled", "disabled");

    });
});

$(document).ready(function () {
    $("#btnGenerateFile").click(function (e) {
        e.preventDefault();
        loadingEffectStart();
        var reportType = $('#ddlReportType').val();
        var chkList = [];
        $.each($("input[name='chkTaxRate_" + reportType + "']:checked"), function () {
            chkList.push($(this).val());
        });
        var voucherList = [];
        $.each($("input[name='chkVoucherNo']:checked"), function () {
            voucherList.push($(this).val());
        });
        $.ajax({
            type: 'POST',
            url: '/Home/HomeReportValidation',
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
            data: {
                ReportType: $('#ddlReportType').val(),
                ReportSubType: $('#ddlSubType').val(),
                FileFormat: $('#ddlFileFormat').val(),
                Year: $('#ddlYear_' + reportType).val(),
                Month: $('#ddlMonth_' + reportType).val(),
                YearTo: $('#ddlYearTo_' + reportType).val(),
                MonthTo: $('#ddlMonthTo_' + reportType).val(),
                PeriodOption: $('.radioPeriodOption_' + $('#ddlReportType').val() + ':checked').val(),
                PeriodFrom: $('#PeriodFrom_' + reportType).val(),
                PeriodTo: $('#PeriodTo_' + reportType).val()
            },
            success: function (result) {
                if (result.message == "Invalid") {
                    var $summaryUl = $(".validation-summary-valid").find("ul");
                    $summaryUl.empty();
                    $.each(result.items, function (index) {
                        $summaryUl.append($("<li>").text(result.items[index]));
                    });
                    $('#ValidationSummary').show();
                }
                else {
                    $('#ValidationSummary').hide();
                    window.location.href = "/Home/GenerateFilePreview?ReportType=" + $('#ddlReportType').val()
                        + "&ReportSubType=" + $('#ddlSubType').val()
                        + "&FileFormat=" + $('#ddlFileFormat').val()
                        + "&Year=" + $('#ddlYear_' + reportType).val()
                        + "&Month=" + $('#ddlMonth_' + reportType).val()
                        + "&YearTo=" + $('#ddlYearTo_' + reportType).val()
                        + "&MonthTo=" + $('#ddlMonthTo_' + reportType).val()
                        + "&PeriodOption=" + $('.radioPeriodOption_' + $('#ddlReportType').val() + ':checked').val()
                        + "&PeriodFrom=" + $('#PeriodFrom_' + reportType).val()
                        + "&PeriodTo=" + $('#PeriodTo_' + reportType).val()
                        + "&TaxRateArray=" + chkList
                        + "&VoucherArray=" + voucherList
                        + "&VoucherNo=" + $('#ddlVoucherNo_' + reportType).val()
                        + "&VoucherNoList=" + $('#VoucherNoList_' + reportType).val()
                        + "&SignatoryID=" + $('#ddlSignatory_' + reportType).val()
                        + "&SignatoryIDVerifier=" + $('#ddlSignatoryVer_' + reportType).val()
                        + "&CheckNoFrom=" + $('#CheckNoFrom_' + reportType).val()
                        + "&CheckNoTo=" + $('#CheckNoTo_' + reportType).val()
                        + "&VoucherNoFrom=" + $('#VoucherNoFrom_' + reportType).val()
                        + "&VoucherNoTo=" + $('#VoucherNoTo_' + reportType).val()
                        + "&TransNoFrom=" + $('#TransNoFrom_' + reportType).val()
                        + "&TransNoTo=" + $('#TransNoTo_' + reportType).val()
                        + "&SubjName=" + $('#SubjName_' + reportType).val();
                }
                loadingEffectStop();
            }, 
            error: function (result) {
                alert('Error');
                loadingEffectStop();
            }
        });
    });
    $("#btnGeneratePreview").click(function (e) {
        e.preventDefault();
        loadingEffectStart();

        var reportType = $('#ddlReportType').val();
        var chkList = [];
        $.each($("input[name='chkTaxRate_" + reportType + "']:checked"), function () {
            chkList.push($(this).val());
        });

        var voucherList = [];
        $.each($("input[id='chkVoucherNo_" + reportType + "']:checked"), function () {
            voucherList.push($(this).val());
        });
        $.ajax({
            type: 'POST',
            url: '/Home/HomeReportValidation',
            dataType: 'json',
            contentType: 'application/x-www-form-urlencoded; charset=utf-8',
            data: {
                ReportType: $('#ddlReportType').val(),
                ReportSubType: $('#ddlSubType').val(),
                FileFormat: '3',
                Year: $('#ddlYear_' + reportType).val(),
                Month: $('#ddlMonth_' + reportType).val(),
                YearTo: $('#ddlYearTo_' + reportType).val(),
                MonthTo: $('#ddlMonthTo_' + reportType).val(),
                PeriodOption: $('.radioPeriodOption_' + $('#ddlReportType').val() + ':checked').val(),
                PeriodFrom: $('#PeriodFrom_' + reportType).val(),
                PeriodTo: $('#PeriodTo_' + reportType).val()
            },
            success: function (result) {
                if (result.message == "Invalid") {
                    var $summaryUl = $(".validation-summary-valid").find("ul");
                    $summaryUl.empty();
                    $.each(result.items, function (index) {
                        $summaryUl.append($("<li>").text(result.items[index]));
                    });
                    $('#ValidationSummary').show();
                    loadingEffectStop();
                }
                else {
                    $('#ValidationSummary').hide();
                    $('#iframePreview').prop('src', "/Home/GenerateFilePreview?ReportType=" + $('#ddlReportType').val()
                        + "&ReportSubType=" + $('#ddlSubType').val()
                        + "&FileFormat=3"
                        + "&Year=" + $('#ddlYear_' + reportType).val()
                        + "&Month=" + $('#ddlMonth_' + reportType).val()
                        + "&YearTo=" + $('#ddlYearTo_' + reportType).val()
                        + "&MonthTo=" + $('#ddlMonthTo_' + reportType).val()
                        + "&PeriodOption=" + $('.radioPeriodOption_' + $('#ddlReportType').val() + ':checked').val()
                        + "&PeriodFrom=" + $('#PeriodFrom_' + reportType).val()
                        + "&PeriodTo=" + $('#PeriodTo_' + reportType).val()
                        + "&TaxRateArray=" + chkList
                        + "&VoucherArray=" + voucherList
                        + "&VoucherNo=" + $('#ddlVoucherNo_' + reportType).val()
                        + "&SignatoryID=" + $('#ddlSignatory_' + reportType).val()
                        + "&SignatoryIDVerifier=" + $('#ddlSignatoryVer_' + reportType).val()
                        + "&CheckNoFrom=" + $('#CheckNoFrom_' + reportType).val()
                        + "&CheckNoTo=" + $('#CheckNoTo_' + reportType).val()
                        + "&VoucherNoFrom=" + $('#VoucherNoFrom_' + reportType).val()
                        + "&VoucherNoTo=" + $('#VoucherNoTo_' + reportType).val()
                        + "&TransNoFrom=" + $('#TransNoFrom_' + reportType).val()
                        + "&TransNoTo=" + $('#TransNoTo_' + reportType).val()
                        + "&SubjName=" + $('#SubjName_' + reportType).val());

                    var dt = new Date();
                    var date_time = ('0' + (dt.getMonth() + 1)).slice(-2) + "/" + dt.getDate() + "/" + dt.getFullYear() + " " + dt.getHours() + ":" + dt.getMinutes() + ":" + dt.getSeconds();

                    $('#txtAsOfLabel').text("As of ");
                    $('#txtDatePreviewShow').text(date_time);
                    loadingEffectStop();
                }
            },
            error: function (result) {
                alert('Error');
                loadingEffectStop();
            }
        });
    });
});
