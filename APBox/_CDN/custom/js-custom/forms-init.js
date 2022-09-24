$(document).ready(function() {

    //Agrega la clase de bootstrap a todos los form elements
    $('input:not(:file):not(:button), select, textarea').addClass('form-control');

    //Hace Readonly a los campos con esta clase
    $('.readonly').attr('readonly', 'readonly');
    $('.disabled').prop('disabled', true);

    // Agrega las clases de bootstrap para todas las tablas que tengan dataTables
    $('.dataTable').addClass('table table-condensed');
    $('table').addClass('table table-condensed font-size-11');

    // Agrega el buscador a un dropdown.
    // Usar la clase .clean para que no tenga el buscador

    $('select:not(.clean)').selectpicker({
        liveSearch: true,
        liveSearchStyle: 'contains',
        title: 'Seleccionar',
        noneSelectedText: 'Sin seleccionar',
        noneResultsText: 'No se ha encontrado {0}',
        size: 'auto'
    });

    //Convierte a todos los inputs de texto en mayasculas
    $(':input:not(".minusculas")').css('text-transform', 'uppercase');
    $('textarea:not(".minusculas")').css('text-transform', 'uppercase');
    $('input[type=text]:not(".minusculas")').keyup(function() {
        $(this).css('text-transform', 'uppercase');
        this.value = this.value.toUpperCase();
    });
    //Deshabilita el boton de submit al momento de hacer clic 
    $(function(setup) {
        $("form").on("submit", function() {
            $(this).find(":submit").prop("disabled", true);
            $(this).find(".multiple").prop("disabled", false);
        });
    });

    //Deshabilita todos los campos en readonly
    if ($('div').hasClass("deshabilitar")) {
        $('.deshabilitar').find('input, textarea, select').not(':hidden').attr("disabled", true);
        $('.deshabilitar').find('svg').removeAttr("onclick");
    }

    //Init Switchery para todos los checkbox
    $("[type='checkbox']:not('.clean')").addClass('js-switch');
    if ($('.js-switch')[0]) {
        var elems = Array.prototype.slice.call(document.querySelectorAll('.js-switch'));
        elems.forEach(function(html) {
            var switchery = new Switchery(html, {
                color: '#26B99A',
                size: 'small'
            });
        });
    }

    //Inicia el Bootstrap Datepicker

    $(function() {
        $('.date').datetimepicker({
            format: 'DD/MM/YYYY',
            sideBySide: true
        });
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

    if (!$('body').hasClass("APBox")) {
        $("[type='datetime']").addClass('date');
        $("[type='date']").addClass('date');

        $("<script/>", {
            src: "~/_CDN/Monarch/custom/widgets-custom/Moment/moment.min.js"
        }).appendTo("head");
        $("<link/>", {
            rel: "stylesheet",
            type: "text/css",
            href: "~/_CDN/Monarch/custom/widgets-custom/datetimepicker/bootstrap-datetimepicker.min.css"
        }).appendTo("head");
        $("<script/>", {
            src: "~/_CDN/Monarch/custom/widgets-custom/datetimepicker/bootstrap-datetimepicker.min.js"
        }).appendTo("head");

        $(function() {
            $('.date').datetimepicker();
            console.log("entro");
        })
    }

    // Asterisco para required fields
    //$("[data-val-required]").attr("required", "true");
    //$('select[data-val-required]').parent().parent().children('label').append('<span class="fas fa-asterisk font-red fa-xs fa-fw mrg5L"></span>');
    $(function() {
        $(':input[data-val-required]').attr("placeholder", "Campo Obligatorio");
    });

    // Modifica clases para la validación del Model State
    $('.validation-summary-errors').addClass('animated bounce');
    $('.validation-summary-errors').removeClass('label-blue-alt');

    var modelState = $('.validation-summary-errors ul li').html();

    if (modelState == 'Comando realizado con éxito') {
        $('.validation-summary-errors').removeClass('text-danger');
        $('.validation-summary-errors').addClass('alert alert-success font-green');
    }
    if (modelState != 'Comando realizado con éxito') {
        $('.validation-summary-errors').removeClass('text-danger');
        $('.validation-summary-errors').addClass('alert alert-warning font-red');
    }


    //Load del CSS cuando hay un input con file upload
    if ($('input').hasClass("file-input")) {
        $("<link/>", {
            rel: "stylesheet",
            type: "text/css",
            href: "//sistemas.infodextra.com/_CDN/Monarch/custom/widgets-custom/bulma-0.7.1/css/bulma.min.css"
        }).appendTo("head");
    }

    // Abre el modal en el máximo height
    function setModalMaxHeight(element) {
        this.$element = $(element);
        this.$content = this.$element.find('.modal-content');
        var borderWidth = this.$content.outerHeight() - this.$content.innerHeight();
        var dialogMargin = $(window).width() < 768 ? 20 : 60;
        var contentHeight = $(window).height() - (dialogMargin + borderWidth);
        var headerHeight = this.$element.find('.modal-header').outerHeight() || 0;
        var footerHeight = this.$element.find('.modal-footer').outerHeight() || 0;
        var maxHeight = contentHeight - (headerHeight + footerHeight);

        this.$content.css({
            'overflow': 'hidden'
        });

        this.$element
            .find('.modal-body').css({
                'max-height': maxHeight,
                'overflow-y': 'auto'
            });
    }

    $('.modal').on('show.bs.modal', function() {
        $(this).show();
        setModalMaxHeight(this);
        $('body').css('padding-right', '0px');
    });

    $(window).resize(function() {
        if ($('.modal.in').length != 0) {
            setModalMaxHeight($('.modal.in'));
        }
    });

    //Init para Cleave - Format Numer 
    // numeral pesos
    $('.formatoPesos').toArray().forEach(function(field) {
        new Cleave(field, {
            numericOnly: true,
            numeral: true,
            numeralThousandsGroupStyle: 'thousand',
            numeralDecimalScale: 2,
            rawValueTrimPrefix: true,
            prefix: '$ '
        })
    });

    // numeral SM
    $('.formatoSM').toArray().forEach(function(field) {
        new Cleave(field, {
            numericOnly: true,
            numeral: true,
            numeralThousandsGroupStyle: 'thousand',
            numeralDecimalScale: 4,
            rawValueTrimPrefix: true,
            prefix: 'SM '
        })
    });

    // numeral SDI
    $('.formatoPorcentaje').toArray().forEach(function(field) {
        new Cleave(field, {
            numericOnly: true,
            numeral: true,
            numeralThousandsGroupStyle: 'thousand',
            numeralDecimalScale: 2,
            rawValueTrimPrefix: true,
            prefix: '% '
        })
    });
});