
$(document).ready(function () {
    populateCol();

    /*//////////////////////////////////////////////////////////////////
    [ Login ]*/
    //// LOGIN
    $(".login-input").on("change", function (e) {
        CheckPw();
        CheckUn();
    });

    /*//////////////////////////////////////////////////////////////////
   [ Pagination ]*/
    $(document).on("click", ".pagination-btn", function (e) {
        var page = $(this).attr('page');
        $("<input />").attr("type", "hidden")
            .attr("name", "page")
            .attr("value", page)
            .appendTo("#search-frm");
        $("#search-frm").submit();

    });
    /*//////////////////////////////////////////////////////////////////
   [ Tabs ]*/
    var tabLinks = new Array();
    var contentDivs = new Array();


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
    ///TABS
    init();

    ////-----------------------------------------------------------

    //$('.tbl-btn:has(button[disabled])').addClass('btnDisabled');

    var $pageInput = $('#paginationInput');
    $pageInput.data("value", $pageInput.val());
    ////DM ON LOAD

    
    /////END DM ON LOAD

    /*//////////////////////////////////////////////////////////////////
    [ Modal ]*/
    setInterval(function () {
        $('div.modal-backdrop.fade.in').not(':first').remove();
    }, 1);

    /*//////////////////////////////////////////////////////////////////
    [ UM ]*/

    $('#um').find('a').click(function () {
        ClearValidations();
    });

    /*//////////////////////////////////////////////////////////////////
    [ Functions ]*/

    function defaultApproved() {
        $('.add-rec').prop('disabled', false);
        $('.edit-rec').prop('disabled', false);
        $('.delete-rec').prop('disabled', false);
    }
    ///////////////////////////////////////////////////////////////////
    function populateCol() {
        $('#dm-col').find('option').remove();
        //get all tbale header values and populate the column header combo box
        $("#partial-container div div table thead tr th").each(function () {
            if ($(this).text().length > 0) {
                var val = $(this).closest('th').attr('id');
                $('#dm-col').append('<option value="'+val+'">' + $(this).text() + '</option>');
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
        var ENTRY = "/Home/Entry";
        var ENTRY_CV = "/Home/Entry_CV";
        var ENTRY_DDV = "/Home/Entry_DDV";
        var ENTRY_NC = "/Home/Entry_NC";
        var ENTRY_PCV = "/Home/Entry_PCV";
        var ENTRY_SS = "/Home/Entry_SS";
        var ENTRY_Liquidation = "/Home/Entry_Liquidation";
        var ENTRY_NEW_CV = "/Home/AddNewCV";
        var ENTRY_NEW_DDV = "/Home/AddNewDDV";
        var ENTRY_NEW_PCV = "/Home/AddNewPCV";
        var ENTRY_NEW_SS = "/Home/AddNewSS";
        var ENTRY_NEW_NC = "/Home/AddNewNC";
        var ENTRY_VIEW_CV = "/Home/View_CV";
        var ENTRY_VIEW_DDV = "/Home/View_DDV";
        var ENTRY_VIEW_PCV = "/Home/View_PCV";
        var ENTRY_VIEW_SS = "/Home/View_SS";
        var ENTRY_VIEW_NC = "/Home/View_NC";
        var ENTRY_MOD_CV = "/Home/VerAppModCV";
        var ENTRY_MOD_DDV = "/Home/VerAppModDDV";
        var ENTRY_MOD_PCV = "/Home/VerAppModPCV";
        var ENTRY_MOD_SS = "/Home/VerAppModSS";
        var ENTRY_MOD_NC = "/Home/VerAppModNC";
        var LIQ_MAIN = "/Home/Liquidation_Main";
        var AMOR = "/Home/Amortization";
        var LIQ_SS = "/Home/Liquidation_SS";
        var LIQ_New_SS = "/Home/Liquidation_AddNewSS";
        var LIQ_View_SS = "/Home/View_Liquidation_SS";
        var LIQ_MOD_SS = "/Home/Liquidation_VerAppModSS";

        var ENTRY_VALS_CV = [
            ENTRY,
            ENTRY_CV,
            ENTRY_NEW_CV,
            ENTRY_VIEW_CV,
            ENTRY_MOD_CV];
        var ENTRY_VALS_DDV = [
            ENTRY_DDV,
            ENTRY_NEW_DDV,
            ENTRY_VIEW_DDV,
            ENTRY_MOD_DDV];
        var ENTRY_VALS_NC = [
            ENTRY_NC,
            ENTRY_NEW_NC,
            ENTRY_VIEW_NC,
            ENTRY_MOD_NC];
        var ENTRY_VALS_PCV = [
            ENTRY_PCV,
            ENTRY_NEW_PCV,
            ENTRY_VIEW_PCV,
            ENTRY_MOD_PCV];

        var ENTRY_VALS_LIQ = [
            ENTRY_Liquidation,
            LIQ_MAIN,
            LIQ_SS,
            LIQ_New_SS,
            LIQ_View_SS,
            LIQ_MOD_SS];

        var ENTRY_VALS_SS = [
            ENTRY_SS,
            ENTRY_NEW_SS,
            ENTRY_VIEW_SS,
            ENTRY_MOD_SS];

        var ENTRY_VALS = [
            ENTRY,
            ENTRY_CV,
            ENTRY_DDV,
            ENTRY_NC,
            ENTRY_PCV,
            ENTRY_SS,
            ENTRY_Liquidation,
            ENTRY_NEW_CV,
            ENTRY_NEW_DDV,
            ENTRY_NEW_PCV,
            ENTRY_NEW_SS,
            ENTRY_NEW_NC,
            ENTRY_VIEW_CV,
            ENTRY_VIEW_DDV,
            ENTRY_VIEW_PCV,
            ENTRY_VIEW_SS,
            ENTRY_VIEW_NC,
            ENTRY_MOD_CV,
            ENTRY_MOD_DDV,
            ENTRY_MOD_PCV,
            ENTRY_MOD_SS,
            ENTRY_MOD_NC,
            LIQ_MAIN,
            AMOR,
            LIQ_SS,
            LIQ_New_SS,
            LIQ_View_SS,
            LIQ_MOD_SS
        ];
        var HOME_INDEX = "/Home/Index";
        var HOME_PENDING = "/Home/Pending";
        var HOME_HISTORY = "/Home/History";

        var HOME_VALS = [HOME_INDEX, HOME_PENDING, HOME_HISTORY];

        for (var id in tabLinks) {
            tabLinks[id].onclick = showTab;
            tabLinks[id].onfocus = function () { this.blur() };
            if (id == tabVal) tabLinks[id].className = 'selected';
            //if any entry tab is clicked, 'Entry' tab is selected as well
            if (ENTRY_VALS.indexOf(tabVal) > 0) {
                document.getElementById('entry').firstElementChild.className = 'selected';
                //if any cv page, cv tab is selected
                if (ENTRY_VALS_CV.indexOf(tabVal) > 0) {
                    document.getElementById('cv').firstElementChild.className = 'selected';
                }
                //if any ddv page, ddv tab is selected
                if (ENTRY_VALS_DDV.indexOf(tabVal) > 0) {
                    document.getElementById('ddv').firstElementChild.className = 'selected';
                }
                //if any nc page, nc tab is selected
                if (ENTRY_VALS_NC.indexOf(tabVal) > 0) {
                    document.getElementById('nc').firstElementChild.className = 'selected';
                }
                //if any pcv page, pcv tab is selected
                if (ENTRY_VALS_PCV.indexOf(tabVal) > 0) {
                    document.getElementById('pcv').firstElementChild.className = 'selected';
                }
                //if any ss page, ss tab is selected
                if (ENTRY_VALS_SS.indexOf(tabVal) > 0) {
                    document.getElementById('ss').firstElementChild.className = 'selected';
                }
                //if any liquidation page, liquidation tab is selected
                if (ENTRY_VALS_LIQ.indexOf(tabVal) > 0) {
                    document.getElementById('liquidation').firstElementChild.className = 'selected';
                }
            }
            //if any home tab is clicked, 'Home' tab is selected as well
            else if (HOME_VALS.indexOf(tabVal) > 0) {
                document.getElementById('home').firstElementChild.className = 'selected';
            }
            i++;
        }

        // Hide all content divs except the first
        var i = 0;
        for (var id in contentDivs) {
            if (i != 0 && contentDivs[id] != null) contentDivs[id].className = 'tabContent hide';
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
    //function CheckUn() {
    //    var val = $('#User_UserName').val();
    //    if (val.length > 0) {
    //        $('.UN').hide();
    //    } else {
    //        $('.UN').show();
    //    }
    //}
    //function CheckPw() {
    //    var val = $('#User_Password').val();
    //    if (val.length > 0) {
    //        //alert(val);
    //        $('.PW').hide();
    //    } else {
    //        $('.PW').show();
    //    }
    //}
    //clear validation onload of User Management
    function ClearValidations() {
        $('#validationSummary').empty();
    };

    
});

//Confirmation pop up
function OpenConfirmationPopup(actCmd, command, specialMsg) {
    //Assign action command
    $("#confirmMsg").text(actCmd);

    //Assign button command value to hidden input
    $('#btnID').val(command);

    //Show special message
    if (specialMsg.length > 1) {
        $.each(specialMsg, function (index, itemData) {
            $('#specialMsgUL').show();
            $('#specialMsgUL').append($('<li>' + itemData +'<li/>'));
        });
    } else if (specialMsg.length > 0) {
        $('#specialMsgUL').show();
        $("#specialMsg").text(specialMsg);
    } else {
        $('#specialMsgUL').hide();
        $("#specialMsg").text("");
    }

}
//Loading effect jquery
function loadingEffectStart() { $('body').addClass("loading"); }
function loadingEffectStop() { $('body').removeClass("loading"); } 

function roundNumber(num, scale) {
    return parseFloat(Math.round(num * (Math.pow(10, scale))) / Math.pow(10, scale)).toFixed(2);
};
