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
    //function validarYcargarScript() {
    //    // Verificar si existe un elemento con la clase '.date'
    //    if ($('.fecha').length > 0) {
    //        // Crear un elemento script
    //        var script = document.createElement('script');

    //        // Configurar el atributo src con la URL del script que deseas cargar
    //        script.src = 'https://unpkg.com/flatpickr/dist/l10n/es.js';

    //        // Adjuntar el elemento script al final del cuerpo del documento
    //        document.body.appendChild(script);
    //    }
    //}
    //$(document).ready(function () {
    //    validarYcargarScript();
    //    $(".fecha").flatpickr({
    //        dateFormat: "d/m/Y",
    //        locale: "es"
    //    });
    //});
});

$(".fecha").flatpickr({
    dateFormat: "d/m/Y",
    locale: "es"
});


//Funcion para agregar el plugin de select2 a todos los select
$('select').attr('data-control', 'select2');
$('select').addClass('form-select');
$('select').select2();


// Función para insertar el contenido en el div con id "botones"
function insertarContenido() {
    // Obtener el elemento con id "contenido"
    var contenidoDiv = document.getElementById('tools');

    // Obtener el elemento con id "botones"
    var botonesDiv = document.getElementById('toolbar');

    // Insertar el contenido dentro del div con id "botones"
    botonesDiv.appendChild(contenidoDiv);
}

// Llamar a la función cuando se carga la página
window.onload = function () {
    insertarContenido();
};
