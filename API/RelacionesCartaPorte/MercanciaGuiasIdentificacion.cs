using API.Operaciones.ComplementoCartaPorte;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.RelacionesCartaPorte
{
    [Table("cp_rel_MercanciaGuiasIdentificacion")]
    public class MercanciaGuiasIdentificacion
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Mercancia_Id { get; set; }
        [ForeignKey("Mercancia_Id")]
        public virtual Mercancia Mercancia { get; set; }

        public int GuiasIdentificacion_Id { get; set; }
        [ForeignKey("GuiasIdentificacion_Id")]
        public virtual GuiasIdentificacion GuiasIdentificacion { get; set; }
    }
}
