using API.Enums;
using API.Operaciones.ComplementoCartaPorte;
using CFDI.API.Enums.CFDI33;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Operaciones
{
    public class ComplementosCartaPorteModel
    {
        //[Required(ErrorMessage = "Campo Obligatorio")]
        public Meses Mes { get; set; }

        [DisplayName("Año")]
        //[Required(ErrorMessage = "Campo Obligatorio")]
        public int Anio { get; set; }

        [DisplayName("Fecha Inicial")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaInicial { get; set; }

        [DisplayName("Fecha Final")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaFinal { get; set; }

        [DisplayName("Tipo de Comprobante")]
        public c_TipoDeComprobante? TipoDeComprobante { get; set; }

        [DisplayName("Tipo Transporte")]
        public string ClaveTransporteId { get; set; }

        [DisplayName("Estatus")]
        public bool? Estatus { get; set; }

        public virtual List<ComplementoCartaPorte>ComplementosCartaPorte { get; set; }
    }
}
