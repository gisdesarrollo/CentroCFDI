using API.CatalogosCartaPorte;
using API.Catalogos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_DetalleMercancia")]
    public class DetalleMercancia
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Número de Piezas")]
        public int NumPiezas { get; set; }

        [DisplayName("Peso Bruto")]
        //[Required(ErrorMessage = "Campo Obligatorio")]
        public Decimal PesoBruto  { get; set; }

        [DisplayName("Peso Neto")]
        //[Required(ErrorMessage = "Campo Obligatorio")]
        public Decimal PesoNeto { get; set; }

        [DisplayName("Peso Tara")]
        //[Required(ErrorMessage = "Campo Obligatorio")]
        public Decimal PesoTara { get; set; }

        [DisplayName("Unidad de Peso")]
        //[Required(ErrorMessage ="Campo Obligatorio")]
        public String ClaveUnidadPeso_Id { get; set; }
        [ForeignKey("ClaveUnidadPeso_Id")]
        public virtual ClaveUnidadPeso ClaveUnidadPeso { get; set; }
        [NotMapped]
        [DisplayName("Unidad de Peso")]
        public string UnidadPeso { get; set; }

    }
}
