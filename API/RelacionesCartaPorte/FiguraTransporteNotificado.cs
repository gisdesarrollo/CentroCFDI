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
    [Table("cp_rel_FiguraTransporteNotificado")]
    public class FiguraTransporteNotificado
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int FiguraTransporte_Id { get; set; }
        [ForeignKey("FiguraTransporte_Id")]
        public virtual FiguraTransporte FiguraTransporte { get; set; }

        public int Notificado_Id { get; set; }
        [ForeignKey("Notificado_Id")]
        public virtual Notificado Notificado { get; set; }

    }
}
