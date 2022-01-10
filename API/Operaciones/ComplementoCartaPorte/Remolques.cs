using API.CatalogosCartaPorte;
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
    [Table("cp_Remolque")]
    public class Remolques
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        //[Required(ErrorMessage = "Campo Obligatorio")]
        public String Placa { get; set; }

        //[Required(ErrorMessage ="Campo Obligatorio")]
        [DisplayName("Subtipo de Remolque")]
        public String SubTipoRem_Id { get; set; }
        [ForeignKey("SubTipoRem_Id")]
        public virtual SubTipoRem SubTipoRem { get; set; }
        public int? AutoTransporte_Id { get; set; }
        [ForeignKey("AutoTransporte_Id")]
        public virtual AutoTransporte AutoTransporte { get; set; }

        [NotMapped]
        [DisplayName("Subtipo de Remolque")]
        public String SubTipoRems { get; set; }
    }
}
