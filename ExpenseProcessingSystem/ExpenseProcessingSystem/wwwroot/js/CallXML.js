function getXMLLiqValue (tag) {
    var request = new XMLHttpRequest();
    request.open("GET", "/xml/LiquidationValue.xml", false);
    request.send();
    var xml = request.responseXML;
    return xml.getElementById(tag).textContent;
};