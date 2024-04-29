using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Catalogos;

namespace API.Relaciones
{
    [Table("rel_SocioComerciaSucursales")]
    public class ProveedorSucursal
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("SocioComercial")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int SocioComercialId { get; set; }
        [ForeignKey("SocioComercialId")]
        public virtual SocioComercial SocioComercial { get; set; }

        [DisplayName("Sucursal")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }
    }
}
