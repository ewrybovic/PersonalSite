"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
    console.log("Rec");
});

connection.on("ReceiveTemp", function (temp) {
    var msg = temp.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    document.getElementById("tempText").innerHTML = msg;
});

connection.on("ReceiveHumid", function (humid) {
    var msg = humid.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    document.getElementById("humidText").innerHTML = msg;
});

connection.on("ReceiveMoist", function (moist) {
    var msg = moist.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    document.getElementById("moistText").innerHTML = msg;
});

connection.on("ResponseMostRecent", function (humid, temp, moist) {
    document.getElementById("humidText").innerHTML = humid;
    document.getElementById("tempText").innerHTML = temp;
    document.getElementById("moistText").innerHTML = moist;
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;

    // Call function to get the last temp and humid data
    connection.invoke("RequestMostRecent").catch(function (err) {
        return console.error(err.toString());
    })

}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});