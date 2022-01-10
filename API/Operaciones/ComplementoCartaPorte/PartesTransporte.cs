using API.Enums.CartaPorteEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_PartesTransporte")]
    public class PartesTransporte
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Parte del Transporte")]
        public c_ParteTransporte ParteTransporte { get; set; }
       
        public int? TiposFigura_Id { get; set; }
        [ForeignKey("TiposFigura_Id")]
        public virtual TiposFigura TiposFigura { get; set; }
       

    }
}
