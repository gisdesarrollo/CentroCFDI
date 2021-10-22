using API.CatalogosCartaPorte;
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
    [Table("cp_Propietario")]
    public class Propietario
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Nombre del Propietario")]
        public String NombrePropietario { get; set; }

        [DisplayName("Número de Identificación o Registro Fiscal del Propietario")]
        public String NumRegIdTribPropietario { get; set; }

        [DisplayName("Residencia Fiscal del Propietario")]
        public c_Pais ResidenciaFiscalPropietario { get; set; }

        [NotMapped]
        [DisplayName("País")]
        public String Pais { get; set; }

        [DisplayName("RFC del Propietario")]
        public String RFCPropietario { get; set; }

        public int Domicilio_Id { get; set; }
        [ForeignKey("Domicilio_Id")]
        public virtual Domicilio Domicilio { get; set; }

    }
}
