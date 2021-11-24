using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.CatalogosCartaPorte
{
    [Table("cp_SubImpuestoConcepto")]
    public  class SubImpuestoConcepto
    {
        //Traslado o Retencion
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Decimal Base { get; set; }
        public String Impuesto { get; set; }

        [DisplayName("Tipo de factor")]
        public String TipoFactor { get; set; }

        [DisplayName("Tasa o cuota ")]
        public String TasaOCuota { get; set; }

        public Decimal Importe { get; set; }
    }
}
