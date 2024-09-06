using API.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Catalogos;
using API.Operaciones.OperacionesProveedores;

namespace API.Integraciones.Clientes
{
    [Table("CI_CofcoFacturasRecibidasReferencias")]
    public class Custom_Cofco_FacturasRecibidas_Referencias
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int SucursalId { get; set; }
        [ForeignKey("SucursalId")]
        public virtual Sucursal Sucursal { get; set; }

        public int? SocioComercialId { get; set; }
        [ForeignKey("SocioComercialId")]
        public virtual SocioComercial SocioComercial { get; set; }

        [DisplayName("Precio del contrato")]
        public decimal PrecioContrato { get; set; }

        [DisplayName("Moneda")]
        public c_Moneda Moneda { get; set; }

        [DisplayName("Tipo de cambio")]
        public decimal TipoCambio { get; set; }

        public decimal PesoOrigen { get; set; }

        public decimal PesoDestino { get; set; }

        public decimal KgMermaExcedente { get; set; }

    }

}