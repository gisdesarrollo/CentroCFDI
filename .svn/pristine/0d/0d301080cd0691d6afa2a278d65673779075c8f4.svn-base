using API.Catalogos;
using API.Enums;
using CFDI.API.Enums.CFDI33;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Operaciones.Facturacion
{
    [Table("ORI_DOCUMENTOSEXTRANJEROS")]
    public class DocumentoExtranjero
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Emisor")]
        public String NombreEmisor { get; set; }

        [DisplayName("Receptor")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int ReceptorId { get; set; }
        [ForeignKey("ReceptorId")]
        public virtual Sucursal Receptor { get; set; }

        [DisplayName("Usuario")]
        public int? UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; }

        [DisplayName("Nombre de Archivo")]
        public String NombreArchivo { get; set; }

        public byte[] Archivo { get; set; }

        #region Especificaciones

        [Required(ErrorMessage = "Campo Obligatorio")]
        public DateTime Fecha { get; set; }

        public String Concepto { get; set; }

        [DisplayName("Tipo de Gasto")]
        public TiposGastos TipoGasto { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public c_Moneda Moneda { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public Double TipoCambio { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public double Total { get; set; }

        #endregion

        #region Autoriazacion

        public bool? Autorizada { get; set; }

        [DisplayName("Fecha de Autorización")]
        public DateTime FechaAutorizacion { get; set; }

        [DisplayName("Usuario de Autorización")]
        public int? UsuarioAutorizacionId { get; set; }
        [ForeignKey("UsuarioAutorizacionId")]
        public virtual Usuario UsuarioAutorizacion { get; set; }

        [DisplayName("Notas de Autorización")]
        public String NotasAutorizacion { get; set; }

        #endregion
    }
}
