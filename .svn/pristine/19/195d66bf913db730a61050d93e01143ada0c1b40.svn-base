using API.Catalogos;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Relaciones
{
    [Table("REL_BANCOSSUCURSALES")]
    public class BancoSucursal
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public String Nombre { get; set; }

        [DisplayName("Banco")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int BancoId { get; set; }
        [ForeignKey("BancoId")]
        public virtual Banco Banco { get; set; }

        [DisplayName("Sucursal")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public String NumeroCuenta { get; set; }
    }
}
