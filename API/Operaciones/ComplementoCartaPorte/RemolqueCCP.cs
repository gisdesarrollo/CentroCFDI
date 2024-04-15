using API.CatalogosCartaPorte;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_remolqueccp")]
    public class RemolqueCCP
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DisplayName("Placa CCP")]
        public String PlacaCCP { get; set; }

        [DisplayName("Subtipo de Remolque CCP")]
        public String SubTipoRemCCP { get; set; }
        [ForeignKey("SubTipoRemCCP")]
        public virtual SubTipoRem SubTipoRem { get; set; }
        public int? TransporteMaritimo_Id { get; set; }
        [ForeignKey("TransporteMaritimo_Id")]
        public virtual TransporteMaritimo TransporteMaritimo { get; set; }

    }
}
