using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte.Domicilio
{
    [Table("c_CodigoPostal")]
    public class CodigoPostal
    {
        [Key]
        [StringLength(5)]
        public String c_CodigoPostal { get; set; }

        [StringLength(3)]
        public String c_Estado_Id { get; set; }
        
        [ForeignKey ("c_Estado_Id")]
        public virtual Estado Estado { get; set; }

        [StringLength(3)]
        public String c_Municipio_Id { get; set; }
        

        [StringLength(2)]
        public String c_Localidad_Id { get; set; }
        
        public String EstimuloFranjaFronteriza { get; set; }

    }
}
