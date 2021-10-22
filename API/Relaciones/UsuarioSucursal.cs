using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Catalogos;

namespace API.Relaciones
{
    [Table("rel_usuariossucursales")]
    public class UsuarioSucursal
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Usuario")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [DisplayName("Sucursal")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }
    }
}
