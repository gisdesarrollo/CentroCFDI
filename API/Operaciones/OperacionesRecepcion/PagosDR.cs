﻿using API.Catalogos;
using API.Enums;
using API.Models;
using API.Models.Dto;
using API.Relaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace API.Operaciones.OperacionesProveedores
{
    [Table("Pagos")]
    public class PagosDR
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SucursalId { get; set; }

        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        [DisplayName("Fecha Pago")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaPago { get; set; }

        public c_Moneda? Moneda { get; set; }

        [DisplayName("Tipo de Cambio")]
        public double? TipoCambio { get; set; }

        public Double Total { get; set; }

        public String ReferenciaBancaria { get; set; }

        public String ReferenciaERP { get; set; }

        public int? CuentaBancariaSucursal_Id { get; set; }

        [ForeignKey("CuentaBancariaSucursal_Id")]
        public virtual BancoSucursal BancoSucursal { get; set; }

        [NotMapped]
        public DocumentosPagadosDR DocumentoPagado { get; set; }

        public virtual List<DocumentosPagadosDR> DocumentosPagados { get; set; }

        //public int? ComplementoPagoRecibido_Id { get; set; }

        //[ForeignKey("ComplementoPagoRecibido_Id")]
        //public virtual DocumentoRecibido DocumentoRecibido { get; set; }
        public virtual List<DocumentoRecibido> DocumentosRecibidos { get; set; }
        public int? SocioComercialId { get; set; }
        [ForeignKey("SocioComercialId")]
        public virtual SocioComercial SocioComercial { get; set; }

        [NotMapped]
        public bool Procesado { get; set; }

        [NotMapped]
        public HttpPostedFileBase ArchivoXml { get; set; }

        [NotMapped]
        public ComplementoPagoDto ComplementoPago { get; set; }

        [NotMapped]
        public String Detalle_Validacion { get; set; }
    }
}