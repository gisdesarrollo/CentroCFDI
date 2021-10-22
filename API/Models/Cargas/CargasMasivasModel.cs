using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Cargas
{
    public class CargasMasivasModel
    {
        [DisplayName("Sucursal")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int SucursalId { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public bool Prepopulado { get; set; }
    }
}
