using System;
using System.Collections.Generic;
using System.ComponentModel;
using API.Operaciones;
using API.Operaciones.Expedientes;

namespace API.Models.Control
{

    public class HomeModel
    {
        //Datos para home de recepción de documentos Usuario y Cuentas por pagar

        public int DocumentosRecibidosEnRevisionUsuario { get; set; }
                   
        public int DocumentosRecibidosDepartamentoUsuario { get; set; }

        public int DocumentosPagosRevisionUsuario { get; set; }

        public int DocumentosPagosAprobadoUsuario { get; set; }

        public int PagosEnRevisionUsuario { get; set; }

        public double PorcentajeAprobacionUsuario { get; set; }

        public int DocumentosRecibidos30DiasUsuario { get; set; }

        //Datos para home de recepción de documentos Socios Comerciales

        public int DocumentosRecibidosEnRevisionUsuarioSC { get; set; }

        public int DocumentosRecibidosEnRevisionSC { get; set; }
                   
        public int DocumentosPagosRevisionSC { get; set; }
                   
        public int DocumentosPagosAprobadosSC { get; set; }
                   
        public int DocumentosRechazadosSC { get; set; }
        
        public double PorcentajeAprobacionSC { get; set; }

        public int DocumentosRecibidos30DiasSC { get; set; }

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



        //Datos para validar y recordar estar al dia con los expedientes fiscales

        public int DiasVigencia { get; set; }

        public virtual ExpedienteFiscal ExpedientesFiscales { get; set; }
       
    }


   

    





}
