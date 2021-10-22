using API.Operaciones.Facturacion;
using CFDI.API.Enums.CFDI33;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Operaciones.ComplementosPagos
{
    [Table("ORI_DOCUMENTOSRELACIONADOS")]
    public class DocumentoRelacionado
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public String IdDocumento { get; set; }

        public String Serie { get; set; }

        public String Folio { get; set; }

        public c_Moneda Moneda { get; set; }

        [DisplayName("Tipo de cambio del dia del pago para moneda del documento relacionado")]
        public double TipoCambio { get; set; }

        [DisplayName("Método de Pago")]
        public c_MetodoPago MetodoPago { get; set; }

        [DisplayName("Número de Parcialidad")]
        public int? NumeroParcialidad { get; set; }

        [DisplayName("Importe de Saldo Anterior")]
        public double? ImporteSaldoAnterior { get; set; }

        [DisplayName("Importe Pagado")]
        public double? ImportePagado { get; set; }

        [DisplayName("Importe de Saldo Insoluto")]
        public double? ImporteSaldoInsoluto { get; set; }

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
    }
}
