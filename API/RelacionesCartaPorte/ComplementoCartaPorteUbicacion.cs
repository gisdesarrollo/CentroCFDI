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
    [Table("cp_rel_ComplementoCartaPorteUbicacion")]
    public class ComplementoCartaPorteUbicacion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ComplementoCartaPorte_Id { get; set; }
        [ForeignKey("ComplementoCartaPorte_Id")]
        public virtual ComplementoCartaPorte ComplementoCartaPorte { get; set; }

        public int Ubicacion_Id { get; set; }
        [ForeignKey("Ubicacion_Id")]
        public virtual Ubicacion Ubicacion { get; set; }

    }
}
