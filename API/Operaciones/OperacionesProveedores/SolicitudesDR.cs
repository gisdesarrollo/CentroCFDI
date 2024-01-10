using API.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Operaciones.OperacionesProveedores
{
    [Table("Solicitudes")]
    public class SolicitudesDR
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public String Nombre { get; set; }

        [DisplayName("Fecha Inicio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaInicio { get; set; }

        [DisplayName("Fecha Final")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaFinal { get; set; }

        public c_EstatusSolicitudes? Estatus { get; set; }

        public int? Solicitante_Id { get; set; }

        public int? Proyecto_Id { get; set; }
        [ForeignKey("Proyecto_Id")]
        public virtual ProyectoDR ProyectoDr { get; set; }

        
    }
}
