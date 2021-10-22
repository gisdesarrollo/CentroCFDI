$(document).ready(function() {
  (function() {
    let fecha = new Date()
      .toLocaleDateString("es-MX", {
        year: 'numeric',
        month: 'numeric',
        day: 'numeric',
		hour: 'numeric',
		minute: 'numeric',
		second: 'numeric'
      });
	 $('#FechaDocumento').val(fecha);
	 console.log(fecha); 
  }());

});