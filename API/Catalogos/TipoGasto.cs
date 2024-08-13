using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Catalogos
{
    [Table("TipoGastos")]
    public class TipoGasto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string CuentaContable { get; set; }

        #region Sucursal

        [Required(ErrorMessage = "Campo Obligatorio")]
        [DisplayName("Sucursal")]
        public int SucursalId { get; set; }

        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        #endregion
    }
}
