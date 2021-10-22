using API.Catalogos;
using API.Enums;
using API.Models.Facturas;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Operaciones.Facturacion
{
    [Table("ori_facturasrecibidas")]
    public class FacturaRecibida : Factura
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Emisor")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int EmisorId { get; set; }
        [ForeignKey("EmisorId")]
        public virtual Proveedor Emisor { get; set; }

        [DisplayName("Receptor")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int ReceptorId { get; set; }
        [ForeignKey("ReceptorId")]
        public virtual Sucursal Receptor { get; set; }

        [DisplayName("Tipo de Gasto")]
        public TiposGastos TipoGasto { get; set; }

        [DisplayName("Usuario")]
        public int? UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [DisplayName("Validación")]
        public int? ValidacionId { get; set; }
        [ForeignKey("ValidacionId")]
        public virtual Validacion Validacion { get; set; }

        #region Autoriazacion

        public bool? Autorizada { get; set; }

        [DisplayName("Fecha de Autorización")]
        public DateTime? FechaAutorizacion { get; set; }

        [DisplayName("Usuario de Autorización")]
        public int? UsuarioAutorizacionId { get; set; }
        [ForeignKey("UsuarioAutorizacionId")]
        public virtual Usuario UsuarioAutorizacion { get; set; }

        [DisplayName("Notas de Autorización")]
        public String NotasAutorizacion { get; set; }

        #endregion

    }
}
