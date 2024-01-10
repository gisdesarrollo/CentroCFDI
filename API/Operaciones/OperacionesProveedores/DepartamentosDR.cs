using API.Catalogos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Operaciones.OperacionesProveedores
{
    [Table("Departamentos")]
    public class DepartamentosDR
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public String Nombre { get; set; }

        public String Clave { get; set; }

        public int Sucursal_Id { get; set; }
        [ForeignKey("Sucursal_Id")]
        public virtual Sucursal Sucursal { get; set; }
    }
}
