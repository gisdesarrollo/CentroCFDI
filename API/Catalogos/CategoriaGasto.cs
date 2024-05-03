using API.Catalogos;
using API.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Catalogos
{
    [Table("CategoriasGastos")]
    public class CategoriaGasto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        [DisplayName("Nombre")]
        public string Name { get; set; }

        [DisplayName("Descripción")]    
        public string Descripcion { get; set; }

        [DisplayName("CuentaContable")]
        public string CuentaContable { get; set; }

    }
}
