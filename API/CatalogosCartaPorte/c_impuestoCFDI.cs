using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.CatalogosCartaPorte
{
    [Table("c_ImpuestoCP")]
    public class c_impuestoCFDI
    {
        [Key]
        [StringLength(3)]
        public String c_Impuesto { get; set; }

        public String Descripcion { get; set; }
    }
}
