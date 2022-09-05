// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).ready(function () {
    autoComplete
    document.getElementById('descriptionData').innerHTML = marked.parse(document.getElementById('desc').innerHTML)
});


let x = 0;


$("#textAreaDescription").on('input', function (e) {
    document.getElementById('descriptionPreview').innerHTML = marked.parse(e.target.value);
});



function addInput() {
    var str = '<div id="customField"><input class="form-control" name="Fields[' + x + '].Name" placeholder="Name of field"> <select class="form-select" name="Fields[' + x + '].DataType"><option value="integer">Number</option><option value="boolean">CheckBox</option><option value="dateTime">Date</option><option value="textarea">Big text</option><option value="text">Small text</option></select></div>';
    $("#fields").append(str);
    x++;
}

function autoComplete() {
    $("#tag").autocomplete({
        source: '/api/GetTags'
    });
}

var intervalId = window.setInterval(function () {
    fetchComments();
}, 3000);

function fetchComments() {
    var itemId = $("#itemId").text();
    $.ajax({
        url: "/api/comments",
        type: "POST",
        data: JSON.stringify(itemId),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            updateComments(response);
        },
        error: function (response) {
            console.log('error:' + response);
        },
    });
};

function updateComments(response) {
    $("#commentsData").empty();
    for (let i = 0; i < response.length; i++) {
        appendComment(response[i]);
    }
};

function appendComment(comment) {
    var date = new Date(comment.createdDate);
    $("#commentsData").append(
        '<div class="w-100" style="margin:10px; background-color: ghostwhite; padding: 20px;">'+
           '<div class="d-flex justify-content-between align-items-center mb-3">' +
                '<h6 class="text-primary fw-bold mb-0">' + 
                    '<b>'+ comment.userName  +'</b>' +
                '<span class="text-dark ms-2">' + comment.content + '</span>' +
        '</h6> ' +
        '<span class="text-dark ms-5">' + date.toLocaleString() + '</span>' +
        '</div>' +
     '</div>'
    )
};