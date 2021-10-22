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
    [Table("cp_rel_FiguraTransportePropietario")]
    public class FiguraTransportePropietario
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int FiguraTransporte_Id { get; set; }
        [ForeignKey("FiguraTransporte_Id")]
        public virtual FiguraTransporte FiguraTransporte { get; set; }

        public int Propietario_Id { get; set; }
        [ForeignKey("Propietario_Id")]
        public virtual Propietario Propietario { get; set; }

    }
}
