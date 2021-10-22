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
    [Table("cp_rel_CarroContenedorC")]
    public class CarroContenedorC
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Carro_Id { get; set; }
        [ForeignKey("Carro_Id")]
        public virtual Carro Carro { get; set; }

        public int ContenedorC_Id { get; set; }
        [ForeignKey("ContenedorC_Id")]
        public virtual ContenedorC ContenedorC { get; set; }

    }
}
