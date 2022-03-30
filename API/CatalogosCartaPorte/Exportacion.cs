using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.CatalogosCartaPorte
{
    [Table("c_Exportacion")]
    public class Exportacion
    {
        [Key]
        [StringLength(2)]
        public String c_Exportacion { get; set; }
        public String Descripcion { get; set; }
    }
}
