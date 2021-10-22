$(function () {
    $('#GrupoId').on('change', function () {
        var sucursalSeleccionada = $('#SucursalId');
        var grupoSeleccionado = $(this);

        sucursalSeleccionada.empty();
        sucursalSeleccionada.prop('disabled', '');
        

        sucursalSeleccionada.append($('<option>', {
            text: "Favor de hacer su selección"
        }));

        if (grupoSeleccionado.val().length > 0 && grupoSeleccionado.val() !== 0) {
            $.ajax({
                type: 'POST',
                url: '/AjaxCatalogos/GetSucursales',
                data: JSON.stringify({ grupoId: grupoSeleccionado.val() }),
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    $.each(data, function (i, sucursal) {
                        sucursalSeleccionada.append($('<option>', {
                            value: sucursal.Id,
                            text: sucursal.Nombre
                        }));
                    });
                    return false;
                }
            });
        } else {
            sucursalSeleccionada.prop('disabled', 'disabled');
        }
    });
});