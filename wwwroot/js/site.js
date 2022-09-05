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



function onSignIn(googleUser) {
    var profile = googleUser.getBasicProfile();
    console.log('ID: ' + profile.getId()); // Do not send to your backend! Use an ID token instead.
    console.log('Name: ' + profile.getName());
    console.log('Image URL: ' + profile.getImageUrl());
    console.log('Email: ' + profile.getEmail()); // This is null if the 'email' scope is not present.
};