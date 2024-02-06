$(document).ready(function() {

    // Agrega las clases de bootstrap para todas las tablas que tengan dataTables
    $('.dataTable').addClass('table table-condensed');
    $('table').addClass('table table-condensed font-size-11');


    //Convierte a todos los inputs de texto en mayasculas
    $(':input:not(".minusculas")').css('text-transform', 'uppercase');
    $('textarea:not(".minusculas")').css('text-transform', 'uppercase');
    $('input[type=text]:not(".minusculas")').keyup(function() {
        $(this).css('text-transform', 'uppercase');
        this.value = this.value.toUpperCase();
    });

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