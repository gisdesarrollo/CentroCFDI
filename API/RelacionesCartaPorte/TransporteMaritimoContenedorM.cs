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
    [Table("cp_rel_TransporteMaritimoContenedorM")]
    public class TransporteMaritimoContenedorM
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int TransporteMaritimo_Id { get; set; }
        [ForeignKey("TransporteMaritimo_Id")]
        public virtual TransporteMaritimo TransporteMaritimo { get; set; }

        public int ContenedorM_Id { get; set; }
        [ForeignKey("ContenedorM_Id")]
        public virtual ContenedorM ContenedorMM { get; set; }

    }
}
