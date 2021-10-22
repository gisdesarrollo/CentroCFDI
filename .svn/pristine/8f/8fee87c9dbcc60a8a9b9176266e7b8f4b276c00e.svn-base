using API.Enums;
using API.Operaciones.ComplementosPagos;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Models.ComplementosPagos
{
    public class CargasComplementosModel : Archivos
    {
        [DisplayName("Previsualización")]
        public bool Previsualizacion { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public Meses Mes { get; set; }

        [DisplayName("Grupo")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int GrupoId { get; set; }

        public virtual List<ComplementoPago> Detalles { get; set; }

    }
}