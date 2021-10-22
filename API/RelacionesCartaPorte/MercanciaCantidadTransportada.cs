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
    [Table("cp_rel_MercanciaCantidadTransportada")]
    public class MercanciaCantidadTransportada
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Mercancia_Id { get; set; }
        [ForeignKey("Mercancia_Id")]
        public virtual Mercancia Mercancia { get; set; }

        public int CantidadTransportada_Id { get; set; }
        [ForeignKey("CantidadTransportada_Id")]
        public virtual CantidadTransportada CantidadTransportada { get; set; }
    }
}
