using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_ClaveUnidadPeso")]
    public class ClaveUnidadPeso
    {
        [Key]
        [StringLength(3)]
        public String ClaveUnidad { get; set; }
        public String Nombre { get; set; }
        public String Descripcion { get; set; }
        public String Nota { get; set; }

        public String Simbolo { get; set; }

        public String Bandera { get; set; }
    }
}
