function getXMLLiqValue (tag) {
    var request = new XMLHttpRequest();
    request.open("GET", "/xml/LiquidationValue.xml", false);
    request.send();
    var xml = request.responseXML;
    return xml.getElementById(tag).textContent;
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