using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.CatalogosCartaPorte.Domicilio
{
    [Table("c_Pais")]
    public class Pais
    {
        [Key]
        [StringLength(3)]
        public String c_Pais { get; set; }

        public String Descripcion { get; set; }

        public String FormatoCodigoPostal { get; set; }

        public String FormatoRegistroIdentidadTributaria { get; set; }
        public String ValidacionRegistroIdentidadTributaria { get; set; }

        public String Agrupaciones { get; set; }

    }
}
