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
    [Table("cp_rel_AutotransporteFederalRemolque")]
    public class AutotransporteFederalRemolque
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AutotransporteFederal_Id { get; set; }
        [ForeignKey("AutotransporteFederal_Id")]
        public virtual AutoTransporteFederal AutoTransporteFederal { get; set; }

        public int Remolques_Id { get; set; }
        [ForeignKey("Remolques_Id")]
        public virtual Remolques Remolques{ get; set; }
    }
}
