$(document).ready(function () {
    // set fields
    var radioPeriod1 = $('#radioPeriodOption1');
    var radioPeriod2 = $('#radioPeriodOption2');
    var radioPeriod3 = $('#radioPeriodOption3');
    var ddlMonth = $('#ddlMonth');
    var ddlYear = $('#ddlYear');
    var ddlMonthTo = $('#ddlMonthTo');
    var ddlYearTo = $('#ddlYearTo');
    var ddlFileFormat = $('#ddlFileFormat');
    var ddlSignatory = $('#ddlSignatory');
    var dtPeriodFrom = $('#PeriodFrom');
    var dtPeriodTo = $('#PeriodTo');
    var txtCheck1 = $('#txtCheck1');
    var txtCheck2 = $('#txtCheck2');
    var txtVoucher1 = $('#txtVoucher1');
    var txtVoucher2 = $('#txtVoucher2');
    var txtCoveredTransNo1 = $('#txtCoveredTranNo1');
    var txtCoveredTransNo2 = $('#txtCoveredTranNo2');
    var txtCoveredTransNo3 = $('#txtCoveredTranNo3');
    var txtCoveredTransNo4 = $('#txtCoveredTranNo4');
    var txtCoveredTransNo5 = $('#txtCoveredTranNo5');
    var txtCoveredTransNo6 = $('#txtCoveredTranNo6');
    var txttxtSubjName = $('#txtSubjName');
    var btnGenerateFile = $("#btnGenerateFile");
    var btnGeneratePreview = $("#btnGeneratePreview");
    var lblValidation = $('#ValidationSummary');
    var dt = new Date();
    var today = dt.getFullYear() + "-" + ('0' + (dt.getMonth() + 1)).slice(-2) + "-" + ('0' + (dt.getDate())).slice(-2);
    var divChkTax = $('#divChkTax');

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

        radioPeriod1.attr("disabled", "disabled");
        radioPeriod2.attr("disabled", "disabled");
        radioPeriod3.attr("disabled", "disabled");
        ddlMonth.attr("disabled", "disabled");
        ddlYear.attr("disabled", "disabled");
        ddlMonthTo.attr("disabled", "disabled");
        ddlYearTo.attr("disabled", "disabled");
        ddlFileFormat.attr("disabled", "disabled");
        ddlSignatory.attr("disabled", "disabled");
        dtPeriodFrom.attr("disabled", "disabled");
        dtPeriodTo.attr("disabled", "disabled");
        txtCheck1.attr("disabled", "disabled");
        txtCheck2.attr("disabled", "disabled");
        txtVoucher1.attr("disabled", "disabled");
        txtVoucher2.attr("disabled", "disabled");
        txtCoveredTransNo1.attr("disabled", "disabled");
        txtCoveredTransNo2.attr("disabled", "disabled");
        txtCoveredTransNo3.attr("disabled", "disabled");
        txtCoveredTransNo4.attr("disabled", "disabled");
        txtCoveredTransNo5.attr("disabled", "disabled");
        txtCoveredTransNo6.attr("disabled", "disabled");
        txttxtSubjName.attr("disabled", "disabled");
        btnGenerateFile.attr("disabled", "disabled");
        btnGeneratePreview.attr("disabled", "disabled");
        divChkTax.css('pointer-events', 'none');
        divChkTax.css('background-color', '#ccc');
        $('.chkTaxRate').prop("checked", false);
        radioPeriod1.prop('checked', false);
        radioPeriod2.prop('checked', false);
        radioPeriod3.prop('checked', false);
        txtCheck1.text("");
        txtCheck2.text("");
        txtVoucher1.text("");
        txtVoucher2.text("");
        txtCoveredTransNo1.text("");
        txtCoveredTransNo2.text("");
        txtCoveredTransNo3.text("");
        txtCoveredTransNo4.text("");
        txtCoveredTransNo5.text("");
        txtCoveredTransNo6.text("");
        txttxtSubjName.text("");
        ddlMonth.val(dt.getMonth() + 1);
        ddlYear.val(dt.getFullYear());
        ddlMonthTo.val(dt.getMonth() + 1);
        ddlYearTo.val(dt.getFullYear());
        ddlFileFormat.val($("#ddlFileFormat option:first").val());
        ddlSignatory.val($("#ddlSignatory option:first").val());
        lblValidation.hide();
        dtPeriodFrom.val(today);
        dtPeriodTo.val(today);

        if (ReportType == "2") {
            radioPeriod1.prop('checked', true);
            radioPeriod1.removeAttr("disabled");
            ddlYear.removeAttr("disabled");
            ddlMonth.removeAttr("disabled");
            ddlSignatory.removeAttr("disabled");

        } else if (ReportType == "3") {
            radioPeriod1.prop('checked', true);
            ddlMonth.removeAttr("disabled");
            ddlYear.removeAttr("disabled");
            ddlMonthTo.removeAttr("disabled");
            ddlYearTo.removeAttr("disabled");
            radioPeriod1.removeAttr("disabled");
            divChkTax.css('pointer-events', 'auto');
            divChkTax.css('background-color', '');
            ddlSignatory.removeAttr("disabled");

        } else if (ReportType == "5") {
            radioPeriod1.prop('checked', true);
            radioPeriod1.removeAttr("disabled");
            ddlYear.removeAttr("disabled");
            ddlMonth.removeAttr("disabled");

        } else if (ReportType == "10") {
            radioPeriod1.prop('checked', true);
            radioPeriod1.removeAttr("disabled");
            radioPeriod3.removeAttr("disabled");
            ddlYear.removeAttr("disabled");
            ddlMonth.removeAttr("disabled");
            ddlMonthTo.removeAttr("disabled");
            ddlYearTo.removeAttr("disabled");

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

    $(".radioPeriodOption").change(function () {

        //Default fields filter
        ddlMonth.attr("disabled", "disabled");
        ddlYear.attr("disabled", "disabled");
        ddlMonthTo.attr("disabled", "disabled");
        ddlYearTo.attr("disabled", "disabled");
        dtPeriodFrom.attr("disabled", "disabled");
        dtPeriodTo.attr("disabled", "disabled");

        switch ($(this).val()) {
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
    });

    $('#chkAllTax').change(function () {
        if ($('#chkAllTax').prop("checked")) {
            $('.chkTaxRate').prop("checked", true);
        } else {
            $('.chkTaxRate').prop("checked", false);
        }
    });
});
$(document).ready(function () {
    $("#btnGenerateFile").click(function (e) {
        e.preventDefault();

        var chkList = [];
        $.each($("input[name='chkTaxRate']:checked"), function () {
            chkList.push($(this).val());
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
                Year: $('#ddlYear').val(),
                Month: $('#ddlMonth').val(),
                YearTo: $('#ddlYearTo').val(),
                MonthTo: $('#ddlMonthTo').val(),
                PeriodOption: $('.radioPeriodOption:checked').val(),
                PeriodFrom: $('#PeriodFrom').val(),
                PeriodTo: $('#PeriodTo').val()
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
                        + "&Year=" + $('#ddlYear').val()
                        + "&Month=" + $('#ddlMonth').val()
                        + "&YearTo=" + $('#ddlYearTo').val()
                        + "&MonthTo=" + $('#ddlMonthTo').val()
                        + "&PeriodOption=" + $('.radioPeriodOption:checked').val()
                        + "&PeriodFrom=" + $('#PeriodFrom').val()
                        + "&PeriodTo=" + $('#PeriodTo').val()
                        + "&TaxRateArray=" + chkList
                        + "&SignatoryID=" + $('#ddlSignatory').val();
                }
            },
            error: function (result) {
                alert('Error');
            }
        });
    });
    $("#btnGeneratePreview").click(function (e) {
        e.preventDefault();

        var chkList = [];
        $.each($("input[name='chkTaxRate']:checked"), function () {
            chkList.push($(this).val());
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
                Year: $('#ddlYear').val(),
                Month: $('#ddlMonth').val(),
                YearTo: $('#ddlYearTo').val(),
                MonthTo: $('#ddlMonthTo').val(),
                PeriodOption: $('.radioPeriodOption:checked').val(),
                PeriodFrom: $('#PeriodFrom').val(),
                PeriodTo: $('#PeriodTo').val()
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
                    $('#iframePreview').prop('src', "/Home/GenerateFilePreview?ReportType=" + $('#ddlReportType').val()
                        + "&ReportSubType=" + $('#ddlSubType').val()
                        + "&FileFormat=3"
                        + "&Year=" + $('#ddlYear').val()
                        + "&Month=" + $('#ddlMonth').val()
                        + "&YearTo=" + $('#ddlYearTo').val()
                        + "&MonthTo=" + $('#ddlMonthTo').val()
                        + "&PeriodOption=" + $('.radioPeriodOption:checked').val()
                        + "&PeriodFrom=" + $('#PeriodFrom').val()
                        + "&PeriodTo=" + $('#PeriodTo').val()
                        + "&TaxRateArray=" + chkList
                        + "&SignatoryID=" + $('#ddlSignatory').val());

                    var dt = new Date();
                    var date_time = ('0' + (dt.getMonth() + 1)).slice(-2) + "/" + dt.getDate() + "/" + dt.getFullYear() + " " + dt.getHours() + ":" + dt.getMinutes() + ":" + dt.getSeconds();

                    $('#txtAsOfLabel').text("As of ");
                    $('#txtDatePreviewShow').text(date_time);
                }
            },
            error: function (result) {
                alert('Error');
            }
        });
    });
});