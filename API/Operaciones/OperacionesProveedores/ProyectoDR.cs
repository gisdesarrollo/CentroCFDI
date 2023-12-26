using API.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Operaciones.OperacionesProveedores
{
    [Table("Proyectos")]
    public class ProyectoDR
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public String Nombre { get; set; }

        public String Detalle { get; set; }

        public c_EstatusSolicitudes? Estatus { get; set; }
    }
}
