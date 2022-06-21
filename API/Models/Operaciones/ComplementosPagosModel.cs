using API.Enums;
using API.Operaciones.ComplementosPagos;
using System;
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

        [DisplayName("Fecha Inicial")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaInicial { get; set; }

        [DisplayName("Fecha Final")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaFinal { get; set; }

        [DisplayName("Estatus")]
        public bool? Estatus { get; set; }

        public virtual List<ComplementoPago> ComplementosPago { get; set; }
    }
}
