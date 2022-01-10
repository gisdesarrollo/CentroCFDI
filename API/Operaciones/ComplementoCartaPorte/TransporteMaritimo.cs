using API.CatalogosCartaPorte;
using API.Catalogos;
using System;
using System.Collections.Generic;
using CFDI.API.Enums.CFDI33;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_TransporteMaritimo")]
    public class TransporteMaritimo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Año de Embarcación")]
        public int AnioEmbarcacion { get; set; }
        public Decimal Calado { get; set; }
        public Decimal Eslora { get; set; }

        [DisplayName("Línea Naviera")]
        public String LineaNaviera { get; set; }
        public Decimal Manga { get; set; }

        [DisplayName("Matrícula")]
        //[Required(ErrorMessage = "Campo obligatorio")]
        public String Matricula { get; set; }

        [DisplayName("Nacionalidad de Embarcación")]
        //[Required(ErrorMessage = "Campo obligatorio")]
        public string  NacionalidadEmbarc { get; set; }

        [DisplayName("Nombre del Agente Naviero")]
        //[Required(ErrorMessage = "Campo obligatorio")]
        public String NombreAgenteNaviero { get; set; }

        [DisplayName("Nombre de la Aseguradora")]
        public String NombreAseg { get; set; }

        [DisplayName("Nombre de Embarcación")]
        public String NombreEmbarc { get; set; }
        [DisplayName("Numero de Autorización Naviero ")]
        //[Required(ErrorMessage = "Campo obligatorio")]
        public String NumAutorizacionNaviero_Id { get; set; }
        [ForeignKeyAttribute("NumAutorizacionNaviero_Id")]
        public virtual NumAutorizacionNaviero NumAutorizacionNaviero { get; set; }
        /*[NotMapped]
        public String NumAutorizacionNavieros { get; set; }
        */
        [DisplayName("Número de Certificación de la ITC")]
        //[Required(ErrorMessage = "Campo obligatorio")]
        public String NumCerITC { get; set; }

        [DisplayName("Número de Conocimiento del Embarque")]
        public String NumConocEmbarc { get; set; }

        [DisplayName("Número OMI")]
        //[Required(ErrorMessage = "Campo obligatorio")]
        public String NumeroOMI { get; set; }
        
        [DisplayName("Número de Permiso SCT")]
        public String NumPermisoSCT { get; set; }

        [DisplayName("Número de Poliza de Seguro")]
        public String NumPolizaSeguro { get; set; }

        [DisplayName("Número de Viaje")]
        public String NumViaje { get; set; }

        [DisplayName("Permiso SCT")]
        public String TipoPermiso_Id { get; set; }
        [ForeignKeyAttribute("TipoPermiso_Id")]
        public virtual TipoPermiso TipoPermiso { get; set; }
        /*[NotMapped]
        [DisplayName("Permiso SCT")]
        public String PermSCT { get; set; }
        */
        [NotMapped]
        [DisplayName("Descipción de Permiso")]
        public String DescripcionPermSCT { get; set; }


        [DisplayName("Tipo de Carga")]
        //[Required(ErrorMessage = "Campo obligatorio")]
        public String ClaveTipoCarga_Id { get; set; }
        [ForeignKeyAttribute("ClaveTipoCarga_Id")]
        public virtual ClaveTipoCarga ClaveTipoCarga { get; set; }

        /*[NotMapped]
        [DisplayName("Tipo de Carga")]
        public String TipoCarga { get; set; }
        */

        [DisplayName("Tipo de Embarcación")]
        //[Required(ErrorMessage ="Campo obligatorio")]
        public String ConfigMaritima_Id { get; set; }
        [ForeignKey("ConfigMaritima_Id")]
        public virtual ConfigMaritima ConfigMaritima{ get; set; }
        /*[NotMapped]
        [DisplayName("Tipo de Embarcación")]
        public String TipoEmbarcacion { get; set; }*/

        //[NotMapped]
        [DisplayName("Unidades de Arqueo Bruto")]
        //[Required(ErrorMessage = "Campo obligatorio")]
        public Decimal UnidadesDeArqBruto { get; set; }

        [NotMapped]
        public virtual ContenedorM ContenedorM { get; set; }

        //[NotMapped]
        public virtual List<ContenedorM> ContenedoresM { get; set; }

    }
}
