using API.CatalogosCartaPorte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_ContenedorC")]
    public class ContenedorC
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Decimal PesoContenedorVacio { get; set; }
        public Decimal PesoNetoMercancia { get; set; }
        public String Contenedor_Id { get; set; }
        [ForeignKey("Contenedor_Id")]
        public virtual Contenedor Contenedor { get; set; }
      
        [NotMapped]
        public String TipoContenedor { get; set; }

    }
}
