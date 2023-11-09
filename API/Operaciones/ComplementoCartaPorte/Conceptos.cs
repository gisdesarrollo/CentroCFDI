using API.CatalogosCartaPorte;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using API.Operaciones.ComprobantesCfdi;
using API.Operaciones.ComplementoCartaPorte;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_conceptos")]
    public class Conceptos
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("Clave de producto o servicio")]
        public string ClavesProdServ { get; set; }

        [DisplayName("Clave de producto o servicio")]
        public string ClaveProdServ_Id { get; set; }
        [ForeignKey("ClaveProdServ_Id")]
        public virtual ClaveProdServCP ClaveProdServCP { get; set; }

        [DisplayName("Clave de unidad")]
        public string ClavesUnidad { get; set; }
        [DisplayName("Clave de unidad")]
        public string ClaveUnidad_Id { get; set; }
        [ForeignKey("ClaveUnidad_Id")]
        public virtual ClaveUnidad ClaveUnidad { get; set; }

        public string Unidad { get; set; }
        public string Descripcion { get; set; }

        public string NoIdentificacion { get; set; }

        public string Cantidad { get; set; }
        [DisplayName("Valor Unitario")]
        public string ValorUnitario { get; set; }

        public Double Importe { get; set; }

        public string Descuento { get; set; }

        [DisplayName("Objeto Impuesto")]
        public string ObjetoImpuestoId { get; set; }
        [ForeignKey("ObjetoImpuestoId")]
        public virtual ObjetoImpuesto ObjetoImpuesto { get; set;}

       
        [NotMapped]
        [DisplayName("Catalogo Concepto")]
        public string CatConcepto { get; set; }


        public int? Traslado_Id { get; set; }
        [ForeignKey("Traslado_Id")]
        public virtual TrasladoCP Traslado { get; set; }

        public int? Retencion_Id { get; set; }
        [ForeignKey("Retencion_Id")]
        public virtual RetencionCP Retencion { get; set; }

        public int? Complemento_Id { get; set; }
        [ForeignKey("Complemento_Id")]
        public virtual ComplementoCartaPorte ComplementoCP { get; set; }

        public int? Comprobante_Id { get; set; }
        [ForeignKey("Comprobante_Id")]
        public virtual ComprobanteCfdi ComprobanteCfdi { get; set; }

    }
}
