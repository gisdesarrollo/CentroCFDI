/* <reference path="datatable-init.js" />*/
$(document).ready(function () {
    if ($('table').hasClass("table")) {
        $.ajax({
            url: "/_CDN/custom/js-custom/dataTable-init.js",
            dataType: "script",
            cache: true,
            success: function (status) {
                console.log(status);
            },
            error: function (xhr, textStatus, error) {
                console.log(xhr.responseText);
                console.log(xhr.statusText);
                console.log(textStatus);
                console.log(error);
            },
        });
    };
});