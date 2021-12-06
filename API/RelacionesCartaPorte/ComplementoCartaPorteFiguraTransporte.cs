using API.Operaciones.ComplementoCartaPorte;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.RelacionesCartaPorte
{
    [Table("cp_rel_ComplementoCartaPorteFiguraTransporte")]
    public class ComplementoCartaPorteFiguraTransporte
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ComplementoCartaPorte_Id { get; set; }
        [ForeignKey("ComplementoCartaPorte_Id")]
        public virtual ComplementoCartaPorte ComplementoCartaPorte { get; set; }

        public int FiguraTransporte_Id { get; set; }
        [ForeignKey("FiguraTransporte_Id")]
        public virtual TiposFigura FiguraTransporte { get; set; }
    }
}
