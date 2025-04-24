$(() => {
    console.log("inside the java");


    MakeAjaxCall("/Select", "GET", {}, "HTML", SelectSuccess, errorHandler);

    $("#Select1").click(() => {
        MakeAjaxCall("/SelectOne", "GET", {}, "HTML", SelectOneSuccess, errorHandler);
    });

    $("#addItem").click(() => {

        let itemname = $("#newItem").val();
        let itemPrice = $("#newPrice").val();
        let itemID = $("#ItemID").val();

        MakeAjaxCall("/add/" +itemname + "/" + itemPrice  + "/" + itemID, "PUT", {}, "JSON", addSuccess, errorHandler);
    })

    $("#delete").click(() => {

        id = $("#ID").val();
        console.log(id);

        MakeAjaxCall("/delete/" + id, "DELETE", {}, "JSON", deleteSuccess, errorHandler);
    })

    $("#UpdateItem").click(() => {

        clientObj = {};
        clientObj.name = $("#newItem").val();
        clientObj.price = $("#newPrice").val();
        clientObj.id = $("#ItemID").val();

        console.log(clientObj.ID);

        MakeAjaxCall("/Update", "PUT", clientObj, "JSON", UpdateSuccess, errorHandler);
    })

})


function SelectSuccess(data) {
    console.log(data);
    $("#selectTable").html(data);
}
function SelectOneSuccess(data) {
    console.log(data);
    $("#selectTable").empty();
    $("#selectTable").html(data);
}
function addSuccess(data) {
    console.log(data.status);
}
function deleteSuccess(data) {

}
function UpdateSuccess(data) {

}
function errorHandler(data) {
    console.log("error");
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