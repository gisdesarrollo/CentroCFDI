using API.Operaciones.ComplementoCartaPorte;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Operaciones.ComplementosPagos
{
    [Table("ori_trasladodocrel")]
    public class TrasladoDR : SubImpuestoC
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }
}
