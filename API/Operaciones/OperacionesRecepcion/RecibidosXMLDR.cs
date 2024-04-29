using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Operaciones.OperacionesProveedores
{
    [Table("Recibidos_XML")]
    public class RecibidosXMLDR
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Archivo XML")]
        public byte[] Archivo { get; set; }
    }
}
