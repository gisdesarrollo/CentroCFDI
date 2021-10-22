using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_ClaveUnidad")]
    public class ClaveUnidad
    {
        [Key]
        [StringLength(12)]
        public String c_ClaveUnidad { get; set; }
        public String Nombre { get; set; }
        public String Descripcion { get; set; }
        public String Nota { get; set; }

        public String Simbolo { get; set; }

    }
}
