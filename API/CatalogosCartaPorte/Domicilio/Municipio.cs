using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte.Domicilio
{
    [Table("c_Municipio")]
    public class Municipio
    {
        [Key, Column(Order = 0)]
        [StringLength(3)]
        public String c_Municipio { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(3)]
        public String c_Estado_Id { get; set; }
        [ForeignKey ("c_Estado_Id")]
        public virtual Estado Estado { get; set; }
        public String Descripcion { get; set; }

    }
}
