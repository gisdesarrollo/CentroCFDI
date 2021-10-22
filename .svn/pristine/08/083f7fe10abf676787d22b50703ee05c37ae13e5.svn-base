using API.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Catalogos
{
    [Table("CAT_GRUPOS")]
    public class Grupo
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid Llave { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public String Nombre { get; set; }

        public Status Status { get; set; }

        [DisplayName("Obtención por FTP")]
        public bool ObtencionFtp { get; set; }

        [DisplayName("Carga de PDF")]
        public bool CargaPdf { get; set; }

        [DisplayName("Autorización Automática de Viajes")]
        public bool AutorizacionAutomaticaViajes { get; set; }

        [DisplayName("Autorización Automática de Solicitudes")]
        public bool AutorizacionAutomaticaSolicitudes { get; set; }

        [DisplayName("Gastos de Viajes")]
        public bool GastosViajes { get; set; }

        [DisplayName("Gastos de Proveedores")]
        public bool GastosProveedores { get; set; }

        [DisplayName("Complementos de Pagos")]
        public bool ComplementosPagos { get; set; }

        public String Notas { get; set; }
        
    }
}
