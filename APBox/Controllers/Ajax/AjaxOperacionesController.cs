using APBox.Context;
using API.Operaciones.ComplementosPagos;
using API.Operaciones.ComplementoCartaPorte;
using API.CatalogosCartaPorte;
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

            if (spei != null)
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

        public PartialViewResult AgregarUbicacion(Decimal DistanciaRecorrida, String TipoEstacionId, String TipoEstacion, String IdOrigen,
         String ORRFCRemitente,
             String ORNombreRemitente,
              String ORResidenciaFiscal,
               String ORNombreEstacion,
                String ORNumEstacion,
                DateTime FechaHoraSalida,
                 String ORCalle,
                  String ORCodigoPostal,
                   String ORColonia,
                   String OREstado,
                    String ORLocalidad,
                    String ORMunicipio,
                    String ORNumeroExterior,
                    String ORNumeroInterior,
                    String ORPais,
                    String ORReferencia,
                     String IdDestino,
                     String DERFCDestinatario,
                     String DENombreDestinatario,
                     String DEResidenciaFiscal,
                     String DENombreEstacion,
                     String DENumEstacion,
                    DateTime FechaHoraProgLlegada,
                     String DECalle,
                     String DECodigoPostal,
                     String DEColonia, String DEEstado, String DELocalidad, String DEMunicipio, String DENumeroExterior,
             String DENumeroInterior, String DEPais, String DEReferencia)
        {

            var Ubicacion = new Ubicacion
            {
                /*DistanciaRecorrida = DistanciaRecorrida,
                TipoEstacion_Id = TipoEstacionId,
                TipoEstaciones = TipoEstacion,*/
                UbicacionOrigen = new UbicacionOrigen
                {
                    DistanciaRecorrida = DistanciaRecorrida,

                    IDUbicacionOrigen = IdOrigen,
                    RfcRemitenteDestinatario = ORRFCRemitente,
                    NombreRemitenteDestinatario = ORNombreRemitente,
                    ResidenciaFiscal = (c_Pais)Enum.Parse(typeof(c_Pais), ORResidenciaFiscal, true),
                    NombreEstacion = ORNombreEstacion,
                    NumEstacion = ORNumEstacion,
                    FechaHoraSalida = FechaHoraSalida,
                    Domicilio = new Domicilio
                    {
                        Calle=ORCalle,
                        CodigoPostal=ORCodigoPostal,
                        Colonia = ORColonia,
                        Estado = OREstado,
                        Localidad = ORLocalidad,
                        Municipio = ORMunicipio,
                        NumeroExterior = ORNumeroExterior,
                        NumeroInterior = ORNumeroInterior,
                        Pais = ORPais,
                        Referencia = ORReferencia
                    }
                },
                UbicacionDestino = new UbicacionDestino
                {
                    IDUbicacionDestino = IdDestino,
                    RfcRemitenteDestinatario = DERFCDestinatario,
                    NombreRemitenteDestinatario = DENombreDestinatario,
                    ResidenciaFiscal = (c_Pais)Enum.Parse(typeof(c_Pais), DEResidenciaFiscal, true),
                    NombreEstacion = DENombreEstacion,
                    NumEstacion = DENumEstacion,
                    FechaHoraLlegada = FechaHoraProgLlegada,
                    Domicilio = new Domicilio
                    {
                        Calle = DECalle,
                        CodigoPostal = DECodigoPostal,
                        Colonia = DEColonia,
                        Estado = DEEstado,
                        Localidad = DELocalidad,
                        Municipio = DEMunicipio,
                        NumeroExterior = DENumeroExterior,
                        NumeroInterior = DENumeroInterior,
                        Pais = DEPais,
                        Referencia = DEReferencia
                    }
                }

            };

            return PartialView("~/Views/ComplementosCartaPorte/Ubicacion.cshtml", Ubicacion);
        }

        #region PopulaForma

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }

        #endregion

    }
}