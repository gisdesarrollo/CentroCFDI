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

        public PartialViewResult AgregarUbicacion(String ORTipoUbicacion,String ORTipoEstacion,String ORTipoEstacionId, String IdOrigen,
         String ORRFCRemitente,
             String ORNombreRemitente,
              String ORResidenciaFiscal,
               String ORNombreEstacion,
                String ORNumEstacion,
                DateTime FechaHoraSalida,
                Decimal ORDistanciaRecorrida,
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
                    String DETipoEstacion,
                    String DETipoEstacionId,
                     String IdDestino,
                     String DERFCDestinatario,
                     String DENombreDestinatario,
                     String DEResidenciaFiscal,
                     String DENombreEstacion,
                     String DENumEstacion,
                    DateTime FechaHoraLlegada,
                    Decimal DEDistanciaRecorrida,
                    String DECalle,
                     String DECodigoPostal,
                     String DEColonia, String DEEstado, String DELocalidad, String DEMunicipio, String DENumeroExterior,
             String DENumeroInterior, String DEPais, String DEReferencia)
        {

            var Ubicacion = new Ubicacion
            {
                UbicacionOrigen = new UbicacionOrigen
                {
                    DistanciaRecorrida = ORDistanciaRecorrida,
                    TipoEstacion_Id = ORTipoEstacionId,
                    TipoEstaciones = ORTipoEstacion,
                    IDUbicacionOrigen = IdOrigen,
                    RfcRemitente = ORRFCRemitente,
                    NombreRemitente = ORNombreRemitente,
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
                    DistanciaRecorrida = DEDistanciaRecorrida,
                    TipoEstacion_Id = DETipoEstacionId,
                    TipoEstaciones = DETipoEstacion,
                    IDUbicacionDestino = IdDestino,
                    RfcDestinatario = DERFCDestinatario,
                    NombreDestinatario = DENombreDestinatario,
                    ResidenciaFiscal = (c_Pais)Enum.Parse(typeof(c_Pais), DEResidenciaFiscal, true),
                    NombreEstacion = DENombreEstacion,
                    NumEstacion = DENumEstacion,
                    FechaHoraLlegada = FechaHoraLlegada,
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

        public PartialViewResult AgregarMercancia(String ClaveProdServID, string ClaveProdSTCCID,string Descripcion,
            Decimal Cantidad,string Unidad,string ClaveUnidadID,string Dimensiones,bool MaterialPeligorosoSN, 
            string MaterialPeligrosoID, string DescripcionEmbalaje, string TipoEmbalajeID,Decimal PesoEnKg,string ValorMercancia,
            string Moneda, string FraccionArancelariaID, string UUIDComercioExt, string PPedimento, string GINumIdent, string GIDescripIdent,
            Decimal GIPesoIdent,Decimal CTCantidad, string CTCveTransporteID, string CTCveOrigen, string CTCveDestino,
            string DEClaveUnidadPesoID, Decimal DEPesoBruto, Decimal DEPEsoNeto,Decimal DEPesoTara, int DENumPiezas)
        {
            
            var mercancia = new Mercancia()
            {
                ClaveProdServCP_Id = ClaveProdServID,
                ClaveProdSTCC_Id = ClaveProdSTCCID,
                Descripcion = Descripcion,
                Cantidad = 0,
                Unidad = Unidad,
                ClaveUnidad_Id = ClaveUnidadID,
                Dimensiones = Dimensiones,
                MaterialPeligrosos = MaterialPeligorosoSN,
                MaterialPeligroso_Id = MaterialPeligrosoID,
                DescripEmbalaje = DescripcionEmbalaje,
                TipoEmbalaje_Id = TipoEmbalajeID,
                PesoEnKg = PesoEnKg,
                ValorMercancia = ValorMercancia,
                Moneda = (c_Moneda)Enum.Parse(typeof(c_Moneda), Moneda, true), 
                FraccionArancelaria_Id = FraccionArancelariaID,
                UUIDComecioExt = UUIDComercioExt,
                Pedimentos = new Pedimentos()
                {
                    Pedimento = PPedimento
                },
                GuiasIdentificacion = new GuiasIdentificacion()
                {
                    NumeroGuiaIdentificacion = GINumIdent,
                    DescripGuiaIdentificacion = GIDescripIdent,
                    PesoGuiaIdentificacion = GIPesoIdent
                },
                CantidadTransportada = new CantidadTransportada()
                {
                    Cantidad = CTCantidad,
                    IDOrigen = CTCveOrigen,
                    IDDestino = CTCveDestino,
                    ClaveTransporte =  CTCveTransporteID
                },
                DetalleMercancias = new DetalleMercancia()
                {
                    ClaveUnidadPeso_Id = DEClaveUnidadPesoID,
                    PesoBruto = DEPesoBruto,
                    PesoNeto = DEPEsoNeto,
                    PesoTara = DEPesoTara,
                    NumPiezas = DENumPiezas
                }
                
            };
            return PartialView("~/Views/ComplementosCartaPorte/Mercancia.cshtml", mercancia);
        }

        public PartialViewResult AgregarFTransporte(string FTransporte, string RFCFigura,string NumLicencia, string NombreFigura,
            string NumRegIdTribFigura,string ResidenciaFiscalFigura)
        {
            var TiposFigura = new TiposFigura()
            {
                FiguraTransporte = FTransporte,
                RFCFigura = RFCFigura,
                NumLicencia = NumLicencia,
                NombreFigura = NombreFigura,
                NumRegIdTribFigura = NumRegIdTribFigura,
                ResidenciaFiscalFigura = (c_Pais)Enum.Parse(typeof(c_Pais), ResidenciaFiscalFigura, true),
            };
            return PartialView("~/Views/ComplementosCartaPorte/FiguraTransporte.cshtml", TiposFigura);
        }

        public PartialViewResult AgregarPTransporte(string PTransporte, string Pais,string Estado,string Municipio,string Localidad,
            string CodigoPostal,string Colonia,string Calle,string NumExterior,string NumInterior,string Referencia)
        {
            var PartesTransporte = new PartesTransporte()
            {
                ParteTransporte = PTransporte,
                Domicilio = new Domicilio()
                {
                    Pais = Pais,
                    Estado = Estado,
                    Municipio = Municipio,
                    Localidad = Localidad,
                    CodigoPostal = CodigoPostal,
                    Colonia = Colonia,
                    Calle = Calle,
                    NumeroExterior = NumExterior,
                    NumeroInterior = NumInterior,
                    Referencia = Referencia
                }

            };
            return PartialView("~/Views/ComplementosCartaPorte/PartesTransporte.cshtml",PartesTransporte);
        }

        public PartialViewResult AgregarTMContenedor(string MatContenedor,string ContenedorMaritId, string NumPrecinto)
        {
            var ContenedorM = new  ContenedorM(){
                MatriculaContenedor = MatContenedor,
                ContenedorMaritimo_Id = ContenedorMaritId,
                NumPrecinto = NumPrecinto
            };
            return PartialView("~/Views/ComplementosCartaPorte/ContenedoresM.cshtml", ContenedorM);
        }

        public PartialViewResult AgregarTFDPaso(string TipoDPasoID, Decimal KilPagado)
        {
            var DerechosDePasos = new DerechosDePasos()
            {
                TipoDerechoDePaso_Id = TipoDPasoID,
                KilometrajePagado = KilPagado
            };

            return PartialView("~/Views/ComplementosCartaPorte/DerechosDePaso.cshtml",DerechosDePasos);
        }

        public PartialViewResult AgregarTFCarro(string TipoCarroID,string MatriculaCarro,string GuiaCarro,Decimal TonNetasCarro)
        {
            var Carro = new Carro()
            {
                TipoCarro_Id = TipoCarroID,
                MatriculaCarro = MatriculaCarro,
                GuiaCarro = GuiaCarro,
                ToneladasNetasCarro = TonNetasCarro
            };
            return PartialView("~/Views/ComplementosCartaPorte/Carro.cshtml",Carro);
        }

        public PartialViewResult AgregarTFContenedor(string ContenedorID,Decimal PesoContenedorVacio,Decimal pesoNetoMercancia)
        {
            var ContenedorC = new ContenedorC()
            {
                Contenedor_Id = ContenedorID,
                PesoContenedorVacio = PesoContenedorVacio,
                PesoNetoMercancia = pesoNetoMercancia
            };
            return PartialView("~/Views/ComplementosCartaPorte/ContenedoresC.cshtml",ContenedorC);
        }

        public PartialViewResult AgregarPedimentos(string Pedimento)
        {
            var Pedimentos = new Pedimentos()
            {
                Pedimento = Pedimento
            };
            return PartialView("~/Views/ComplementosCartaPorte/Pedimentos.cshtml", Pedimento);
        }

        public PartialViewResult AgregarGIdentificacion(string NumGuiIdentificacion,string DescripGuiaIdentificacion,Decimal PesoGuiaIdentificacion)
        {
            var GuiasIdent = new GuiasIdentificacion()
            {
                NumeroGuiaIdentificacion = NumGuiIdentificacion,
                DescripGuiaIdentificacion = DescripGuiaIdentificacion,
                PesoGuiaIdentificacion = PesoGuiaIdentificacion
            };
            return PartialView("~/Views/ComplementosCartaPorte/GuiasIdentificacion.cshtml",GuiasIdent);
        }

        public PartialViewResult AgregarCTransportadas(Decimal Cantidad, string CveTransporteID, string UbicacionOrigenID,string UbicacionDestinoID)
        {
            var CTransportadas = new CantidadTransportada()
            {
               Cantidad = Cantidad,
               CveTransporte_Id =CveTransporteID,
               IDOrigen = UbicacionOrigenID,
               IDDestino = UbicacionDestinoID
            };
            return PartialView("~/Views/ComplementosCartaPorte/CantidadTransportadas.cshtml", CTransportadas);
        }
        #region PopulaForma

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }

        #endregion

    }
}