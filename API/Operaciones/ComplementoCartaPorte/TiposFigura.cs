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
        [DisplayName("Figura Transporte")]
        public string FiguraTransporte { get; set; }
        [DisplayName("RFC Figura Transporte")]
        public string RFCFigura { get; set; }
        [DisplayName("Número de Licencia")]
        public string NumLicencia { get; set; }
        [DisplayName("Nombre de Figura")]
        public string NombreFigura { get; set; }

        [DisplayName("Registro fiscal de la figura de transporte")]
        public string NumRegIdTribFigura { get; set; }
        [DisplayName("Residencia fiscal de la figura de transporte")]
        public c_Pais? ResidenciaFiscalFigura { get; set; }

        
        public int? Domicilio_Id { get; set; }
        [ForeignKey("Domicilio_Id")]
        public virtual Domicilio Domicilio { get; set; }

        public int? Complemento_Id { get; set; }
        [ForeignKey("Complemento_Id")]
        public virtual ComplementoCartaPorte ComplementoCP { get; set; }

        [NotMapped]
        public virtual PartesTransporte PartesTransporte { get; set; }

       //[NotMapped]
        public virtual List<PartesTransporte> PartesTransportes { get; set; }

        

    }
}
