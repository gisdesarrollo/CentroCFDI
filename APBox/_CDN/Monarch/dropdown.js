
$(document).ready(function () {
	 
 	$('#dataTable2').DataTable({
		//'processing' : true,
		//'serverSide' : true,
		//paging: true,
    	//searching: true,
		ajax: {
                    "url": "@Url.Action('ListadoFacturasEmitidas')",
                    "dataSrc": ''
                },
		
		columns : [{
            "data" : 'Id'	 
		},{
			"data" : 'Version'
		},{
		    "data" : 'Emisor.Nombre'
		},{
		    "data" : 'Receptor.RazonSocial'
		},{
			"data" : 'Fecha'
		},{
		    "data" : 'Serie'
		},{
			"data" : 'Folio'
		},{
		    "data" : "Moneda"
		}]
		//dataSrc : ""
	
		/*"lengthChange": true,
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
            }*/
		
	});   	
	
});
