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
    [Table("cp_rel_TransporteFerroviarioCarro")]
    public class TransporteFerroviarioCarro
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int TransporteFerroviario_Id { get; set; }
        [ForeignKey("TransporteFerroviario_Id")]
        public virtual TransporteFerroviario TransporteFerroviario { get; set; }

        public int Carro_Id { get; set; }
        [ForeignKey("Carro_Id")]
        public virtual Carro Carro { get; set; }
    }
}
