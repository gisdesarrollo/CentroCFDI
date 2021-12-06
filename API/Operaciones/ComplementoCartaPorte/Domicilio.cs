using CFDI.API.Enums.CFDI33;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_Domicilio")]
    public class Domicilio
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public String Calle { get; set; }

        [DisplayName("Codigo Postal")]
        public String CodigoPostal { get; set; }
        public String Colonia { get; set; }
        
        public String Estado { get; set; }
        public String Localidad { get; set; }
        public String Municipio { get; set; }

        [DisplayName("Número Exterior")]
        public String NumeroExterior { get; set; }

        [DisplayName("Número Interior")]
        public String NumeroInterior { get; set; }
        
        public String Pais { get; set; }
        public String Referencia { get; set; }

        [NotMapped]
        public string paiss { get; set; }
        [NotMapped]
        public string municipios { get; set; }
        [NotMapped]
        public string estados { get; set; }
        [NotMapped]
        public string localidades { get; set; }

        [NotMapped]
        public string colonias { get; set; }
    }
}
