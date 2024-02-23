using API.Catalogos;
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
    [Table("AprobacionesDR")]
    public class AprobacionesDR
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Usuario Entrega")]
        public int? UsuarioEntrega_Id { get; set; }

        [DisplayName("Usuario Solicitante")]
        public int? UsuarioSolicitante_Id { get; set; }

        [DisplayName("Departamento Usuario Solicitante")]
        public int? DepartamentoUsuarioSolicitante_Id { get; set; }

        [DisplayName("Fecha Solicitud")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaSolicitud { get; set; }

        [DisplayName("Usuario Aprobación Comercial")]
        public int? UsuarioAprobacionComercial_id { get; set; }

        [DisplayName("Fecha Aprobación Comercial")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaAprobacionComercial { get; set; }

        [DisplayName("Usuario Aprobación Pagos")]
        public int? UsuarioAprobacionPagos_id { get; set; }

        [DisplayName("Fecha Aprobación Pagos")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaAprobacionPagos { get; set; }

        [DisplayName("Usuario Rechazo")]
        public int? UsuarioRechazo_id { get; set; }

        [DisplayName("Fecha Rechazo")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaRechazo { get; set; }

        [DisplayName("Detalle Rechazo")]
        public String Detalle { get; set; }


        //eliminar este campo mas tarde
        [DisplayName("Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Fecha { get; set; }

    }
}
