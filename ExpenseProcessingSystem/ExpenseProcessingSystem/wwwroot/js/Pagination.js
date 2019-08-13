$(document).ready(function () {
    var currPageNo = $('#paginationInput').attr('placeholder');
    var lastPageNo = $('.txtMaxPage').val().substring($('.txtMaxPage').val().lastIndexOf(" ") + 1);
    var urlparam = window.location.search.substring(1);
    var link = $('.btnForInputPagination').attr('href');
    if (urlparam.substring(urlparam.lastIndexOf("page=")).replace(/\d+/g, '') == "page=") {
        if (parseInt(urlparam.substring(urlparam.lastIndexOf("page=")).replace("page=", "")) > lastPageNo) {
            window.location = link.substring(0, link.lastIndexOf("=") + 1) + "" + lastPageNo;
        }
    }

    $('.pagination-input').css("width", ($('.txtMaxPage').val().length * 6) + "px");

    $("#paginationInput").change(function (e) {
        loadingEffectStart();
        //var currPageNo = $('#paginationInput').attr('placeholder');
        //var lastPageNo = $('.txtMaxPage').val().substring($('.txtMaxPage').val().lastIndexOf(" ") + 1);
        var $pageInput = $(this);
        var data = $pageInput.data("value"),
            val = $pageInput.val();
        if (val == "") {
            return false;
        }
        var href = $('.btnForInputPagination').attr('href');

        if (parseInt(val) > parseInt(lastPageNo)) {
            $(this).val(lastPageNo);
        }
        window.location = href.substring(0, href.lastIndexOf("=") + 1) + "" + $(this).val();
    });

    $('.pagination-btn').click(function () { loadingEffectStart(); });
});