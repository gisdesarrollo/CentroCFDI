$(document).ready(function() {
    if ($('table').hasClass("dataTable")) {
        $.ajax({
            url: "//cdn.datatables.net/v/bs/jszip-2.5.0/dt-1.10.16/b-1.5.1/b-colvis-1.5.1/b-flash-1.5.1/b-html5-1.5.1/b-print-1.5.1/fh-3.1.3/r-2.2.1/sc-1.4.4/sl-1.2.5/datatables.min.js",
            dataType: "script",
            cache: true,
        });
    }
    if ($('table').hasClass("dataTable")) {
        $.ajax({
            url: "//cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.32/vfs_fonts.js",
            dataType: "script",
            cache: true
        });
    }
    if ($('table').hasClass("dataTable")) {
        $.ajax({
            url: "//cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.32/pdfmake.min.js",
            dataType: "script",
            cache: true
        });
    }
});