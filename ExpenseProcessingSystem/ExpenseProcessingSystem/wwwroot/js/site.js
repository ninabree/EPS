$(document).ready(function () {
    var tabLinks = new Array();
    var contentDivs = new Array();

    var $pageInput = $('#paginationInput');

    $pageInput.data("value", $pageInput.val());

    /*//////////////////////////////////////////////////////////////////
    [ Pagination - Number Input]*/

    setInterval(function () {
        var data = $pageInput.data("value"),
            val = $pageInput.val();

        if (data !== val) {
            $pageInput.data("value", val);
            var url = window.location.pathname + '?page=' + val;
            window.location = url;
        }
    }, 100);


    /*//////////////////////////////////////////////////////////////////
    [ Tabs ]*/

    //temp - if page came from login page, localstorage with default to /Home/Index
    if (document.referrer = "https://localhost:44395/") {
        localStorage['tabVal'] = window.location.pathname;
    }
    var tabVal = localStorage['tabVal'] != "" ? localStorage['tabVal'] : "/Home/Index";

    init();

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
});