using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("c_UsoCFDI")]
    public  class UsoCfdi
    {
        [Key]
        [StringLength(3)]
        public String C_UsoCfdi { get; set; }

        public String descripcion { get; set; }

        public String PFisica { get; set; }

        public String PMoral { get; set; }
    
       // public DateTime FInicialVigencia { get; set; }

       // public DateTime FFinalVigencia { get; set; }


    }
}
