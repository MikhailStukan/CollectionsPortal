// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).ready(function () {
    autoComplete();
});


let x = 0;

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