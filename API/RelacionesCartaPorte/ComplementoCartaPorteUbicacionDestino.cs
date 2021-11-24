using API.Operaciones.ComplementoCartaPorte;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.RelacionesCartaPorte
{
    [Table("cp_rel_ComplementoCartaPorteUbicacionDestino")]
    public class ComplementoCartaPorteUbicacionDestino
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ComplementoCartaPorte_Id { get; set; }
        [ForeignKey("ComplementoCartaPorte_Id")]
        public virtual ComplementoCartaPorte ComplementoCartaPorte { get; set; }

        public int UbicacionD_Id { get; set; }
        [ForeignKey("UbicacionD_Id")]
        public virtual UbicacionDestino UbicacionDestino { get; set; }
    }
}
