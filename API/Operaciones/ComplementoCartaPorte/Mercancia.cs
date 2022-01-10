using API.CatalogosCartaPorte;
using API.Catalogos;
using CFDI.API.Enums.CFDI33;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_Mercancia")]
    public class Mercancia
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(8)]
        //[Required(ErrorMessage = "Campo Obligatorio")]
        [DisplayName("Clave Bienes Transportados")]
        public String ClaveProdServCP { get; set; }
        /*[ForeignKey("ClaveProdServCP_Id")]
        public virtual ClaveProdServCP ClaveProdServCP { get; set; }*/
        [NotMapped]
        public string NameBienesTransp { get; set; }

        //[Required(ErrorMessage = "Campo Obligatorio")]
        public int Cantidad { get; set; }

        [DisplayName("Clave STCC")]
        public string ClaveProdSTCC { get; set; }
        /*[ForeignKey("ClaveProdSTCC_Id")]
        public virtual ClaveProdSTCC ClaveProdSTCC { get; set; }*/
        [NotMapped]
        public string NameClaveSTCC { get; set; }

        [DisplayName("Clave de Unidad")]
        //[Required(ErrorMessage = "Campo Obligatorio")]
        public string ClaveUnidad_Id { get; set; }
        [ForeignKey("ClaveUnidad_Id")]
        public virtual ClaveUnidad ClaveUnidad { get; set; }

        [DisplayName("Clave de Unidad")]
        public string ClavesUnidad { get; set; }

        [DisplayName("Clave de Material Peligroso")]
        public string MaterialPeligroso_Id { get; set; }
        [ForeignKey("MaterialPeligroso_Id")]
        public virtual MaterialPeligroso MaterialPeligroso { get; set; }

        /*[NotMapped]
        [DisplayName("Material Peligroso")]
        public string MaterialPeligrosos { get; set; }*/

        [DisplayName("Clave de Material Peligroso")]
        public string ClaveMaterialPeligroso { get; set; }

        [DisplayName("Material Peligroso")]
        public Boolean MaterialPeligrosos { get; set; }
        
        [DisplayName("Descripción")]
        //[Required(ErrorMessage = "Campo Obligatorio")]
        public string Descripcion { get; set; }

        public string Dimensiones { get; set; }

        [DisplayName("Clave Embalaje")]
        public string TipoEmbalaje_Id { get; set; }
        [ForeignKey("TipoEmbalaje_Id")]
        public virtual TipoEmbalaje TipoEmbalaje { get; set; }
        /*[NotMapped]
        public string Embalaje { get; set; }*/

        
        [DisplayName("Tipo Embalaje")]
        public string DescripEmbalaje { get; set; }

        [DisplayName("Fracción Arancelaria")]
        public string FraccionArancelaria_Id { get; set; }
        [ForeignKey("FraccionArancelaria_Id")]
        public virtual FraccionArancelaria FraccionArancelaria { get; set; }
        
        [DisplayName("Clave Fracción Arancelaria")]
        public string  FraccionArancelarias { get; set; }
        /*
        [NotMapped]
        [DisplayName("Descripción de Fracción Arancelaria")]
        public string DescripcionFraccionArancelaria { get; set; }*/

        public Boolean MaterialPeligro { get; set; }
        public c_Moneda Moneda { get; set; }

        [DisplayName("Peso en Kilogramos")]
        //[Required(ErrorMessage ="Campo Obligatorio")]
        public Decimal PesoEnKg { get; set; }
        public String Unidad { get; set; }

        [DisplayName("UUID del Comprobante de Comercio Exterior")]
        public String UUIDComecioExt { get; set; }

        [DisplayName("Valor de la Mercancía")]
        public String ValorMercancia { get; set; }

        public int? DetalleMercanciaId { get; set; }
        [ForeignKey("DetalleMercanciaId")]
        public virtual DetalleMercancia DetalleMercancia { get; set; }

        public int? Mercancias_Id { get; set; }
        [ForeignKey("Mercancias_Id")]
        public virtual Mercancias Mercancias { get; set; }

        [NotMapped]
        public virtual CantidadTransportada CantidadTransportada { get; set; }

        //[NotMapped]
        public virtual List<CantidadTransportada> CantidadTransportadass { get; set; }

        [NotMapped]
        public virtual GuiasIdentificacion GuiasIdentificacion { get; set; }

        //[NotMapped]
        public virtual List<GuiasIdentificacion> GuiasIdentificacionss { get; set; }

        [NotMapped]
        public virtual Pedimentos Pedimentos { get; set; }

        //[NotMapped]
        public virtual List<Pedimentos> Pedimentoss { get; set; }

        
    }
}
