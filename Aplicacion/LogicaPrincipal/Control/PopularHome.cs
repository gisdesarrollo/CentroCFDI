using API.Catalogos;
using API.Models.Control;
using Aplicacion.Context;
using System;
using System.Linq;

namespace Aplicacion.LogicaPrincipal.Control
{
    public class PopularHome
    {
        #region Variables

        private readonly int _sucursalId;
        private readonly int _usuarioId;
        private readonly int _socioComercialId;
        private readonly int _departamentoId;
        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion Variables

        #region Constructor

        public PopularHome(int sucursalId, int usuarioId, int socioComercialId, int departamentoId)
        {
            //var usuario = _db.Usuarios.Find(usuarioId);
            _sucursalId = sucursalId;
            _usuarioId = usuarioId;
            _socioComercialId = socioComercialId;
            _departamentoId = departamentoId;
        }

        #endregion Constructor

        public void Popular(ref HomeModel homeModel)
        {
            //Mes actual desde el primer dia hasta el dia actual
            var fechaInicial = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            var fechaFinal = DateTime.Now;

            //datos para home de recepción de documentos Usuario y Cuentas por pagar

            homeModel.DocumentosRecibidosEnRevisionUsuario = _db.DocumentosRecibidos
                .Count(d => d.SucursalId == _sucursalId &&
                            d.EstadoComercial == API.Enums.c_EstadoComercial.EnRevision &&
                            d.AprobacionesDR.UsuarioSolicitante_Id == _usuarioId);

            homeModel.DocumentosRecibidosDepartamentoUsuario = _db.DocumentosRecibidos
                .Count(d => d.SucursalId == _sucursalId &&
                            d.EstadoComercial == API.Enums.c_EstadoComercial.EnRevision &&
                            d.AprobacionesDR.DepartamentoUsuarioSolicitante_Id == _departamentoId);

            homeModel.DocumentosPagosRevisionUsuario = _db.DocumentosRecibidos
                .Count(d => d.SucursalId == _sucursalId &&
                            d.EstadoPago == API.Enums.c_EstadoPago.EnRevision);

            homeModel.DocumentosPagosAprobadoUsuario = _db.DocumentosRecibidos
                .Count(d => d.SucursalId == _sucursalId &&
                            d.EstadoPago == API.Enums.c_EstadoPago.Aprobado);

            homeModel.PagosEnRevisionUsuario = _db.DocumentosRecibidos
                .Count(d => d.SucursalId == _sucursalId &&
                            d.EstadoPago == API.Enums.c_EstadoPago.EnRevision);

            homeModel.PagosEnRevisionUsuario = _db.DocumentosRecibidos
                .Count(d => d.SucursalId == _sucursalId &&
                            d.EstadoPago == API.Enums.c_EstadoPago.EnRevision);

            homeModel.PorcentajeAprobacionUsuario = PorcentajeAprobacionUsuario();

            homeModel.DocumentosRecibidos30DiasUsuario = DocumentosRecibidos30DiasUsuario();

            //datos para home de recepción de documentos Socios Comerciales

            homeModel.DocumentosRecibidosEnRevisionUsuarioSC = _db.DocumentosRecibidos
                .Count(d => d.SucursalId == _sucursalId &&
                            d.EstadoComercial == API.Enums.c_EstadoComercial.EnRevision &&
                            d.AprobacionesDR.UsuarioEntrega_Id == _usuarioId &&
                            d.SocioComercialId == _socioComercialId);

            homeModel.DocumentosRecibidosEnRevisionSC = _db.DocumentosRecibidos
                .Count(d => d.SucursalId == _sucursalId &&
                            d.EstadoComercial == API.Enums.c_EstadoComercial.EnRevision &&
                            d.SocioComercialId == _socioComercialId);

            homeModel.DocumentosPagosRevisionSC = _db.DocumentosRecibidos
                .Count(d => d.SucursalId == _sucursalId &&
                            d.EstadoPago == API.Enums.c_EstadoPago.EnRevision &&
                            d.SocioComercialId == _socioComercialId);

            homeModel.PorcentajeAprobacionSC = PorcentajeAprobacionSC();

            homeModel.DocumentosRecibidos30DiasSC = DocumentosRecibidos30DiasSC();

            //rechazados de los ultimos 15 días
            var fechaInicialRechazados = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var fechaFinalRechazados = DateTime.Now;
            homeModel.DocumentosRechazadosSC = _db.DocumentosRecibidos
                .Count(d => d.SucursalId == _sucursalId &&
                            d.EstadoPago == API.Enums.c_EstadoPago.Rechazado &&
                            d.SocioComercialId == _socioComercialId &&
                            d.FechaEntrega >= fechaInicialRechazados &&
                            d.FechaEntrega <= fechaFinalRechazados);

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

            //Clientes Totales
            homeModel.Clientes = _db.SociosComerciales.Count(c => c.SucursalId == _sucursalId);

            //Facturas emitidas del mes presente
            homeModel.CfdiEmitidos = _db.FacturasEmitidasTemp.Count(fe => fe.EmisorId == _sucursalId && fe.Fecha >= fechaInicial && fe.Fecha <= fechaFinal);

            //Complemento Pago Pendientes
            var ComplementoPagoPen = homeModel.ComplementosPagoPendientes = _db.ComplementosPago.Count(fe => fe.SucursalId == _sucursalId && !fe.Generado && fe.FechaDocumento >= fechaInicial && fe.FechaDocumento <= fechaFinal);

            //Complemento Carta Porte Pendientes
            var ComplementoCartaPen = homeModel.ComplementoCartaPortePendientes = _db.ComplementoCartaPortes.Count(fe => fe.SucursalId == _sucursalId && !fe.Generado && fe.FechaDocumento >= fechaInicial && fe.FechaDocumento <= fechaFinal);

            //Complemento Cfdi Pendientes
            var ComplementoCfdiPen = homeModel.ComprobanteCfdiPendientes = _db.ComprobantesCfdi.Count(fe => fe.SucursalId == _sucursalId && !fe.Generado && fe.FechaDocumento >= fechaInicial && fe.FechaDocumento <= fechaFinal);

            //suma de todos los comprobantes CFDI pendientes
            var sumaCFDi = homeModel.Total = ComplementoCartaPen + ComplementoPagoPen + ComplementoCfdiPen;

            //Monto de total  de ingreso del mes presente  correspondiente a Facturas Emitidas
            homeModel.MontoCFDI = (int)_db.FacturasEmitidasTemp.Where(fe => fe.EmisorId == _sucursalId && fe.TipoComprobante == API.Enums.c_TipoDeComprobante.I && fe.Fecha >= fechaInicial && fe.Fecha <= fechaFinal).Select(fe => fe.Total).DefaultIfEmpty(0).Sum();

            //Rango de fechas para obtener los ingresos por cada mes

            homeModel.Mes2total = (int)_db.FacturasEmitidasTemp.Where(fe => fe.EmisorId == _sucursalId && fe.TipoComprobante == API.Enums.c_TipoDeComprobante.I && fe.Fecha >= fechaInicial2 && fe.Fecha <= fechaFinal2).Select(fe => fe.Total).DefaultIfEmpty(0).Sum(); ;

            homeModel.Mes3total = (int)_db.FacturasEmitidasTemp.Where(fe => fe.EmisorId == _sucursalId && fe.TipoComprobante == API.Enums.c_TipoDeComprobante.I && fe.Fecha >= fechaInicial3 && fe.Fecha <= fechaFinal3).Select(fe => fe.Total).DefaultIfEmpty(0).Sum(); ;

            homeModel.Mes4total = (int)_db.FacturasEmitidasTemp.Where(fe => fe.EmisorId == _sucursalId && fe.TipoComprobante == API.Enums.c_TipoDeComprobante.I && fe.Fecha >= fechaInicial4 && fe.Fecha <= fechaFinal4).Select(fe => fe.Total).DefaultIfEmpty(0).Sum(); ;

            homeModel.Mes5total = (int)_db.FacturasEmitidasTemp.Where(fe => fe.EmisorId == _sucursalId && fe.TipoComprobante == API.Enums.c_TipoDeComprobante.I && fe.Fecha >= fechaInicial5 && fe.Fecha <= fechaFinal5).Select(fe => fe.Total).DefaultIfEmpty(0).Sum(); ;

            homeModel.Mes6total = (int)_db.FacturasEmitidasTemp.Where(fe => fe.EmisorId == _sucursalId && fe.TipoComprobante == API.Enums.c_TipoDeComprobante.I && fe.Fecha >= fechaInicial6 && fe.Fecha <= fechaFinal6).Select(fe => fe.Total).DefaultIfEmpty(0).Sum(); ;

            //validación de vigencia de expediente fiscal

            if (_socioComercialId != 0)
            {
                var expedientefiscal = _db.ExpedientesFiscales
                          .Where(e => e.SucursalId == _sucursalId && e.SocioComercialId == _socioComercialId)
                          .OrderByDescending(e => e.Anio)
                          .ThenByDescending(e => e.Mes)
                          .FirstOrDefault();
                if (expedientefiscal != null)
                {
                    var vigencia = expedientefiscal.Vigencia;

                    var dias = (vigencia - DateTime.Now).Days;

                    homeModel.DiasVigencia = dias;
                    homeModel.ExpedientesFiscales = _db.ExpedientesFiscales.Where(e => e.SucursalId == _sucursalId && e.SocioComercialId == _socioComercialId).FirstOrDefault();
                }
            }
        }

        private double PorcentajeAprobacionUsuario()
        {
            // Determinar la fecha de hace 30 días atrás desde la fecha actual
            DateTime fechaInicio = DateTime.Now.AddDays(-30);
            DateTime fechaFin = DateTime.Now;

            // Obtener el número de facturas aprobadas y rechazadas en los últimos 30 días
            int facturasAprobadas = _db.DocumentosRecibidos
                .Count(d => d.SucursalId == _sucursalId && d.FechaEntrega >= fechaInicio && d.FechaEntrega <= fechaFin && d.EstadoPago == API.Enums.c_EstadoPago.Aprobado);

            int facturasRechazadas = _db.DocumentosRecibidos
                .Count(d => d.SucursalId == _sucursalId && d.FechaEntrega >= fechaInicio && d.FechaEntrega <= fechaFin && d.EstadoPago == API.Enums.c_EstadoPago.Rechazado);

            // Calcular el porcentaje de facturas aprobadas y rechazadas
            double porcentajeAprobadas = (double)facturasAprobadas / (facturasAprobadas + facturasRechazadas) * 100;
            double porcentajeRechazadas = (double)facturasRechazadas / (facturasAprobadas + facturasRechazadas) * 100;

            if (double.IsNaN(porcentajeAprobadas))
            {
                porcentajeAprobadas = 0;
            }

            return porcentajeAprobadas;
        }

        private double PorcentajeAprobacionSC()
        {
            // Determinar la fecha de hace 30 días atrás desde la fecha actual
            DateTime fechaInicio = DateTime.Now.AddDays(-30);
            DateTime fechaFin = DateTime.Now;

            // Obtener el número de facturas aprobadas y rechazadas en los últimos 30 días
            int facturasAprobadas = _db.DocumentosRecibidos
                .Count(d => d.SucursalId == _sucursalId && d.FechaEntrega >= fechaInicio && d.FechaEntrega <= fechaFin && d.EstadoPago == API.Enums.c_EstadoPago.Aprobado && d.SocioComercialId == _socioComercialId);

            int facturasRechazadas = _db.DocumentosRecibidos
                .Count(d => d.SucursalId == _sucursalId && d.FechaEntrega >= fechaInicio && d.FechaEntrega <= fechaFin && d.EstadoPago == API.Enums.c_EstadoPago.Rechazado && d.SocioComercialId == _socioComercialId);

            // Calcular el porcentaje de facturas aprobadas y rechazadas
            double porcentajeAprobadas = (double)facturasAprobadas / (facturasAprobadas + facturasRechazadas) * 100;
            double porcentajeRechazadas = (double)facturasRechazadas / (facturasAprobadas + facturasRechazadas) * 100;

            if (double.IsNaN(porcentajeAprobadas))
            {
                porcentajeAprobadas = 0;
            }

            return porcentajeAprobadas;
        }

        private int DocumentosRecibidos30DiasUsuario()
        {
            // Determinar la fecha de hace 30 días atrás desde la fecha actual
            DateTime fechaInicio = DateTime.Now.AddDays(-30);
            DateTime fechaFin = DateTime.Now;

            // Obtener el número de documentos recibidos en los últimos 30 días
            int documentosRecibidos = _db.DocumentosRecibidos
                .Count(d => d.SucursalId == _sucursalId &&
                            d.FechaEntrega >= fechaInicio &&
                            d.FechaEntrega <= fechaFin);

            return documentosRecibidos;
        }

        private int DocumentosRecibidos30DiasSC()
        {
            // Determinar la fecha de hace 30 días atrás desde la fecha actual
            DateTime fechaInicio = DateTime.Now.AddDays(-30);
            DateTime fechaFin = DateTime.Now;

            // Obtener el número de documentos recibidos en los últimos 30 días
            int documentosRecibidos = _db.DocumentosRecibidos
                .Count(d => d.SucursalId == _sucursalId &&
                            d.SocioComercialId == _socioComercialId &&
                            d.FechaEntrega >= fechaInicio &&
                            d.FechaEntrega <= fechaFin);

            return documentosRecibidos;
        }
    }
}