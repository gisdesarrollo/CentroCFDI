using API.CatalogosCartaPorte;
using API.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using API.Enums;
using API.Enums.CartaPorteEnums;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_mercancia")]
    public class Mercancia
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(8)]
        [DisplayName("Clave Bienes Transportados")]
        public String ClaveProdServCP { get; set; }
        [NotMapped]
        public string NameBienesTransp { get; set; }

        public int Cantidad { get; set; }

        [DisplayName("Clave STCC")]
        public string ClaveProdSTCC { get; set; }
        [NotMapped]
        public string NameClaveSTCC { get; set; }

        [DisplayName("Clave de Unidad")]
        public string ClaveUnidad_Id { get; set; }
        [ForeignKey("ClaveUnidad_Id")]
        public virtual ClaveUnidad ClaveUnidad { get; set; }

        [DisplayName("Clave de Unidad")]
        public string ClavesUnidad { get; set; }

        [DisplayName("Clave de Material Peligroso")]
        public string MaterialPeligroso_Id { get; set; }
        [ForeignKey("MaterialPeligroso_Id")]
        public virtual MaterialPeligroso MaterialPeligroso { get; set; }

        [DisplayName("Clave de Material Peligroso")]
        public string ClaveMaterialPeligroso { get; set; }

        [DisplayName("Material Peligroso")]
        public Boolean MaterialPeligrosos { get; set; }
        
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }

        public string Dimensiones { get; set; }

        [DisplayName("Clave Embalaje")]
        public string TipoEmbalaje_Id { get; set; }
        [ForeignKey("TipoEmbalaje_Id")]
        public virtual TipoEmbalaje TipoEmbalaje { get; set; }
        
        
        [DisplayName("Tipo Embalaje")]
        public string DescripEmbalaje { get; set; }

        [DisplayName("Fracción Arancelaria")]
        public string FraccionArancelaria_Id { get; set; }
        [ForeignKey("FraccionArancelaria_Id")]
        public virtual FraccionArancelaria FraccionArancelaria { get; set; }
        
        [DisplayName("Clave Fracción Arancelaria")]
        public string  FraccionArancelarias { get; set; }
        
        public Boolean MaterialPeligro { get; set; }
        public c_Moneda Moneda { get; set; }

        [DisplayName("Peso en Kilogramos")]
        public Decimal PesoEnKg { get; set; }
        public String Unidad { get; set; }

        [DisplayName("UUID del Comprobante de Comercio Exterior")]
        public String UUIDComecioExt { get; set; }

        [DisplayName("Valor de la Mercancía")]
        public String ValorMercancia { get; set; }

        //campos nuevos version 3.0 CartaPorte
        [DisplayName("Sector Cofepris")]
        public c_SectorCofepris? SectorCofepris { get; set; }
        [StringLength(1000, ErrorMessage = "La longitud del Registro Nombre Ingrediente Activo no puede exceder de 1000 caracteres.")]
        [DisplayName("Nombre Ingrediente Activo")]
        public String NombreIngredienteActivo { get; set; }
        [StringLength(150, ErrorMessage = "La longitud del Registro Nombre del Quimico no puede exceder de 150 caracteres.")]
        [DisplayName("Nombre del Quimico")]
        public String NomQuimico { get; set; }
        [StringLength(50, ErrorMessage = "La longitud del Registro Denominacion Generica del Producto no puede exceder de 50 caracteres.")]
        [DisplayName("Denominacion Generica del Producto")]
        public String DenominacionGenericaProd { get; set; }
        [StringLength(50, ErrorMessage = "La longitud del Registro Denominacion Distintiva del Producto no puede exceder de 50 caracteres.")]
        [DisplayName("Denominacion Distintiva del Producto")]
        public String DenominacionDistintivaProd { get; set; }
        [StringLength(240, ErrorMessage = "La longitud del Registro Fabricante no puede exceder de 240 caracteres.")]
        public String Fabricante { get; set; }
        [DisplayName("Fecha Caducidad")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaCaducidad { get; set; }
        [StringLength(10, ErrorMessage = "La longitud del Registro Lote del Medicamento no puede exceder de 10 caracteres.")]
        [DisplayName("Lote del Medicamento")]
        public String LoteMedicamento { get; set; }
        [DisplayName("Forma Farmaceutica")]
        public c_FormaFarmaceutica? FormaFarmaceutica { get; set; }
        [DisplayName("Condiciones Especiales")]
        public c_CondicionesEspeciales? CondicionesEspecialesTransp { get; set; }
        [StringLength(15, ErrorMessage = "La longitud del Registro Sanitario Folio Autorización no puede exceder de 15 caracteres.")]
        public String RegistroSanitarioFolioAutorizacion { get; set; }
        [StringLength(10, ErrorMessage = "La longitud del Registro Permiso Importacion no puede exceder de 10 caracteres.")]
        public String PermisoImportacion { get; set; }
        [StringLength(25, ErrorMessage = "La longitud del Registro FolioImpoVucem no puede exceder de 25 caracteres.")]
        public String FolioImpoVucem { get; set; }
        [StringLength(15, ErrorMessage = "La longitud del Registro NumCas no puede exceder de 15 caracteres.")]
        public String NumCas { get; set; }
        [StringLength(80, ErrorMessage = "La longitud del Registro RazonSocialEmpImp no puede exceder de 80 caracteres.")]
        public String RazonSocialEmpImp { get; set; }
        [StringLength(60, ErrorMessage = "La longitud del Registro NumRegSanPlagCofepris no puede exceder de 60 caracteres.")]
        public String NumRegSanPlagCofepris { get; set; }
        [StringLength(600, ErrorMessage = "La longitud del Registro DatosFabricante no puede exceder de 600 caracteres.")]
        public String DatosFabricante { get; set; }
        [StringLength(600, ErrorMessage = "La longitud del Registro DatosFormulador no puede exceder de 600 caracteres.")]
        public String DatosFormulador { get; set; }
        [StringLength(600, ErrorMessage = "La longitud del Registro DatosMaquilador no puede exceder de 600 caracteres.")]
        public String DatosMaquilador { get; set; }
        [StringLength(1000, ErrorMessage = "La longitud del Registro UsoAutorizado no puede exceder de 1000 caracteres.")]
        public String UsoAutorizado { get; set; }
        public c_TipoMateria? TipoMateria { get; set; }
        [StringLength(50, ErrorMessage = "La longitud del Registro DescripcionMateria no puede exceder de 50 caracteres.")]
        public String DescripcionMateria { get; set; }
        [NotMapped]
        public virtual DocumentacionAduanera DocumentacionAduanera { get; set; }
        public virtual List<DocumentacionAduanera> DocumentacionAduaneras { get; set; }


        //////////////////
        public int? DetalleMercanciaId { get; set; }
        [ForeignKey("DetalleMercanciaId")]
        public virtual DetalleMercancia DetalleMercancia { get; set; }

        public int? Mercancias_Id { get; set; }
        [ForeignKey("Mercancias_Id")]
        public virtual Mercancias Mercancias { get; set; }


        /*Para Establecer el catalogo que se va a utilizar*/

        [NotMapped]
        [DisplayName("Catalogo Mercancia")]
        public string CatMercancia { get; set; }
        //


        [NotMapped]
        public virtual CantidadTransportada CantidadTransportada { get; set; }

        //[NotMapped]
        public virtual List<CantidadTransportada> CantidadTransportadass { get; set; }

        [NotMapped]
        public virtual GuiasIdentificacion GuiasIdentificacion { get; set; }

        //[NotMapped]
        public virtual List<GuiasIdentificacion> GuiasIdentificacionss { get; set; }
        
    }
}
