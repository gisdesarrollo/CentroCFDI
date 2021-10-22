using API.Operaciones.ComplementoCartaPorte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.RelacionesCartaPorte
{
    [Table("cp_rel_FiguraTransporteArrendatario")]
    public class FiguraTransporteArrendatario
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int FiguraTransporte_Id { get; set; }
        [ForeignKey("FiguraTransporte_Id")]
        public virtual FiguraTransporte FiguraTransporte { get; set; }

        public int Arrendatario_Id { get; set; }
        [ForeignKey("Arrendatario_Id")]
        public virtual Arrendatario Arrendatario { get; set; }

    }
}
