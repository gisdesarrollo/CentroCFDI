$(document).ready(function() {
    if ($('table').hasClass('clean')) {

    } else {
        var columna = 0;
        var orden = 'desc';
        var responsivo = 'false';
        var descarga = 'fti';
        var varscrollX = 'false';
        var varscrollY = 'false';
        var orderCellsTop = 'false';
        var fixedHeader = 'false';
		var paginacion = false;


        if ($('table').attr("data-columna")) {
            columna = $('table').attr('data-columna');
        }

        if ($('table').attr("data-orden")) {
            orden = $('table').attr('data-orden');
        }

        if ($('table').attr("data-responsivo")) {
            responsivo = $('table').attr('data-responsivo');
        }

        if ($('table').attr("data-descarga")) {
            descarga = "Bftip";
			paginacion = true;
        }

        if ($('table').attr("data-scrollX")) {
            varscrollX = "true";
        }

        if ($('table').attr("data-scrollY")) {
            varscrollY = 500;
        }
		
		if ($('table').attr("data-filtros")) {
            $('.dataTable thead tr').clone(true).appendTo( '.dataTable thead' );
            $('.dataTable thead tr:eq(1) th').each( function (i) {
                var title = $(this).text();
                $(this).html( '<input type="text" placeholder="Buscar '+title+'" />' );
         
                $( 'input', this ).on( 'keyup change', function () {
                    if ( table.column(i).search() !== this.value ) {
                        table
                            .column(i)
                            .search( this.value )
                            .draw();
                    }
                } );
            });
            orderCellsTop = 'true';
            fixedHeader = 'true';
            
		}
		
        var table = $('.dataTable').DataTable({
          "autoWidth": false,
          "stateSave": false,
          "paging": paginacion,
          "order": [columna, orden],
          // responsive: responsivo,
          dom: descarga,
          //scrollX: varscrollX,
          //scrollY: varscrollY,
           orderCellsTop: orderCellsTop,
           fixedHeader: fixedHeader,
           buttons: [{
                    extend: "excel",
                    className: "btn btn-sm",
                    text: '<i class="fa fa-file-excel"></i> Excel'
                },
                {
                    extend: "print",
                    className: "btn btn-sm",
                    text: '<i class="fa fa-print"></i> Imprimir'
                }
            ],
            "language": {
                "search": "Buscar:",
                "emptyTable": "No hay registros en la tabla",
                "info": "Mostrando _START_ a _END_ de _TOTAL_ registros",
                "infoEmpty": "Mostrando 0 registros",
                "infoFiltered": "(De un total de _MAX_ registros)",
                "thousands": ",",
                "lengthMenu": "Mostrar _MENU_ registros",
                paginate: {
                    first: 'Primera',
                    previous: '<',
                    next: '>',
                    last: 'Ultima'
                }
            }
        });		
		/*var table2 = $('#dataTable2').DataTable({
		  "autoWidth": false,
          "stateSave": false,	
		  "processing" : true,
          "serverSide" : true,
		  "destroy": true,
		//orderCellsTop = 'true';
        //fixedHeader = 'true';
		 // "filter": true, 
		  "order": [columna, orden],
		  //agregue al dom r=para search y p=paginacion
		  dom: 'Bfrtip',
        //"orderMulti": false, 
          "paging": true,
		//"iDisplayLength": 15,
		//lengthChange: true,
         // "searching": true,
		//"pageLength": 10,
		 orderCellsTop: orderCellsTop,
         fixedHeader: fixedHeader,
           buttons: [{
                    extend: "excel",
                    className: "btn btn-sm",
                    text: '<i class="fa fa-file-excel"></i> Excel'
                },
                {
                    extend: "print",
                    className: "btn btn-sm",
                    text: '<i class="fa fa-print"></i> Imprimir'
                }
           ],
        ajax: {
            "url": '/FacturasEmitidas/ListadoFacturasEmitidas',
            "dataSrc": 'data'
        },

        columns: [{
            data: 'ReceptorRazonSocial'
        }, {
            data: 'Uuid'
        }, {
            data: 'Serie'
        }, {
            data: 'Folio'
        }, {
            data: 'Fecha'
        }, {
            data: 'TipoComprobante'
        }, {
            data: 'Moneda'
        }, {
            data: 'TipoCambio'
        }, {
            data: 'Subtotal'
        },{
            data: 'Descuento'
        },{
            data: 'TotalImpuestosRetenidos'
        },{
            data: 'TotalImpuestosTrasladados'
        },{
            data: 'Total'
        }],
       
        "language": {
            "search": "Buscar:",
            "emptyTable": "No hay registros en la tabla",
            "info": "Mostrando _START_ a _END_ de _TOTAL_ registros",
            "infoEmpty": "Mostrando 0 registros",
            "infoFiltered": "(De un total de _MAX_ registros)",
            "thousands": ",",
            "lengthMenu": "Mostrar _MENU_ registros",
            paginate: {
                first: 'Primera',
                previous: '<',
                next: '>',
                last: 'Ultima'
            }
        }

    });*/
	
	

        // Handle form submission event
        $("form").submit(function(e) {
            var form = this;

            // Encode a set of form elements from all pages as an array of names and values
            var params = table.$('input,select,textarea').serializeArray();

            // Iterate over all form elements
            $.each(params, function() {
                // If element doesn't exist in DOM
                if (!$.contains(document, form[this.name])) {
                    // Create a hidden element
                    $(form).append(
                        $('<input>')
                        .attr('type', 'hidden')
                        .attr('name', this.name)
                        .val(this.value)
                    );
                }
            });
        });
    }
    // TableManageButtons = function() {
    //     "use strict";
    //     return {
    //         init: function() {
    //             handleDataTableButtons();
    //         }
    //     };
    // }();
    // TableManageButtons.init();
});