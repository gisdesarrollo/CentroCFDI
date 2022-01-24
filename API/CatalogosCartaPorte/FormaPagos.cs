using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.CatalogosCartaPorte
{
    [Table("c_FormaPago")]
    public class FormaPagos
    {
        [Key]
        [StringLength(3)]
        public String c_FormaPago { get; set; }

        public String Descripcion { get; set; }

        
    }
}
