using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Operaciones.ComplementosPagos
{
    [Table("ORI_IMPUESTOS")]
    public class Impuesto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Total de Impuestos Retenidos")]
        public Double TotalImpuestosRetenidos { get; set; }

        [DisplayName("Total de Impuestos Trasladados")]
        public Double TotalImpuestosTrasladados { get; set; }

        public virtual List<Retencion> Retenciones { get; set; }

        public virtual List<Traslado> Traslados { get; set; }

        //Objetos
        [DisplayName("Pago")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int PagoId { get; set; }
        [ForeignKey("PagoId")]
        public virtual Pago Pago { get; set; }
    }
}
