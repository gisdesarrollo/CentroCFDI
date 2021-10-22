using API.CatalogosCartaPorte;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Operaciones.ComplementoCartaPorte
{
    [Table("cp_FiguraTransporte")]
    public class FiguraTransporte
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public String CveTransporte_Id { get; set; }
        [ForeignKey("CveTransporte_Id")]
        public virtual CveTransporte CveTransporte { get; set; }
        [NotMapped]
        public String CveTransportes { get; set; }
        
        [NotMapped]
        public virtual Propietario Propietario { get; set; }
        
        [NotMapped]
        public virtual Arrendatario Arrendatario { get; set; }
        
        [NotMapped]
        public virtual Notificado Notificado { get; set; }

    }
}
