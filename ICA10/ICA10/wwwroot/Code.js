$(() => {
    console.log("CODE.js worked");

    MakeAjaxCall("https://localhost:7041/OnLoad", "GET", {}, "JSON", successHandler, errorHandler)

})

function successHandler(data) {

    console.log("data " + data);
    

    // table = "<table>";
    // table += "<tr>";
    // table += "<th>1</th>";
    // table += "<th>2</th>";
    // table += "<th>3</th>";
    // table += "<th>4</th>";
    // table += "<th>5</th>";
    // table += "<th>6</th>";
    // table += "</tr>";

    // data.orders.forEach((value, index) => {
    //     table += "<tr>";
    //     table += "<td>" + value["orderId"] + "</td>";
    //     table += "<td>" + value["locationid"] + "</td>";
    //     table += "<td>" + value["cid"] + "</td>";
    //     table += "<td>" + value["paymentMethod"] + "</td>";
    //     table += "<td>" + value["itemid"] + "</td>";
    //     table += "<td>" + value["itemCount"] + "</td>";
    //     table += "</tr>";
    // })

    // table += "</table>";

    // $("#table1").html(table);
}
function errorHandler() {
    console.log("Error");
}

function MakeAjaxCall(serverURL, reqMethod, data, serverResponse, successHandler, errorHandler) {
    console.log("Inside MakeAjaxCall function ");

    let ajaxOptions = {};
    ajaxOptions['url'] = serverURL;                // Destination URL
    ajaxOptions['type'] = reqMethod;                // GET/POST
    ajaxOptions['dataType'] = serverResponse;           // HTML/JSON 
    ajaxOptions['data'] = JSON.stringify(data);     // Client data   -- NEW for ASP PART
    ajaxOptions['success'] = successHandler;           // Callback function to handle successful case
    ajaxOptions['error'] = errorHandler;             // Callback function to handle error 

    ajaxOptions['contentType'] = "application/json";       // NEW for ASP PART

    // actually make ajax call
    $.ajax(ajaxOptions);

}