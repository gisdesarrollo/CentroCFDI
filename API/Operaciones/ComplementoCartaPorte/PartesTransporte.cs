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
        public String ParteTransporte { get; set; }
       
        public int? Domicilio_Id { get; set; }
        [ForeignKey("Domicilio_Id")]
        public virtual Domicilio Domicilio { get; set; }
       

    }
}
