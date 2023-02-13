using API.CatalogosCartaPorte;
using API.Enums;
using API.Operaciones.Facturacion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Operaciones.ComplementosPagos
{
    [Table("ori_documentosrelacionados")]
    public class DocumentoRelacionado
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public String IdDocumento { get; set; }

        public String Serie { get; set; }

        public String Folio { get; set; }

        public c_Moneda Moneda { get; set; }

        [DisplayName("Equivalencia")]
        public Double EquivalenciaDR { get; set; }

       /* [DisplayName("Método de Pago")]
        public c_MetodoPago MetodoPago { get; set; }
       */
        [DisplayName("Número de Parcialidad")]
        public int? NumeroParcialidad { get; set; }

        [DisplayName("Importe de Saldo Anterior")]
        public double? ImporteSaldoAnterior { get; set; }

        [DisplayName("Importe Pagado")]
        public double? ImportePagado { get; set; }

        [DisplayName("Importe de Saldo Insoluto")]
        public double? ImporteSaldoInsoluto { get; set; }

        // CFDI40
        [DisplayName("Objeto Impuesto Doc Relacionado")]
        public String ObjetoImpuestoId { get; set; }
        [ForeignKey("ObjetoImpuestoId")]
        public virtual ObjetoImpuesto ObjetoImpuesto { get; set; }

        [NotMapped]
        public string DescripcionObjetoImpuesto { get; set; }

        [DisplayName("Factura Emitida")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int FacturaEmitidaId { get; set; }
        [ForeignKey("FacturaEmitidaId")]
        public virtual FacturaEmitida FacturaEmitida { get; set; }

        //Objetos
        [DisplayName("Pago")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int PagoId { get; set; }
        [ForeignKey("PagoId")]
        public virtual Pago Pago { get; set; }


        //relacion con traslados y retenciones CFDI40
        [NotMapped]
        public TrasladoDR Traslado { get; set; }
        public virtual List<TrasladoDR> Traslados { get; set; }

        [NotMapped]
        public RetencionDR Retencion { get; set; }
        public virtual List<RetencionDR> Retenciones { get; set; }
        /*public int? TrasladoDRId { get; set; }
        [ForeignKey("TrasladoDRId")]
        public virtual TrasladoDR Traslado { get; set; }

        public int? RetencionDRId { get; set; }
        [ForeignKey("RetencionDRId")]
        public virtual RetencionDR Retencion { get; set; }*/

    }
}
