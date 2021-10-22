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
    [Table("cp_rel_TransporteFerroviarioDerechosDePaso")]
    public class TransporteFerroviarioDerechosDePaso
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int TransporteFerroviario_Id { get; set; }
        [ForeignKey("TransporteFerroviario_Id")]
        public virtual TransporteFerroviario TransporteFerroviario { get; set; }

        public int DerechosDePaso_Id { get; set; }
        [ForeignKey("DerechosDePaso_Id")]
        public virtual DerechosDePasos DerechosDePasos { get; set; }
    }
}
