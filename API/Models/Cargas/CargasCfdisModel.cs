using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Cargas
{
    public class CargasCfdisModel : Archivos
    {
        [DisplayName("Departamento")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int DepartamentoId { get; set; }

        [DisplayName("Centro de Costos")]
        [Required(ErrorMessage = "Campo Obligatorio")]
        public int CentroCostoId { get; set; }

        [DisplayName("Orden de Compra")]
        public String Referencia1 { get; set; }

        [DisplayName("Pedido")]
        public String Referencia2 { get; set; }

        [DisplayName("Referencia")]
        public String Referencia3 { get; set; }

        [DisplayName("Nota")]
        public String Referencia4 { get; set; }

        [DisplayName("Nota 2")]
        public String Referencia5 { get; set; }

    }
}