using API.Models.Control;
using API.Models.Facturas;
using Aplicacion.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aplicacion.LogicaPrincipal.Control
{
    public class PopularHome
    {

        #region Variables

        private readonly int _sucursalId;
        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion

        #region Constructor

        public PopularHome(int sucursalId)
        {
            _sucursalId = sucursalId;
        }

        #endregion

        public void Popular(ref HomeModel homeModel)


        {
            

           //Mes actual desde el primer dia hasta el dia actual 

            var fechaInicial = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            var fechaFinal = DateTime.Now;

           
            //Fechas de Acorde a los ingresos por cada mes  de los 6 meses hasta el mes actual

            //Mes Segundo
            var fechaInicial2 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1).AddDays(0);

            var fechaFinal2 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-0).AddDays(-1);

            //Mes Tercero
            var fechaInicial3 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-2).AddDays(0);

            var fechaFinal3 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1).AddDays(-1);

            //Mes Cuarto
            var fechaInicial4 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-3).AddDays(0);

            var fechaFinal4 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-2).AddDays(-1);

            //Mes Quinto
            var fechaInicial5 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-4).AddDays(0);

            var fechaFinal5 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-3).AddDays(-1);

            //Mes Sexto
            var fechaInicial6 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-5).AddDays(0);

            var fechaFinal6 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-4).AddDays(-1);



            //Nombres de los Meses anteriores hasta el actual

            string mes1 = DateTime.Now.ToString("MMM");

            string mes2 = DateTime.Now.AddMonths(-1).ToString("MMM");

            string mes3 = DateTime.Now.AddMonths(-2).ToString("MMM");

            string mes4 = DateTime.Now.AddMonths(-3).ToString("MMM");

            string mes5 = DateTime.Now.AddMonths(-4).ToString("MMM");

            string mes6 = DateTime.Now.AddMonths(-5).ToString("MMM");


            //Asignacion de los meses 

            homeModel.Mes1 = mes1;

            homeModel.Mes2 = mes2;

            homeModel.Mes3 = mes3;

            homeModel.Mes4 = mes4;

            homeModel.Mes5 = mes5;

            homeModel.Mes6 = mes6;



            //var fechaIniant = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(0).AddDays(0);

            // var fechaFiant = DateTime.Now;


            //homeModel.CfdiEmitidos = _db.FacturasEmitidas.Count(fe => fe.EmisorId == _sucursalId && fe.Fecha >= fechaIniant && fe.Fecha <= fechaFiant);

            //Clientes Totales
            homeModel.Clientes = _db.Clientes.Count(c => c.SucursalId == _sucursalId);

            //Facturas emitidas del mes presente
            homeModel.CfdiEmitidos = _db.FacturasEmitidas.Count(fe => fe.EmisorId == _sucursalId && fe.Fecha >= fechaInicial && fe.Fecha <= fechaFinal);


            //Complemento Pago Pendientes 
            var ComplementoPagoPen = homeModel.ComplementosPagoPendientes = _db.ComplementosPago.Count(fe => fe.SucursalId == _sucursalId && !fe.Generado && fe.FechaDocumento >= fechaInicial && fe.FechaDocumento <= fechaFinal);


            //Complemento Carta Porte Pendientes
            var ComplementoCartaPen = homeModel.ComplementoCartaPortePendientes = _db.ComplementoCartaPortes.Count(fe => fe.SucursalId == _sucursalId && !fe.Generado && fe.FechaDocumento >= fechaInicial && fe.FechaDocumento <= fechaFinal);


            //Complemento Cfdi Pendientes
            var ComplementoCfdiPen = homeModel.ComprobanteCfdiPendientes = _db.ComprobantesCfdi.Count(fe => fe.SucursalId == _sucursalId && !fe.Generado && fe.FechaDocumento >= fechaInicial && fe.FechaDocumento <= fechaFinal);

            //suma de todos los comprobantes CFDI pendientes
            var sumaCFDi = homeModel.Total = ComplementoCartaPen + ComplementoPagoPen + ComplementoCfdiPen;

            //Monto de total  de ingreso del mes presente  correspondiente a Facturas Emitidas
            homeModel.MontoCFDI = (int)_db.FacturasEmitidas.Where(fe => fe.EmisorId == _sucursalId && fe.TipoComprobante==API.Enums.c_TipoDeComprobante.I && fe.Fecha >= fechaInicial &&  fe.Fecha <= fechaFinal ).Select(fe => fe.Total).DefaultIfEmpty(0).Sum();


            //Rango de fechas para obtener los ingresos por cada mes 
            

            homeModel.Mes2total = (int)_db.FacturasEmitidas.Where(fe => fe.EmisorId == _sucursalId && fe.TipoComprobante == API.Enums.c_TipoDeComprobante.I && fe.Fecha >= fechaInicial2 && fe.Fecha <= fechaFinal2).Select(fe => fe.Total).DefaultIfEmpty(0).Sum(); ;

            homeModel.Mes3total = (int)_db.FacturasEmitidas.Where(fe => fe.EmisorId == _sucursalId && fe.TipoComprobante == API.Enums.c_TipoDeComprobante.I && fe.Fecha >= fechaInicial3 && fe.Fecha <= fechaFinal3).Select(fe => fe.Total).DefaultIfEmpty(0).Sum(); ;

            homeModel.Mes4total = (int)_db.FacturasEmitidas.Where(fe => fe.EmisorId == _sucursalId && fe.TipoComprobante == API.Enums.c_TipoDeComprobante.I && fe.Fecha >= fechaInicial4 && fe.Fecha <= fechaFinal4).Select(fe => fe.Total).DefaultIfEmpty(0).Sum(); ;

            homeModel.Mes5total = (int)_db.FacturasEmitidas.Where(fe => fe.EmisorId == _sucursalId && fe.TipoComprobante == API.Enums.c_TipoDeComprobante.I && fe.Fecha >= fechaInicial5 && fe.Fecha <= fechaFinal5).Select(fe => fe.Total).DefaultIfEmpty(0).Sum(); ;

            homeModel.Mes6total = (int)_db.FacturasEmitidas.Where(fe => fe.EmisorId == _sucursalId && fe.TipoComprobante == API.Enums.c_TipoDeComprobante.I && fe.Fecha >= fechaInicial6 && fe.Fecha <= fechaFinal6).Select(fe => fe.Total).DefaultIfEmpty(0).Sum(); ;







        }



    }
}
