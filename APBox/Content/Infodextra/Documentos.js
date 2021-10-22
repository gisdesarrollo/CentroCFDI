$(function () {
    $('#TipoDocumento').on('change', function () {
        var tipoDocumento = $(this).prop('selectedIndex');

        switch (tipoDocumento) {
            case 0:
                $('.Cfdi').hide();
                $('.Comprobante').hide();

                $('#FechaDocumento').prop("readonly", false);
                $('#SubTotal').prop("readonly", false);
                $('#Total').prop("readonly", false);

                break;
            case 1:
                $('.Cfdi').show();
                $('.Comprobante').hide();

                $('#FechaDocumento').prop("readonly", true);
                $('#SubTotal').prop("readonly", true);
                $('#Total').prop("readonly", true);

                $('#FechaDocumento').val("");
                $('#SubTotal').val("");
                $('#Total').val("");
                break;
            case 2:
                $('.Cfdi').hide();
                $('.Comprobante').show();

                $('#FechaDocumento').prop("readonly", false);
                $('#SubTotal').prop("readonly", false);
                $('#Total').prop("readonly", false);
                break;
            case 3:
                $('.Cfdi').hide();
                $('.Comprobante').show();

                $('#FechaDocumento').prop("readonly", false);
                $('#SubTotal').prop("readonly", false);
                $('#Total').prop("readonly", false);
                break;
            case 4:
                $('.Cfdi').hide();
                $('.Comprobante').show();

                $('#FechaDocumento').prop("readonly", false);
                $('#SubTotal').prop("readonly", false);
                $('#Total').prop("readonly", false);
                break;
            default:
                alert('Selección Incorrecta');
        }

        return false;
    });
});