using API.Catalogos;
using API.CatalogosCartaPorte;
using API.Enums;
using API.Enums.CartaPorteEnums;
using API.Operaciones.Facturacion;
using API.Operaciones.RelacionesCfdi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_complementocartaporte")]
    public class ComplementoCartaPorte
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Forma de Pago")]
        public string FormaPago { get; set; }
        [ForeignKeyAttribute("FormaPago")]
        public virtual FormaPagos FormaPagos { get; set; }

        [NotMapped]
        public string IdFormaPago { get; set; }

        [DisplayName("Método de Pago")]
        public c_MetodoPago? MetodoPago { get; set; }

        [DisplayName("Tipo Cambio")]
        public string TipoCambio { get; set; }

        [DisplayName("Condiciones Pago")]
        public string CondicionesPago { get; set; }

        public string Descuento { get; set; }

        [DisplayName("Transporte Internacional")]
        public Boolean TranspInternac { get; set; }


        [DisplayName("Salida o Entrada de Mercancia")]
        public string EntradaSalidaMerc { get; set; }

        [DisplayName("Uso Cfdi")]
        public c_UsoCfdiCP UsoCfdiCP { get; set; }

        [DisplayName("Total de Distancia Recorrida")]
        public Decimal TotalDistRec { get; set; }

        [DisplayName("Pais de origen destino")]
        public string PaisOrigendestino { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public string Version { get; set; }

        [NotMapped]
        public Conceptos Conceptos { get; set; }

        //[NotMapped]
        public virtual List<Conceptos> Conceptoss { get; set; }

        [DisplayName("Tipo Transporte")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public string ClaveTransporteId { get; set; }
        [ForeignKey("ClaveTransporteId")]
        public virtual CveTransporte claveTransportes { get; set; }

        [DisplayName("Vía de Entrada o Salida")]
        public String viaEntradaSalida { get; set; }


        [Required]
        [DisplayName("Tipo de Comprobante")]
        public c_TipoDeComprobante TipoDeComprobante { get; set; }

        public c_Moneda? Moneda { get; set; }

        public Decimal Subtotal { get; set; }

        public Decimal Total { get; set; }

        [NotMapped]
        public virtual Ubicacion Ubicacion { get; set; }

        //[NotMapped]
        public virtual List<Ubicacion> Ubicaciones { get; set; }
        public int Mercancias_Id { get; set; }

        [ForeignKey("Mercancias_Id")]
        public virtual Mercancias Mercancias { get; set; }

        [NotMapped]
        public virtual List<Mercancia> Mercanciass { get; set; }

        [NotMapped]
        public virtual List<DocumentacionAduanera> DocumentacionAduanerass { get; set; }

        [NotMapped]
        public virtual List<GuiasIdentificacion> GuiasIdentificacioness { get; set; }

        [NotMapped]
        public virtual List<CantidadTransportada> CantidadTransportadass { get; set; }

        [NotMapped]
        public virtual List<Remolques> Remolques { get; set; }

        [NotMapped]
        public virtual List<RemolqueCCP> RemolqueCCPs { get; set; }

        [NotMapped]
        public virtual List<ContenedorM> ContenedoresM { get; set; }

        [NotMapped]
        public virtual List<DerechosDePasos> DerechosDePasoss { get; set; }

        [NotMapped]
        public virtual List<ContenedorC> ContenedoresC { get; set; }

        [NotMapped]
        public virtual List<Carro> Carros { get; set; }

        [NotMapped]
        public virtual List<PartesTransporte> PartesTransportes { get; set; }

        [NotMapped]
        public virtual TiposFigura TiposFigura { get; set; }

        //[NotMapped]
        public virtual List<TiposFigura> FiguraTransporte { get; set; }

        [NotMapped]
        public Boolean ValidaMaterialPeligroso { get; set; }

        [DisplayName("Referencia")]
        public String ReferenciaAddenda { get; set; }

        //campos nuevos version 3.0 CartaPorte
        public String IdCCP { get; set; }
        [DisplayName("Regimen Aduanero")]
        public c_RegimenAduanero? RegimenAduanero { get; set; }
        [DisplayName("Registro ISTMO")]
        public bool RegistroIstmo { get; set; }
        [DisplayName("Ubicación polo origen")]
        public c_RegistroIstmo? UbicacionPoloOrigen { get; set; }
        [DisplayName("Ubicación polo destino")]
        public c_RegistroIstmo? UbicacionPoloDestino { get; set; }
        

        #region Campos CFDI

        [DisplayName("Sucursal")]
        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        [NotMapped]
        public int IDCliente { get; set; }

        [DisplayName("Receptor")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int ReceptorId { get; set; }
        [ForeignKey("ReceptorId")]
        public virtual SocioComercial Receptor { get; set; }

        [DisplayName("Fecha del Documento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaDocumento { get; set; }

        public Decimal TotalImpuestoTrasladado { get; set; }

        public Decimal TotalImpuestoRetenidos { get; set; }


        //[DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime Hora { get; set; }

        //[Required(ErrorMessage = "Campo Obligatorio")]
        public Meses Mes { get; set; }

        public bool Generado { get; set; }

        public Status Status { get; set; }

        [DisplayName("Factura Emitida")]
        public int? FacturaEmitidaId { get; set; }
        [ForeignKey("FacturaEmitidaId")]
        public virtual FacturaEmitida FacturaEmitida { get; set; }

        [DisplayName("Exportacion")]
        public string ExportacionId { get; set; }
        [ForeignKey("ExportacionId")]
        public virtual Exportacion Exportacion { get; set; }

        [NotMapped]
        public bool hidden { get; set; }

        [NotMapped]
        public bool Seleccionado { get; set; }

        [NotMapped]
        [DisplayName("Motivo Cancelación")]
        public string MotivoCancelacion { get; set; }

        [NotMapped]
        [DisplayName("Folio sustitución")]
        public string FolioSustitucion { get; set; }
        #endregion


        #region Cfdis Relacionados

        [DisplayName("Tipo de Relación")]
        public string TipoRelacion { get; set; }

        [NotMapped]
        public string IdTipoRelacion { get; set; }

        [DisplayName("UUID De CFDI Relacionado")]
        public String UUIDCfdiRelacionado { get; set; }
        
        [NotMapped]
        public CfdiRelacionado CfdiRelacionado { get; set; }
        
        public virtual List<CfdiRelacionado> CfdiRelacionados { get; set; }
        #endregion


    }

}
