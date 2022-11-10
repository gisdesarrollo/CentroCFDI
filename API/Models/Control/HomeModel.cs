using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace API.Models.Control
{

    public class HomeModel
    {

        [DisplayName("Clientes Totales")]
        public int Clientes { get; set; }

        [DisplayName("CFDIs Emitidos")]
        public int CfdiEmitidos { get; set; }

        [DisplayName("Suma Total CFDIs Emitidos")]
        public int TotalCfdiEmitidos { get; set; }

        [DisplayName("Complementos de Pago Emitidos")]
        public int ComplementosPagoEmitidos { get; set; }

        [DisplayName("Complementos de Carta Porte Pendientes")]
        public int ComplementoCartaPortePendientes { get; set; }

        [DisplayName("Complementos de Pago Pendientes")]
        public int ComplementosPagoPendientes { get; set; }

        [DisplayName("Complementos Cfdi Pendientes")]
        public int ComprobanteCfdiPendientes { get; set; }

        [DisplayName("CTotal")]
        public int Total { get; set; }

        [DisplayName("Monto CFDI Total")]
        public int MontoCFDI { get; set; }


        //Monto de Meses anteriores asta la fecha actual

        public int Mes1total { get; set; }


        [DisplayName("Monto CFDI Total del Mes Segundo")]
        public int Mes2total { get; set; }

        [DisplayName("Monto CFDI Total del Mes Tercero ")]
        public int Mes3total { get; set; }

        [DisplayName("Monto CFDI Total del Mes Cuarto")]
        public int Mes4total { get; set; }

        [DisplayName("Monto CFDI Total del Mes Quinto ")]
        public int Mes5total { get; set; }

        [DisplayName("Monto CFDI Total del Mes Sexto")]
        public int Mes6total { get; set; }


        
        //Nombres de Meses Anteriores asta la fecha actual
        public string Mes1 { get; set; }

        public string Mes2 { get; set; }

        public string Mes3 { get; set; }

        public string Mes4 { get; set; }

        public string Mes5 { get; set; }

        public string Mes6 { get; set; }




    }


   

    





}
