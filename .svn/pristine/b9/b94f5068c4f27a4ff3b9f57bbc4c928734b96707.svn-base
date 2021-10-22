using APBox.Context;
using API.Operaciones.ComplementosPagos;
using Aplicacion.LogicaPrincipal.CargasMasivas.CSV;
using CFDI.API.Enums.CFDI33;
using CFDI.API.Enums.Complementos.Pagos10;
using System;
using System.Web.Mvc;

namespace APBox.Controllers.Ajax
{
    public class AjaxOperacionesController : Controller
    {

        #region Variables

        private readonly APBoxContext _db = new APBoxContext();

        #endregion

        public PartialViewResult AgregarPago(DateTime fechaPago, string formaPago, string moneda, double tipoCambio, double monto, string numeroOperacion, string tipoCadenaPago, string certificadoPago, string cadenaPago, string selloPago, string spei, int? bancoEmisorId, int? bancoReceptorId)
        {
            var pago = new Pago
            {
                BancoOrdenante = _db.BancosClientes.Find(bancoEmisorId),
                BancoOrdenanteId = bancoEmisorId,
                BancoBeneficiario = _db.BancosSucursales.Find(bancoReceptorId),
                BancoBeneficiarioId = bancoReceptorId,
                CadenaPago = cadenaPago,
                FechaPago = fechaPago,
                FormaPago = (c_FormaPago)Enum.Parse(typeof(c_FormaPago), formaPago, true),
                Moneda = (c_Moneda)Enum.Parse(typeof(c_Moneda), moneda, true),
                Monto = monto,
                NumeroOperacion = numeroOperacion,
                TipoCambio = tipoCambio,
                SucursalId = ObtenerSucursal(),
            };

            if(spei != null)
            {
                var pathSpei = String.Format(@"C:/Infodextra/Temp/{0}.xml", DateTime.Now.ToString("ddMMyyyymmssttt"));
                System.IO.File.WriteAllText(pathSpei, spei);
                
                var operacionesSpei = new OperacionesSpei();
                var xmlSpei = operacionesSpei.Decodificar(pathSpei);

                pago.TipoCadenaPago = c_TipoCadenaPago.Spei;
                pago.CertificadoPago = xmlSpei.numeroCertificado;
                pago.CadenaPago = xmlSpei.cadenaCDA;
                pago.SelloPago = xmlSpei.sello;
            }
            
            return PartialView("~/Views/ComplementosPagos/Pagos.cshtml", pago);
        }

        public PartialViewResult AgregarFacturaComplementoPago(int pagoId, int facturaEmitidaId, int numeroParcialidad, string moneda, double tipoCambio, double importeSaldoAnterior, double importePagado, double importeSaldoInsoluto)
        {
            var facturaEmitida = _db.FacturasEmitidas.Find(facturaEmitidaId);

            var documentoRelacionado = new DocumentoRelacionado
            {
                FacturaEmitidaId = facturaEmitida.Id,
                FacturaEmitida = facturaEmitida,
                ImportePagado = importePagado,
                ImporteSaldoAnterior = importeSaldoAnterior,
                ImporteSaldoInsoluto = importeSaldoInsoluto,
                NumeroParcialidad = numeroParcialidad,
                Moneda = (c_Moneda)Enum.Parse(typeof(c_Moneda), moneda, true),
                TipoCambio = tipoCambio,
                PagoId = pagoId,

                IdDocumento = facturaEmitida.Uuid,
                Folio = facturaEmitida.Folio.ToString(),
                MetodoPago = c_MetodoPago.PPD,
                Serie = facturaEmitida.Serie
            };

            return PartialView("~/Views/ComplementosPagos/FacturasDetalles.cshtml", documentoRelacionado);
        }

        #region PopulaForma

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }

        #endregion

    }
}