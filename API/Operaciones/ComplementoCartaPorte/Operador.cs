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
    [Table("cp_Operador")]
    public class Operador
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Nombre del Operador")]
        public String NombreOperador { get; set; }

        [DisplayName("Número de Licencia")]
        public String NumLicencia { get; set; }

        [DisplayName("Número de Identificación o Registro Fiscal del Operador")]
        public String NumRegIdTribOperador { get; set; }

        [DisplayName("Residencia Fiscal del Operador")]
        public c_Pais ResidenciaFiscalOperador { get; set; }
        [NotMapped]
        public String Pais { get; set; }

        [DisplayName("RFC del Operador")]
        public String RFCOperador { get; set; }

        public int Domicilio_Id { get; set; }
        [ForeignKey("Domicilio_Id")]
        public virtual Domicilio Domicilio { get; set; }

    }
}
