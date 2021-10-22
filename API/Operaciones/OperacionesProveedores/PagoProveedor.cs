using API.Catalogos;
using API.Operaciones.Facturacion;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Operaciones.OperacionesProveedores
{
    [Table("ori_pagosproveedores")]
    public class PagoProveedor
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        [DisplayName("Usuario")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [DisplayName("Proveedor")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int ProveedorId { get; set; }
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }

        [DisplayName("Factura Recibida")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int FacturaRecibidaId { get; set; }
        [ForeignKey("FacturaRecibidaId")]
        public virtual FacturaRecibida FacturaRecibida { get; set; }
    }
}
