
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
        for (var id in tabLinks) {
            tabLinks[id].onclick = showTab;
            tabLinks[id].onfocus = function () { this.blur() };
            if (id == tabVal) tabLinks[id].className = 'selected';

            //if any entry tab is clicked, 'Entry' tab is selected as well
            if ("/Home/Entry_CV" == tabVal || "/Home/Entry_DDV" == tabVal || "/Home/Entry_PCV" == tabVal || "/Home/Entry_NC" == tabVal ||
                "/Home/Entry_SS" == tabVal || "/Home/Entry_Liquidation" == tabVal || "/Home/Liquidation_Main" == tabVal) {
                document.getElementById('entry').firstElementChild.className = 'selected';
            }
            //if any home tab is clicked, 'Home' tab is selected as well
            else if ("/Home/Index" == tabVal || "/Home/Pending" == tabVal || "/Home/History" == tabVal || "/Home/Entry_NC" == tabVal) {
                document.getElementById('home').firstElementChild.className = 'selected';
            }
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
        var val = $('#User_UserName').val();
        if (val.length > 0) {
            $('.UN').hide();
        } else {
            $('.UN').show();
        }
    }
    function CheckPw() {
        var val = $('#User_Password').val();
        if (val.length > 0) {
            //alert(val);
            $('.PW').hide();
        } else {
            $('.PW').show();
        }
    }
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
    return Math.round(num * (Math.pow(10, scale))) / Math.pow(10, scale);
};
