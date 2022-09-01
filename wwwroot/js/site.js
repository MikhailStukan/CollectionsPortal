// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).ready(function () {
    autoComplete();
});


let x = 0;

function addInput() {
    var str = '<input class="form-control" name="Fields[' + x + '].Name" placeholder="NameCollection">';
    document.getElementById('fields').innerHTML += str;
    x++;
}

function autoComplete() {
    $("#tag").autocomplete({
        source: '/api/GetTags'
    });
}