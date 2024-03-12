$(document).ready(function() {

    // Agrega las clases de bootstrap para todas las tablas que tengan dataTables
    $('.dataTable').addClass('table table-condensed');
    $('table').addClass('table table-condensed');


    $(function() {
        $("#Pago_FechaPago").datetimepicker({
            defaultDate: new Date(),
            format: 'DD/MM/YYYY hh:mm:ss',
            sideBySide: true
        });
    });

    $(function() {
        $("#Ubicacion_UbicacionOrigen_FechaHoraSalida").datetimepicker({
            defaultDate: new Date(),
            format: 'DD/MM/YYYY hh:mm:ss',
            sideBySide: true
        });
    });

    $(function() {
        $("#Ubicacion_UbicacionDestino_FechaHoraProgLlegada").datetimepicker({
            defaultDate: new Date(),
            format: 'DD/MM/YYYY hh:mm:ss',
            sideBySide: true
        });
    });

});