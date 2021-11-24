using CFDI.API.Enums.CFDI33;
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
    [Table("cp_Tiposfigura")]
    public class TiposFigura
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

       // [Required(ErrorMessage = "Campo Obligatorio")]
        public string FiguraTransporte { get; set; }
        [DisplayName("RFC Figura Transporte")]
        public string RFCFigura { get; set; }
        [DisplayName("Número de Licencia")]
        public string NumLicencia { get; set; }
        public string NombreFigura { get; set; }
        public string NumRegIdTribFigura { get; set; }
        [DisplayName("Clave País")]
        public c_Pais ResidenciaFiscalFigura { get; set; }

        [NotMapped]
        public virtual PartesTransporte PartesTransporte { get; set; }

        [NotMapped]
        public virtual List<PartesTransporte> PartesTransportes { get; set; }

        [NotMapped]
        public virtual List<PartesTransporte> PartesTransportes2 { get; set; }

    }
}
