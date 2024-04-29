using API.Catalogos;
using API.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Operaciones.OperacionesRecepcion
{
    [Table("ComprobacionesGastos")]
    public class ComprobacionGasto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        [DisplayName("Folio")]
        public int? Folio { get; set; }

        [DisplayName("Descripcion")]
        public string Descripcion { get; set; }

        [DataType(DataType.MultilineText)]
        [DisplayName("Comentarios")]
        public string Comentarios { get; set; }

        [DisplayName("Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }

        [DisplayName("Monto")]
        public double Monto { get; set; }

        [DisplayName("Moneda")]
        public c_Moneda? MonedaId { get; set; }

        [DisplayName("Usuario")]
        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [DisplayName("Departamento")]
        public int DepartamentoId { get; set; }
        [ForeignKey("DepartamentoId")]
        public virtual Departamento Departamento { get; set; }

        [DisplayName("Proyecto")]
        public int? ProyectoId { get; set; }
        [ForeignKey("ProyectoId")]
        public virtual Proyecto Proyecto { get; set; }

        [DisplayName("Evento")]
        public int? EventoId { get; set; }
        [ForeignKey("EventoId")]
        public virtual Evento Evento { get; set; }

        [DisplayName("Estatus")]
        public c_Estatus Estatus { get; set; }

    }
}
