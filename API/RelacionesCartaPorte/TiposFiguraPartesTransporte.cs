using API.Operaciones.ComplementoCartaPorte;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.RelacionesCartaPorte
{

    [Table("cp_rel_TiposFiguraPartesTransporte")]
    public class TiposFiguraPartesTransporte
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int TiposFigura_Id { get; set; }
        [ForeignKey("TiposFigura_Id")]
        public virtual TiposFigura TiposFigura { get; set; }

        public int PartesTransporte_Id { get; set; }
        [ForeignKey("PartesTransporte_Id")]
        public virtual PartesTransporte PartesTransporte { get; set; }
    }
}
