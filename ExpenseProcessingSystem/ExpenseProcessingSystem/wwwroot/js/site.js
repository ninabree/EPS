
$(document).ready(function () {
    CheckUn();
    CheckPw();
    populateCol();
    var tabLinks = new Array();
    var contentDivs = new Array();
    $('.tbl-btn:has(button[disabled])').addClass('btnDisabled');

    var $pageInput = $('#paginationInput');
    $pageInput.data("value", $pageInput.val());
    ////DM ON LOAD

    
    
    /////END DM ON LOAD
    $('#um').find('a').click(function () {
        ClearValidations();
    });
    /*//////////////////////////////////////////////////////////////////
    [ Login ]*/
    $(document).on("change", "'#User_UserName", function (e) {
        CheckUn();
    });
    $(document).on("change", "'#User_Password", function (e) {
        CheckPw();
    });

    /*//////////////////////////////////////////////////////////////////
    [ Modal ]*/
    setInterval(function () {
        $('div.modal-backdrop.fade.in').not(':first').remove();
    }, 1);

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
    }, 1000);


    
    /*//////////////////////////////////////////////////////////////////
    [ UM ]*/

    //to highlight a table row in UM
    var rows = $('#um-tbl tbody tr');
    rows.on('click', function(e) {
        /* Get current row */
        var row = $(this);
        /* Check if 'Ctrl', 'cmd' or 'Shift' keyboard key was pressed
         * 'Ctrl' => is represented by 'e.ctrlKey' or 'e.metaKey'
         * 'Shift' => is represented by 'e.shiftKey' */
        if ((e.ctrlKey || e.metaKey) || e.shiftKey) {
            /* If pressed highlight the other row that was clicked */
            //row.addClass('highlight');
        } else {
            var name = $(row).children(":eq(1)").text().split(', ');
            $('#NewAcc_User_UserName').prop("readonly", true);
            $('#NewAcc_User_UserName').val($(row).children(":first").text());
            $('#NewAcc_User_FName').val(name[1]);
            $('#NewAcc_User_LName').val(name[0]);
            $('#NewAcc_User_DeptID').val($(row).children(":eq(2)").attr('id'));
            $('#NewAcc_User_Email').val($(row).children(":eq(4)").text());
            $('#NewAcc_User_Role').val($(row).children(":eq(3)").text());
            $('#NewAcc_User_ID').val(parseInt($(row).children(":eq(12)").val())); 
            $('#NewAcc_User_Comment').val($(row).children(":eq(5)").text());
            $('#NewAcc_User_InUse').prop('checked', ($(row).children(":eq(6)").find('input').is(":checked")) ? true : false);
           
            /* Otherwise just highlight one row and clean others */
            rows.removeClass('highlight');
            row.addClass('highlight');
        }
        /* This 'event' is used just to avoid that the table text 
         * gets selected (just for styling). */
        $(document).bind('selectstart dragstart', function (e) {
            e.preventDefault(); return false;
        });
        
        $('#clear-btn').click(function (e) {
            e.stopImmediatePropagation();
            $('#NewAcc_User_UserName').prop("readonly", false);
            $('#NewAcc_User_UserName').val("");
            $('#NewAcc_User_FName').val("");
            $('#NewAcc_User_LName').val("");
            $('#NewAcc_User_DeptID').val("0");
            $('#NewAcc_User_Email').val("");
            $('#NewAcc_User_Role').val("");
            $('#NewAcc_User_UserID').val("");
            $('#NewAcc_User_Comment').val("");
            $('#NewAcc_User_InUse').prop('checked', false);
            $('#validationSummary').empty();
            rows.removeClass('highlight');
            return false;
        });
    });
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
                "/Home/Entry_SS" == tabVal || "/Home/Entry_Liquidation" == tabVal) {
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
        if (val != "") {
            //alert(val);
            $('.UN').hide();
        } else {
            $('.UN').show();
        }
    }
    function CheckPw() {
        var val = $('#User_Password').val();
        if (val != "") {
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
