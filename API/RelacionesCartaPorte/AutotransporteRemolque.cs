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
    [Table("cp_rel_AutotransporteRemolque")]
    public class AutotransporteRemolque
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Autotransporte_Id { get; set; }
        [ForeignKey("Autotransporte_Id")]
        public virtual AutoTransporte AutoTransporte { get; set; }

        public int Remolques_Id { get; set; }
        [ForeignKey("Remolques_Id")]
        public virtual Remolques Remolques{ get; set; }
    }
}
