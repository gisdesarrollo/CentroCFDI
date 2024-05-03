using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Operaciones.OperacionesProveedores
{
    [Table("Adjuntos")]
    public class AdjuntosDR
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? DocumentosRecibidos_Id { get; set; }
        [ForeignKey("DocumentosRecibidos_Id")]
        public virtual DocumentosRecibidosDR DocumentoRecibido { get; set; }

        public byte[] Adjunto { get; set; }

        public String Referencia { get; set; }

        [DisplayName("Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
    }
}
