function getXMLLiqValue (tag) {
    var request = new XMLHttpRequest();
    request.open("GET", "/xml/LiquidationValue.xml", false);
    request.send();
    var xml = request.responseXML;
    return xml.getElementById(tag).textContent;
};
//function getXMLLiqValueAccount(tag) {
//    var request = new XMLHttpRequest();
//    request.open("GET", "/xml/LiquidationValue.xml", false);
//    request.send();
//    var xml = request.responseXML;
//    return getAccountByMasterIDFunc("1", xml.getElementById(tag).textContent);
//};
function getXMLLiqValueAccount(tag, no) {
    var request = new XMLHttpRequest();
    request.open("GET", "/xml/LiquidationValue.xml", false);
    request.send();
    var xml = request.responseXML;
    return getAccountByMasterIDFunc(no, xml.getElementById(tag).textContent);
};
function getXMLNCAccs(tag) {
    var request = new XMLHttpRequest();
    request.open("GET", "/xml/NonCashAccounts.xml", false);
    request.send();
    var xml = request.responseXML;
    return xml.getElementById(tag).textContent;
};
function getXMLBudgetMonitoring(tag) {
    var request = new XMLHttpRequest();
    request.open("GET", "/xml/BudgetMonitoring.xml", false);
    request.send();
    var xml = request.responseXML;
    return xml.getElementById(tag).textContent;
};

//get account Information
function getAccountByMasterIDFunc(info, masterID) {
    var tmp = "";
    var url = "";

    //"1" == Account Incremental ID
    if (info == "1") {
        url = '/Home/getAccountStr';
    }
    if (info == "2") {
        url = '/Home/getAccountCodeStr';
    }
    $.ajax({
        url: url,
        type: "POST",
        dataType: 'text',
        async: false,
        data: { masterID: masterID+"" },
        success: function (data) {
            if (data != "null") {
                    tmp = data;
            }
        },
        error: function (xhr) {
            alert('error');
        }
    });
    return tmp;
}