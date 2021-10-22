using API.Catalogos;
using API.Relaciones;
using CFDI.API.Enums.CFDI33;
using CFDI.API.Enums.Complementos.Pagos10;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace API.Operaciones.ComplementosPagos
{
    [Table("ORI_PAGOS")]
    public class Pago
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Fecha de Pago")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public DateTime FechaPago { get; set; }

        [DisplayName("Forma de Pago")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public c_FormaPago FormaPago { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public c_Moneda Moneda { get; set; }

        [DisplayName("Tipo de Cambio")]
        public double TipoCambio { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio")]
        public double Monto { get; set; }

        [DisplayName("Número de Operación")]
        public String NumeroOperacion { get; set; }

        [DisplayName("RFC Emisor de Cuenta Origen")]
        public String RfcEmisorCuentaOrigen { get; set; }

        [DisplayName("RFC Emisor de Cuenta Beneficiario")]
        public String RfcEmisorCuentaBeneficiario { get; set; }

        [DisplayName("Nombre del Banco Ordenante en el Extranjero")]
        public String NombreBancoOrdenanteExtranjero { get; set; }

        #region Spei

        [DisplayName("Tipo de Cadena de Pago")]
        public c_TipoCadenaPago? TipoCadenaPago { get; set; }

        [DisplayName("Certificado de Pago")]
        public String CertificadoPago { get; set; }

        [DisplayName("Cadena de Pago")]
        public String CadenaPago { get; set; }

        [DisplayName("Sello del Pago")]
        public String SelloPago { get; set; }

        [NotMapped]
        [DisplayName("Archivo")]
        public HttpPostedFileBase Archivo { get; set; }

        public String NombreArchivo { get; set; }

        [DisplayName("Archivo")]
        public byte[] ArchivoFisico { get; set; }

        #endregion

        public String Notas { get; set; }

        [DisplayName("Banco Ordenante")]
        public int? BancoOrdenanteId { get; set; }
        [ForeignKey("BancoOrdenanteId")]
        public virtual BancoCliente BancoOrdenante { get; set; }

        [DisplayName("Banco Beneficiario")]
        public int? BancoBeneficiarioId { get; set; }
        [ForeignKey("BancoBeneficiarioId")]
        public virtual BancoSucursal BancoBeneficiario { get; set; }

        //Objetos
        [DisplayName("Pago Container")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int ComplementoPagoId { get; set; }
        [ForeignKey("ComplementoPagoId")]
        public virtual ComplementoPago ComplementoPago { get; set; }

        [DisplayName("Sucursal")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        //Listas
        [NotMapped]
        public DocumentoRelacionado DocumentoRelacionado { get; set; }
        public virtual List<DocumentoRelacionado> DocumentosRelacionados { get; set; }

        public virtual List<Impuesto> Impuestos { get; set; }

        //Desplegado
        [NotMapped]
        public String Desplegado { get { return String.Format("Pago: {0:dd/MM/yyyy} - {1:c}", FechaPago, Monto); } }
    }
}
