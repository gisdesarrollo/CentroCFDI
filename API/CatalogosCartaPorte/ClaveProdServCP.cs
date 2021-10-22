using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte
{
    [Table("c_ClaveProdServCP")]
    public class ClaveProdServCP
    {
        [Key]
        [StringLength(8)]
        public String c_ClaveUnidad { get; set; }
        public String Descripcion { get; set; }
        public String PalabrasSimilares { get; set; }

        public String MaterialPeligroso { get; set; }

    }
}
