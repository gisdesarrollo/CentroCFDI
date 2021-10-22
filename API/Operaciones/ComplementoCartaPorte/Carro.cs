using API.CatalogosCartaPorte;
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
    [Table("cp_Carro")]
    public class Carro
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Guías de Carro")]
        public String GuiaCarro { get; set; }

        [DisplayName("Matrícula de Carro")]
        public String MatriculaCarro { get; set; }
        public String TipoCarro_Id { get; set; }
        [ForeignKey("TipoCarro_Id")]
        public virtual TipoCarro TipoCarro { get; set; }
        [NotMapped]
        [DisplayName("Tipo de Carro")]
        public String TipoCarros { get; set; }

        [DisplayName("Toneladas Netas de Carro")]
        public Decimal ToneladasNetasCarro { get; set; }

        [NotMapped]
        public virtual ContenedorC ContenedorC { get; set; }
    }
}
