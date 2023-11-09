using API.Catalogos;
using API.Enums;
using API.Operaciones.Facturacion;
using API.Operaciones.RelacionesCfdi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Operaciones.ComplementosPagos
{
    [Table("ori_complementospagos")]
    public class ComplementoPago
    {
        [DisplayName("Folio")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Versión del Complemento")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public String Version { get; set; }

        [NotMapped]
        public virtual Pago Pago { get; set; }
        public virtual List<Pago> Pagos { get; set; }

        [DisplayName("Sucursal")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        //Filtros
        [DisplayName("Receptor")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int ReceptorId { get; set; }
        [ForeignKey("ReceptorId")]
        public virtual Cliente Receptor { get; set; }

        [DisplayName("Fecha del Documento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaDocumento { get; set; }

        //[Required(ErrorMessage = "Campo Obligatorio")]
        public Meses Mes { get; set; }

        public bool Generado { get; set; }

        public Status Status { get; set; }

        [NotMapped]
        public List<DocumentoRelacionado> DocumentosRelacionados { get; set; }

        [DisplayName("Factura Emitida")]
        public int? FacturaEmitidaId { get; set; }
        [ForeignKey("FacturaEmitidaId")]
        public virtual FacturaEmitida FacturaEmitida { get; set; }

        [DisplayName("Exportacion")]
        public string ExportacionId { get; set; }


        [NotMapped]
        public bool Seleccionado { get; set; }

        [NotMapped]
        public string FolioSustitucion { get; set; }

        [NotMapped]
        public string MotivoCancelacion { get; set; }

        #region Cfdis Relacionados

        [DisplayName("Tipo de Relación")]
        public string TipoRelacion { get; set; }

        [NotMapped]
        public string IdTipoRelacion { get; set; }

        [DisplayName("UUID De CFDI Relacionado")]
        public string UUIDCfdiRelacionado { get; set; }
        
        [NotMapped]
        public CfdiRelacionado CfdiRelacionado { get; set; }

        public virtual List<CfdiRelacionado> CfdiRelacionados { get; set; }

        public int? TotalesPagoImpuestoId { get; set; }
        [ForeignKey("TotalesPagoImpuestoId")]
        public virtual TotalesPagosImpuestos TotalesPagosImpuestos { get; set; }

        #endregion

    }
}
