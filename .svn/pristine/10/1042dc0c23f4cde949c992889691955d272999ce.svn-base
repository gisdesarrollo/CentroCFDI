$(function () {
    $("#agregarSucursal").on('click', function () {
        $.ajax({
            type: 'POST',
            url: '/AjaxCatalogos/SucursalesUsuarios',
            data: JSON.stringify({ sucursalId: $('#SucursalId').val() }),
            dataType: 'html',
            contentType: "application/json; charset=utf-8",
        }).success(function (partialView) {
            $('#sucursales').append(partialView);
        })
        .fail(function (jqxhr, textStatus, error) {
            alert("Favor de llenar todos los datos para los detalles");
        });
        return false;
    });
});