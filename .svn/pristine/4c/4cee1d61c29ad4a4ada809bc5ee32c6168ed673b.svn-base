$(function () {
    $("#agregarBanco").on('click', function () {
        $.ajax({
            type: 'POST',
            url: '/AjaxCatalogos/BancosClientes',
            data: JSON.stringify({ bancoId: $('#BancoId').val(), nombre: $('#Banco_Nombre').val(), numeroCuenta: $('#Banco_NumeroCuenta').val() }),
            dataType: 'html',
            contentType: "application/json; charset=utf-8",
        }).success(function (partialView) {
            $('#bancos').append(partialView);
            return false;
        })
        .fail(function (jqxhr, textStatus, error) {
            alert("Favor de llenar todos los datos para los detalles");
            return false;
        });
        return false;
    });
});