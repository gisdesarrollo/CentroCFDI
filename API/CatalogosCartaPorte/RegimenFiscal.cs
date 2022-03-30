using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.CatalogosCartaPorte
{
    [Table("c_RegimenFiscal")]
    public class RegimenFiscal
    {
        [Key]
        public int c_RegimenFiscal { get; set; }
        public String Descripcion { get; set; }
    }
}
