using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte.Domicilio
{
    [Table("c_Colonia")]
    public class Colonia
    {
        [Key, Column(Order = 0)]
        [StringLength(4)]
        public String c_Colonia { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(5)]
        public String c_CodigoPostal_Id { get; set; }
        [ForeignKey ("c_CodigoPostal_Id")]
        public virtual CodigoPostal CodigoPostal { get; set; }
        public String Nombreasentamiento { get; set; }

    }
}
