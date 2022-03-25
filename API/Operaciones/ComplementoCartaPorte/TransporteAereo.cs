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
using API.Enums.CartaPorteEnums;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_TransporteAereo")]
    public class TransporteAereo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(5)]
        [DisplayName("Codigo de Transportista")]
        //[Required(ErrorMessage ="Campo Obligatorio")]
        public String CodigoTransporteAereo_Id { get; set; }
        [ForeignKey("CodigoTransporteAereo_Id")]
        public virtual CodigoTransporteAereo CodigoTransporteAereo{ get; set; }
        /*[NotMapped]
        [DisplayName("Codigo de Transportista")]
        public String CodigoTransportista { get; set; }
        */
        [DisplayName("Lugar de Contrato")]
        public String LugarContrato { get; set; }

        [DisplayName("Matrícula de Aeronave")]
        public String MatriculaAereonave { get; set; }

        [DisplayName("Nombre de la Aseguradora")]
        public String NombreAseg { get; set; }

        [DisplayName("Nombre del Embarcador")]
        public String NombreEmbarcador { get; set; }

        /*[DisplayName("Nombre del Tranportista")]
        public String NombreTransportista { get; set; }*/

        [DisplayName("Número de Guía")]
        //[Required(ErrorMessage = "Campo Obligatorio")]
        public String NumeroGuia { get; set; }

        [DisplayName("Número de Permiso SCT")]
        //[Required(ErrorMessage = "Campo Obligatorio")]
        public String NumPermisoSCT { get; set; }

        [DisplayName("Número de Poliza de Seguro")]
        public String NumPolizaSeguro { get; set; }

        [DisplayName("Número de Identificación o Registro Fiscal del Embarcador ")]
        public String NumRegIdTribEmbarc { get; set; }

        /*[DisplayName("Número de Identificación o Registro Fiscal del Transportista")]
        public String NumRegIdTribTranspor { get; set; }*/

        /*[NotMapped]
        [DisplayName("País del Transportista")]
        public String PaisT { get; set; }*/

        [NotMapped]
        [DisplayName("País del Embarcador")]
        public String PaisE { get; set; }

        [DisplayName("Permiso SCT")]
        //[Required(ErrorMessage ="Campo Obligatorio")]
        public String TipoPermiso_Id { get; set; }
        [ForeignKey("TipoPermiso_Id")]
        public virtual TipoPermiso TipoPermiso { get; set; }
        
        /*[NotMapped]
        [DisplayName("Permiso SCT")]
        public String PermSCT { get; set; }
        */
        [NotMapped]
        [DisplayName("Descripción del Permiso")]
        public String DescripcionPermSCT { get; set; }

        [DisplayName("Residencia Fiscal del Embarcador")]
        public c_PaisCP ResidenciaFiscalEmbarc { get; set; }

        /*[DisplayName("Residencia Fiscal del Transportista")]
        public c_Pais ResidenciaFiscalTranspor { get; set; }*/

        [DisplayName("RFC del Embarcador")]
        public String RFCEmbarcador { get; set; }

        /*[DisplayName("RFC de Transportista")]
        public String RFCTransportista { get; set; }*/

    }
}
