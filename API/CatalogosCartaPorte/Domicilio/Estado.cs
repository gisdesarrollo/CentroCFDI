using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte.Domicilio
{
    [Table("c_Estado")]
    public class Estado
    {
        [Key]
        [StringLength(3)]
        public String c_Estado { get; set; }

        [StringLength(3)]
        public String c_Pais_Id { get; set; }
        [ForeignKey("c_Pais_Id")]
        public virtual Pais c_Pais { get; set; }

        public String NombreEstado { get; set; }

    }
}
