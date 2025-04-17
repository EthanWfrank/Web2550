EditId = 0;

$(() => {
    console.log("OnLoad in Code.js");

    LoadWelcomeMessage();
    LoadLocations();
    connectToSQL();
    GetclassData();

    $("#submit").click(() => {
        clientObj = {};
        clientObj.fname = $("[name = 'fname']").val();
        clientObj.age = $("[name = 'age']").val();

        MakeAjaxCall("https://localhost:7067/NameAge", "POST", clientObj, "HTML", NameAgeSuccesss, errorHandler);
    });

    $("#addstudent").click(() => {
        AddStudent();
    });

    $("#senddata").click(() => {
        clientObj = {};
        num1 = 5;

        MakeAjaxCall("https://localhost:7067/sendData/" + num1, "GET", clientObj, "JSON", SendDatasuccess, errorHandler);
    });
})
////////////////////////////////////////////Connecting to sql displaying table///////////////////////////////////////////////
function SendDatasuccess(data) {
    console.log("send data - data " + data);

    console.log(data[0]["num1"]);
    console.log(data[1]["num3"]);

    for (let i = 0; i < data.length; i++) {
        console.log("This one -- " + data[i]["num1"]);
    }

    data.forEach((value, key) => {
        console.log(value["num1"]);
    })

}

function connectToSQL() {
    clientObj = {};

    MakeAjaxCall("https://localhost:7067/select", "GET", clientObj, "JSON", SQLSuccess, errorHandler);
}
function SQLSuccess(data) {
    console.log("Success");
    console.log("look herer" + data);

    // Creating a table to display on main page
    table = "<table>";
    table += "<tr>";
    table += "<th>Select</th>";
    table += "<th>Student ID</th>";
    table += "<th>Last Name</th>";
    table += "<th>First Name</th>";
    table += "<th>School ID</th>"; 
    table += "<th>Action</th>"
    table += "</tr>";

    data.forEach((value, index) => {
        table += "<tr>";
        table += "<td><button onclick = GetData('" + value["studentID"] + "')>Retreive</button></td>";
        table += "<td>" + value["studentID"] + "</td>";
        table += "<td>" + value["lname"] + "</td>";
        table += "<td>" + value["fname"] + "</td>";
        table += "<td>" + value["schoolID"] + "</td>";
        table += "<td><button onclick = EditStud('" + value["studentID"] + "')>Edit</button></td>";
        table += "<td><button onclick = DeleteStud('" + value["studentID"] + "')>Delete</button></td></tr>";
    })
    table += "</table>";

    console.log(table);

    $("#tableDiv").append(table);

}

///////////////////////////////////////////Delete Student//////////////////////////////////////////////////////////////////////
function DeleteStud(id){
    clientObj = {};
    clientObj.id = id;

    MakeAjaxCall("https://localhost:7067/DeleteStud/" + id, "DELETE", {}, "HTML", DeleteSuccess, errorHandler);


}
function DeleteSuccess(data){
    console.log("success");
    $("#tableDiv").empty();
    connectToSQL();
    $("#status").text("You have delete student with " + id);
}

////////////////////////////////////////////EDIT//////////////////////////////////////////////////////////////////////////////
function EditStud(id) {
    clientObj = {};
    EditId = id;
    MakeAjaxCall("https://localhost:7067/select", "GET", clientObj, "JSON", EditSuccess, errorHandler);
}
function EditSuccess(data) {
    console.log(data);
    console.log("Inside Edit");
    $("#tableDiv").empty();

    // Creating a table to display on main page
    table = "<table>";
    table += "<tr>";
    table += "<th>Select</th>";
    table += "<th>Student ID</th>";
    table += "<th>Last Name</th>";
    table += "<th>First Name</th>";
    table += "<th>School ID</th>";
    table += "<th>Action</th>"
    table += "</tr>";

    data.forEach((value, index) => {
        table += "<tr>";
        table += "<td><button onclick = GetData('" + value["studentID"] + "')>Retreive</button></td>";
        table += "<td>" + value["studentID"] + "</td>";
        if (value["studentID"] == EditId) {
            table += "<td><input type='text' id = 'lname" + value["studentID"] + "' value =' " + value["lname"] + "'></td>";
            table += "<td><input type='text' id = 'fname" + value["studentID"] + "' value =' " + value["fname"] + "'></td>";
            table += "<td><input type='text' id = 'schoolId" + value["studentID"] + "' value =' " + value["schoolID"] + "'></td>";
        }
        else {
            table += "<td>" + value["lname"] + "</td>";
            table += "<td>" + value["fname"] + "</td>";
            table += "<td>" + value["schoolID"] + "</td>";
        }
        table += "<td><button onclick = UpdateStud('" + value["studentID"] + "')>Update</button></td>";
        table += "<td><button onclick = Cancel('" + value["studentID"] + "')>Cancel</button></td></tr>";
    });
    table += "</table>";
    $("#tableDiv").append(table);
}

///////////////////////////////////UPDATE///////////////////////////////////////////////////////////////////////////////////
function UpdateStud(id) {
    console.log("inside update Stud");

    clientObj = {};
    clientObj.lname = $("#lname" + id).val();
    clientObj.fname = $("#fname" + id).val();
    clientObj.sid = $("#schoolId" + id).val();
    MakeAjaxCall("https://localhost:7067/UpdateStud/" + id, "PUT", clientObj, "JSON", UpdateSuccess, errorHandler);
}
function UpdateSuccess(data) {
    console.log("success");
    $("#tableDiv").empty();
    connectToSQL();
    $("#status").text("You have delete student with " + id);
}

///////////////////////////////////////Retreive/////////////////////////////////////////////////////////////////////////////
function GetData(id) {
    console.log(id);

    clientObj = {};
    clientObj.sid = id;

    MakeAjaxCall("https://localhost:7067/GetStudentData/", "POST", clientObj, "JSON", StudentDataSuccess, errorHandler);
}
function StudentDataSuccess(data) {
    $("#stData").empty();
    // Creating a table to display on main page
    table = "<table>";
    table += "<tr>";
    table += "<th>ClassID</th>";
    table += "<th>Class_desc</th>";
    table += "<th>days</th>";
    table += "<th>start_date</th>";
    table += "<th>instructors</th>";
    table += "<th>fname</th>";
    table += "<th>lastname</th>";
    table += "</tr>";

    data.forEach((value, index) => {
        table += "<tr>";
        //table += "<td><button onclick = GetData('" + value["studentID"] + "')>Retreive</button></td>";
        table += "<td>" + value["classid"] + "</td>";
        table += "<td>" + value["class_desc"] + "</td>";
        table += "<td>" + value["days"] + "</td>";
        table += "<td>" + value["startDate"] + "</td>";
        table += "<td>" + value["instructor"] + "</td>";
        table += "<td>" + value["firstName"] + "</td>";
        table += "<td>" + value["lastName"] + "</td>";
        table += "</tr>";
    })
    table += "</table>";

    console.log(table);

    $("#stData").append(table);


    //$("#StudentToClass").append(table);
}

///////////////////////////////////Cancel////////////////////////////////////////////////////////////////////////////////////
function Cancel(id) {
    console.log("success");
    $("#tableDiv").empty();
    connectToSQL();

}

/////////////////////////////////Add Student//////////////////////////////////////////////////////////////////////////////////
function GetclassData() {
    MakeAjaxCall("https://localhost:7067/GetClasses", "GET", {}, "HTML", GetclassDataSuccess, errorHandler);
}

function GetclassDataSuccess(data){
    console.log(data);
    $("#classes").append(data);
}

function AddStudent() {

    console.log($("#firstname").val());
    clientObj = {};
    clientObj.firstName = $("#firstname").val();
    clientObj.lastName = $("#lastname").val();
    clientObj.studentId = $("#studid").val();
    clientObj.classes = $("#classes").val();
    console.log($("#classes").val());
    MakeAjaxCall("https://localhost:7067/AddStud/" , "POST", clientObj, "JSON", addStudentSucces, errorHandler);
}

function addStudentSucces(data) {

}


///////////////////////////////////Practice/////////////////////////////////////////////////////////////////////////////////
function NameAgeSuccesss(data) {
    $("#nameAge").text(data)
}

function LoadWelcomeMessage(){
    console.log("LoadWelcomeMessage");
    clientObj = {};


    MakeAjaxCall("https://localhost:7067/welcome", "GET", clientObj, "JSON", WelcomeSuccess, errorHandler);
}

function WelcomeSuccess(data) {

    $("#welcome").text(data.msg);
}

function LoadLocations() {
    clientObj = {};


    MakeAjaxCall("https://localhost:7067/location", "GET", clientObj, "JSON", LocationSuccess, errorHandler);
}

function LocationSuccess(data) {

    data.location.forEach((value, index) =>
    {
        $("#locSelect").append(`<option value= "${value}">${value}</option>`);
    })         
}

function successHandler(data){
    console.log(data);
}
function errorHandler(){
    console.log("Error");
}

function MakeAjaxCall(serverURL, reqMethod, data, serverResponse, successHandler, errorHandler)
{
    console.log("Inside MakeAjaxCall function ");

    let ajaxOptions= {};
    ajaxOptions['url']          = serverURL;                // Destination URL
    ajaxOptions['type']         = reqMethod;                // GET/POST
    ajaxOptions['dataType']     = serverResponse;           // HTML/JSON 
    ajaxOptions['data']         = JSON.stringify(data);     // Client data   -- NEW for ASP PART
    ajaxOptions['success']      = successHandler;           // Callback function to handle successful case
    ajaxOptions['error']        = errorHandler;             // Callback function to handle error 

    ajaxOptions['contentType']  = "application/json";       // NEW for ASP PART

    // actually make ajax call
    $.ajax(ajaxOptions);

}