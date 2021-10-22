using API.Enums;
using API.Operaciones.ComplementosPagos;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Operaciones
{
    public class ComplementosPagosModel
    {
        [Required(ErrorMessage = "Campo Obligatorio")]
        public Meses Mes { get; set; }

        [DisplayName("Año")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int Anio { get; set; }

        public virtual List<ComplementoPago> ComplementosPago { get; set; }
    }
}
