"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();


connection.start().then(function () {
    
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

connection.on("ReceiveMessage", function (message) {
    var li = document.createElement("li");
    var divTitle = document.createElement("h5");
    var divText = document.createElement("div");
    var hr = document.createElement("hr");
    var divLike = document.createElement("div");
    var imageLike = document.createElement("img");
    var imageDislike = document.createElement("img");
    divLike.appendChild(imageLike);
    divLike.appendChild(imageDislike);

    var image = document.createElement("img");
    var imageFromHtml = document.getElementById("image-character").src;
    var imageFromHtmlLike = document.getElementById("image-like").src;
    var imageFromHtmlDislike = document.getElementById("image-dislike").src;

    image.src = imageFromHtml;
    li.appendChild(divTitle);
    li.appendChild(divText);
    li.appendChild(hr);
    li.appendChild(divLike);

    document.getElementById("messagesList").appendChild(image);
    document.getElementById("messagesList").appendChild(li);

    divTitle.textContent = "Some title";
    if (message.length == 0) {
        divText.textContent = "Empty field";
    }
    else {
        divText.textContent = message;
    }

    //imageLike.src = "~/images/inactive+.svg";
    //imageDislike.src = "~/images/inactive.svg";
    imageLike.src = imageFromHtmlLike;
    imageDislike.src = imageFromHtmlDislike;
});