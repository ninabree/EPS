﻿
$(document).ready(function () {
    CheckUn();
    CheckPw();
    populateCol();
    var tabLinks = new Array();
    var contentDivs = new Array();

    var $pageInput = $('#paginationInput');
    $pageInput.data("value", $pageInput.val());


    /*//////////////////////////////////////////////////////////////////
    [ Login ]*/
    $('#Acc_UserName').change(function () {
        CheckUn();
    });
    $('#Acc_Password').change(function () {
        CheckPw();
    });

    /*//////////////////////////////////////////////////////////////////
    [ Modal ]*/
    setInterval(function () {
        $('div.modal-backdrop.fade.in').not(':first').remove();
    }, 1);
    /*//////////////////////////////////////////////////////////////////
    [ Modal ]*/
    $('#bm a').click(function () {
        //var h = $(window).height();
        //this..attr("href", "/Home/BM/"+h);
        //$("#clientScreenWidth").val($(window).width());
        //$("#clientScreenHeight").val($(window).height());
        //sessionStorage.setItem("clientScreenHeight", $(window).height());
    });

    /*//////////////////////////////////////////////////////////////////
    [ Pagination - Number Input]*/

    setInterval(function () {
        //get hidden value of partial view in Data Maintenance
        var partial = $('input#partialVal').val();

        var data = $pageInput.data("value"),
            val = $pageInput.val();

        if (data !== val) {
            $pageInput.data("value", val);
            var url = window.location.pathname + '?page=' + val;
            //check if in data maintenance
            if (window.location.pathname == "/Home/DM") {
                url = window.location.pathname + '?partialName='+partial+'&page=' + val;
            }
            window.location = url;
        }
    }, 100);


    /*//////////////////////////////////////////////////////////////////
    [ DM ]*/
    
    /*//////////////////////////////////////////////////////////////////
    [ Tabs ]*/

    //temp - if page came from login page, localstorage with default to /Home/Index
    if (document.referrer = "https://localhost:44395/") {
        localStorage['tabVal'] = window.location.pathname;
    }
    var tabVal = localStorage['tabVal'] != "" ? localStorage['tabVal'] : "/Home/Index";


    /*//////////////////////////////////////////////////////////////////
    [ JQuery UI ]*/
    //always last but not after init()
    $('.validation-summary-errors').dialog({
        dialogClass: 'msg_dlg',
        title: 'Error',
        closeText: "",
        position: {
            of: 'section',
            at: 'right top',
            my: 'right top'
        },
        open: function (event, ui) {
            $(this).dialog('moveToTop');
        },
        close: function () {
            $(this).dialog('destroy');
        }
    });

    init();


    /*//////////////////////////////////////////////////////////////////
    [ Functions ]*/
    function populateCol() {
        $('#dm-col').find('option').remove();
        //get all tbale header values and populate the column header combo box
        $("#partial-container div div table thead tr th").each(function () {
            if ($(this).text().length > 0) {
                $('#dm-col').append('<option value="">' + $(this).text() + '</option>');
            }
        });
    }
    //for tabs
    function init() {
        // Grab the tab links and content divs from the page
        var tabListItems = document.getElementById('tabs').childNodes;
        for (var i = 0; i < tabListItems.length; i++) {
            if (tabListItems[i].nodeName == "LI") {
                var tabLink = getFirstChildWithTagName(tabListItems[i], 'A');
                var id = getHash(tabLink.getAttribute('href'));
                tabLinks[id] = tabLink;
                contentDivs[id] = document.getElementById(id);
            }
        }
        //to include the tabs in entity
        var tabListItems2 = document.getElementsByClassName('ent-tabs');
        for (var i = 0; i < tabListItems2.length; i++) {
            if (tabListItems2[i].nodeName == "LI") {
                var tabLink2 = getFirstChildWithTagName(tabListItems2[i], 'A');
                var id = getHash(tabLink2.getAttribute('href'));
                tabLinks[id] = tabLink2;
                contentDivs[id] = document.getElementById(id);
            }
        }

        // Assign onclick events to the tab links, and
        // highlight the tab assigned in localStorage
        var i = 0;
        for (var id in tabLinks) {
            tabLinks[id].onclick = showTab;
            tabLinks[id].onfocus = function () { this.blur() };
            if (id == tabVal) tabLinks[id].className = 'selected';
            if ("/Home/Entry_CV" == tabVal || "/Home/Entry_DDV" == tabVal || "/Home/Entry_PCV" == tabVal || "/Home/Entry_NC" == tabVal ||
                "/Home/Entry_SS" == tabVal) {
                document.getElementById('entry').firstElementChild.className = 'selected';
            };
            i++;
        }

        // Hide all content divs except the first
        var i = 0;
        for (var id in contentDivs) {
            if (i != 0) contentDivs[id].className = 'tabContent hide';
            i++;
        }
    }

    //this event is triggered every onclick of any tab
    function showTab() {
        var selectedId = getHash(this.getAttribute('href'));
        localStorage['tabVal'] = selectedId;
    }
    function getFirstChildWithTagName(element, tagName) {
        for (var i = 0; i < element.childNodes.length; i++) {
            if (element.childNodes[i].nodeName == tagName) return element.childNodes[i];
        }
    }
    function getHash(url) {
        var hashPos = url.lastIndexOf('#');
        return url.substring(hashPos + 1);
    }
    //modals
    function showREModal() {
        alert();
    }

    //login
    function CheckUn() {
        var val = $('#Acc_UserName').val();
        if (val != "") {
            //alert(val);
            $('.UN').hide();
        } else {
            $('.UN').show();
        }
    }
    function CheckPw() {
        var val = $('#Acc_Password').val();
        if (val != "") {
            //alert(val);
            $('.PW').hide();
        } else {
            $('.PW').show();
        }
    }
});
$(function () {
    /*//////////////////////////////////////////////////////////////////
    [ MODAL ]*/
    //set content to modal body and show dynamic modal
    //Entry_DDV
    $("#reversal_entry").click(function () {
        //controller name, method name, modal header
        ModalPopup('Modal', 'ReversalEntryModal','Reversal Entry');
    });

    $(".adjust_btn").click(function () {
        var id = this.id;
        var splitid = id.split("_");
        //controller name, method name, modal header
        ModalPopup('Modal', 'BudgetAdjustmentModal', 'Budget Adjustment');
    });

    $("#add_payee").click(function () {
        //controller name, method name, modal header
        ModalPopup('Modal', 'DMAddPayee', 'Add New Record/s');
    });

    $("#edit_payee").click(function () {
        var chkCount = $('input.tbl-chk[type="checkbox"]:checked').length;
        if (!(chkCount <= 0)) {
            var idsArr = Array();
            $('input.tbl-chk[type="checkbox"]:checked').each(function (i, v) {
                idsArr.push($(v).attr('id'));
            });
            //controller name, method name, modal header
            ModalPopup('Modal', 'DMEditPayee', 'Edit Record/s', idsArr);
        } else {
            alert("No data record/s selected.");
        }
    });

    $("#add_dept").click(function () {
        //controller name, method name, modal header
        ModalPopup('Modal', 'DMAddDept', 'Add New Record/s');
    });

    $("#edit_dept").click(function () {
        var chkCount = $('input.tbl-chk[type="checkbox"]:checked').length;
        if (!(chkCount <= 0)) {
            var idsArr = Array();
            $('input.tbl-chk[type="checkbox"]:checked').each(function (i, v) {
                idsArr.push($(v).attr('id'));
            });
            //controller name, method name, modal header
            ModalPopup('Modal', 'DMEditDept', 'Edit Record/s', idsArr);
        } else {
            alert("No data record/s selected.");
        }
    });

    //clicks the hidden add row button inside the table
    $("#add_row_btn").click(function () {
        $(".table-add").click();
    });

    //set modal contents and show
    function ModalPopup(modal, method, modalHeader) {
        var modalDivBody = $('.modal-body');
        var modalDivHeader = $('.modal-header');
        //var modal1 = (a.indexOf('Add') > -1) ? $('#myModal') : $('#myModal2');
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
                modalDivBody.html(data);

                $('#myModal').modal('show');
            },
            fail: function (ex) {
                alert("fail");
            }
        });
    }
    function ModalPopup(modal, method, modalHeader, idsArr) {
        var modalDivBody = $('.modal-body');
        var modalDivHeader = $('.modal-header');
        var modalDivFooter = $('.modal-footer');

        var word = "Add";
        $.ajax({
            url: "RedirectCont2/",
            type: "POST",
            data: { Cont: modal, Method: method, IdsArr: idsArr }, //set controller and method name to show
            success: function (data) {
                //remove prev contents
                modalDivBody.empty();
                modalDivHeader.find('h4').remove();

                //set modal header title
                modalDivHeader.append('<h4 class="modal-title">' + modalHeader + '</h4>');
                if (RegExp('\\bAdd\\b', 'i').test(modalHeader) == true) {
                    modalDivFooter.find('#add_row_btn').remove();
                    modalDivFooter.append('<button type="button" id="add_row_btn" class="btn">Add Row</button>');
                }
                modalDivBody.html(data);

                $('#myModal').modal('show');
            },
            fail: function (ex) {
                alert("fail");
            }
        });
    }
}); 