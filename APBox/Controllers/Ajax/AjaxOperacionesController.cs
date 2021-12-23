using APBox.Context;
using API.Operaciones.ComplementosPagos;
using API.Operaciones.ComplementoCartaPorte;
using API.CatalogosCartaPorte;
using Aplicacion.LogicaPrincipal.CargasMasivas.CSV;
using CFDI.API.Enums.CFDI33;
using CFDI.API.Enums.Complementos.Pagos10;
using System;
using System.Web.Mvc;
using System.Collections.Generic;
using API.Models.Dto;
using System.Linq;
using API.Enums.CartaPorteEnums;

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

        public PartialViewResult AgregarUbicacion(String TipoUbicacion,
            String TipoEstacion,
            String TipoEstacionId, 
            String IdUbicacion,
         String RFCRemitenteDestinatario,
             String NombreRemitenteDestinatario,
             string NumRegIdTrib,
              String ResidenciaFiscal,
               String NombreEstacion,
                String NumEstacion,
                string NavegacionTrafico,
                DateTime FechaHoraSalidaLlegada,
                Decimal DistanciaRecorrida,
                 String Calle,
                  String CodigoPostal,
                   String Colonia,
                   string ColoniaText,
                   String Estado,
                   string EstadoText,
                    String Localidad,
                    string LocalidadText,
                    String Municipio,
                    string MunicipioText,
                    String NumeroExterior,
                    String NumeroInterior,
                    String Pais,
                    string PaisText,
                    String Referencia)
        {
            c_Pais? ResidenciaFiscalParse=null;
            if (ResidenciaFiscal == "")
            {
                ResidenciaFiscalParse = null;
            }
            else { ResidenciaFiscalParse = (c_Pais)Enum.Parse(typeof(c_Pais), ResidenciaFiscal, true); }
            var Ubicacion = new Ubicacion
            {
                TipoUbicacion = TipoUbicacion,
                TipoEstaciones = TipoEstacion,
                TipoEstacion_Id = TipoEstacionId,
                IDUbicacion = IdUbicacion,
                RfcRemitenteDestinatario = RFCRemitenteDestinatario,
                NombreRemitenteDestinatario = NombreRemitenteDestinatario,
                NumRegIdTrib = NumRegIdTrib,
                ResidenciaFiscal = ResidenciaFiscalParse,
                NombreEstacion = NombreEstacion,
                NumEstacion = NumEstacion,
                Estaciones_Id = NumEstacion,
                NavegacionTrafico= NavegacionTrafico,
                FechaHoraSalidaLlegada = FechaHoraSalidaLlegada,
                DistanciaRecorrida = DistanciaRecorrida,
                Domicilio = new Domicilio()
                {
                    Calle = Calle,
                    CodigoPostal = CodigoPostal,
                    Colonia = Colonia,
                    colonias = ColoniaText,
                    Estado = Estado,
                    estados = EstadoText,
                    Localidad = Localidad,
                    localidades = LocalidadText,
                    Municipio = Municipio,
                    municipios = MunicipioText,
                    NumeroExterior = NumeroExterior,
                    NumeroInterior = NumeroInterior,
                    Pais = Pais,
                    paiss = PaisText,
                    Referencia = Referencia
                }
            };

            return PartialView("~/Views/ComplementosCartaPorte/Ubicacion.cshtml", Ubicacion);
        }

        /*public PartialViewResult AgregarUbicacionDestino(string DETipoEstacion,string DETipoEstacionId,string IdDestino,string DERFCRemitente
            , string DENombreRemitente,string DEResidenciaFiscal, string DENombreEstacion,string DENumEstacion,DateTime FechaHoraLlegada
            , Decimal DEDistanciaRecorrida,string DECalle,string DECodigoPostal,string DEColonia, string DEEstado,string DELocalidad
            , string DEMunicipio,string DENumeroExterior,string DENumeroInterior,string DEPais,string DEReferencia)
        {
            var UbicacionDestino = new UbicacionDestino()
            {
                TipoEstaciones = DETipoEstacion,
                TipoEstacion_Id = DETipoEstacionId,
                IDUbicacionDestino = IdDestino,
                RfcDestinatario = DERFCRemitente,
                NombreDestinatario = DENombreRemitente,
                ResidenciaFiscal = (c_Pais)Enum.Parse(typeof(c_Pais), DEResidenciaFiscal, true),
                NombreEstacion = DENombreEstacion,
                NumEstacion = DENumEstacion,
                FechaHoraLlegada = FechaHoraLlegada,
                DistanciaRecorrida = DEDistanciaRecorrida,
                Domicilio = new Domicilio()
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

            };
            return PartialView("~/Views/ComplementosCartaPorte/UbicacionDestino.cshtml", UbicacionDestino);  
        }*/

        public PartialViewResult AgregarMercancia(String ClaveProdServID, string ClaveProdSTCCID,string Descripcion,
            int Cantidad,string Unidad,string ClaveUnidadID,string Dimensiones,bool MaterialPeligorosoSN, 
            string MaterialPeligrosoID, string DescripcionEmbalaje, string TipoEmbalajeID,Decimal PesoEnKg,string ValorMercancia,
            string Moneda, string FraccionArancelariaID, string UUIDComercioExt/*, string PPedimento, string GINumIdent, string GIDescripIdent,
            Decimal GIPesoIdent,Decimal CTCantidad, string CTCveTransporteID, string CTCveOrigen, string CTCveDestino*/,
            string DEClaveUnidadPesoID, Decimal DEPesoBruto, Decimal DEPEsoNeto,Decimal DEPesoTara, int DENumPiezas,
            List<Pedimentos> PedimentoArray, List<GuiasIdentificacion> GIdentificacionArray, List<CantidadTransportada> CTransportadaArray)
        {
            
            var mercancia = new Mercancia()
            {
                ClaveProdServCP = ClaveProdServID,
                ClaveProdSTCC = ClaveProdSTCCID,
                Descripcion = Descripcion,
                Cantidad = Cantidad,
                Unidad = Unidad,
                ClavesUnidad = ClaveUnidadID,
                Dimensiones = Dimensiones,
                MaterialPeligrosos = MaterialPeligorosoSN,
                ClaveMaterialPeligroso = MaterialPeligrosoID,
                DescripEmbalaje = DescripcionEmbalaje,
                TipoEmbalaje_Id = TipoEmbalajeID,
                PesoEnKg = PesoEnKg,
                ValorMercancia = ValorMercancia,
                Moneda = (c_Moneda)Enum.Parse(typeof(c_Moneda), Moneda, true), 
                FraccionArancelarias= FraccionArancelariaID,
                UUIDComecioExt = UUIDComercioExt,
                DetalleMercancia = new DetalleMercancia()
                {
                    ClaveUnidadPeso_Id = DEClaveUnidadPesoID,
                    PesoBruto = DEPesoBruto,
                    PesoNeto = DEPEsoNeto,
                    PesoTara = DEPesoTara,
                    NumPiezas = DENumPiezas
                }
                
            };
            if (PedimentoArray != null)
            {
                mercancia.Pedimentoss = new List<Pedimentos>();
                foreach(var ped in PedimentoArray)
                {
                    mercancia.Pedimentoss.Add(ped);
                }
            }
            if (GIdentificacionArray != null)
            {
                mercancia.GuiasIdentificacionss = new List<GuiasIdentificacion>();
                foreach(var GIdent in GIdentificacionArray)
                {
                    mercancia.GuiasIdentificacionss.Add(GIdent);
                }
            }
            if (CTransportadaArray != null)
            {
                mercancia.CantidadTransportadass = new List<CantidadTransportada>();
                foreach(var CTrans in CTransportadaArray)
                {
                    mercancia.CantidadTransportadass.Add(CTrans);
                }
            }
            return PartialView("~/Views/ComplementosCartaPorte/Mercancia.cshtml", mercancia);
        }

        public PartialViewResult AgregarFTransporte(string FTransporte, string RFCFigura,string NumLicencia, string NombreFigura,
            string NumRegIdTribFigura,string ResidenciaFiscalFigura, List<PartesTransporteDto> PartesTransporte)
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
            //var listPartTrans = new List<PartesTransporte>();
            TiposFigura.PartesTransportes = new List<PartesTransporte>();
            if (PartesTransporte != null)
            {
                var parteTransporte = new PartesTransporte();
                foreach (var parteTransporteDto in PartesTransporte)
                {
                    parteTransporte.ParteTransporte = parteTransporteDto.ParteTransporte;
                    parteTransporte.Domicilio = new Domicilio() {
                        Pais = parteTransporteDto.Pais,
                        Estado = parteTransporteDto.Estado,
                        Municipio = parteTransporteDto.Municipio,
                        Localidad = parteTransporteDto.Localidad,
                        CodigoPostal = parteTransporteDto.CodigoPostal,
                        Colonia = parteTransporteDto.Colonia,
                        Calle = parteTransporteDto.Calle,
                        NumeroExterior = parteTransporteDto.NumeroExterior,
                        NumeroInterior = parteTransporteDto.NumeroInterior,
                        Referencia = parteTransporteDto.Referencia
                };
                    //listPartTrans.Add(parteTransporte);
                    
                    TiposFigura.PartesTransportes.Add(parteTransporte);
                }
            }
            return PartialView("~/Views/ComplementosCartaPorte/FiguraTransporte.cshtml", TiposFigura);
        }
        public PartialViewResult AgregarFTransporte2(ComplementoCartaPorte cartaPorte)
        {
            var CartaPorte = new ComplementoCartaPorte()
            {
                TiposFigura = new TiposFigura()
                {
                    NombreFigura = cartaPorte.TiposFigura.NombreFigura
                }
            };
            return PartialView("~/Views/ComplementosCartaPorte/FiguraTransporte.cshtml", CartaPorte);
        }

                  
        public PartialViewResult AgregarPTransporte(string PTransporte, string Pais, string PaisText, string Estado,string EstadoText,string Municipio,string MunicipioText,string Localidad,
            string LocalidadText,string CodigoPostal,string Colonia,string ColoniaText,string Calle,string NumExterior,string NumInterior,string Referencia)
        {
            var PartesTransporte = new PartesTransporte()
            {
                ParteTransporte = PTransporte,
                Domicilio = new Domicilio()
                {
                    Pais = Pais,
                    paiss = PaisText,
                    Estado = Estado,
                    estados = EstadoText,
                    Municipio = Municipio,
                    municipios = MunicipioText,
                    Localidad = Localidad,
                    localidades = LocalidadText,
                    CodigoPostal = CodigoPostal,
                    Colonia = Colonia,
                    colonias = ColoniaText,
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

        public PartialViewResult AgregarTFCarro(string TipoCarroID,string MatriculaCarro,string GuiaCarro,Decimal TonNetasCarro,List<ContenedorC> contenedorC)
        {
            var Carro = new Carro()
            {
                TipoCarro_Id = TipoCarroID,
                MatriculaCarro = MatriculaCarro,
                GuiaCarro = GuiaCarro,
                ToneladasNetasCarro = TonNetasCarro
            };
            Carro.ContenedoresC = new List<ContenedorC>();
            if (contenedorC != null)
            {
                foreach(var contenedor in contenedorC)
                {
                    Carro.ContenedoresC.Add(contenedor);
                }
            }
            
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
            return PartialView("~/Views/ComplementosCartaPorte/Pedimentos.cshtml", Pedimentos);
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

        public PartialViewResult AgregarConceptos(string ClaveProdServID, string ClaveUnidadID, string Descripcion,string TTipoImpuesto, Decimal TBase
            ,string TImpuesto, string TTipoFactor,Decimal TTasaOCuota,Decimal TImporte,Decimal TTImpuestosTR,string RTipoImpuesto, Decimal RBase,string RImpuesto
            ,string RTipofactor,Decimal RTasaOCuota,Decimal RImporte,Decimal RTImpuestoTR)
        {
            var Conceptos = new Conceptos()
            {
                ClavesProdServ = ClaveProdServID,
                ClavesUnidad = ClaveUnidadID,
                Descripcion = Descripcion,
                Traslado = new SubImpuestoC()
                {
                    TipoImpuesto = TTipoImpuesto,
                    Base=TBase,
                    Impuesto = (c_Impuesto)Enum.Parse(typeof(c_Impuesto), TImpuesto, true),
                    TipoFactor = (c_TipoFactor)Enum.Parse(typeof(c_TipoFactor), TTipoFactor, true),
                    TasaOCuota = TTasaOCuota,
                    Importe = TImporte,
                    TotalImpuestosTR = TTImpuestosTR,
                },
                Retencion = new SubImpuestoC()
                {
                    TipoImpuesto = RTipoImpuesto,
                    Base = RBase,
                    Impuesto = (c_Impuesto)Enum.Parse(typeof(c_Impuesto), RImpuesto, true),
                    TipoFactor = (c_TipoFactor)Enum.Parse(typeof(c_TipoFactor), RTipofactor, true),
                    TasaOCuota = RTasaOCuota,
                    Importe = RImporte,
                    TotalImpuestosTR = RTImpuestoTR
                }
            };
            return PartialView("~/Views/ComplementosCartaPorte/Conceptos.cshtml", Conceptos);
        }

        public PartialViewResult AgregarRemolque(string Placa,string TipoRemolqueId, string TipoRemolque)
        {
            var remolques = new Remolques()
            {
                Placa = Placa,
                SubTipoRem_Id = TipoRemolqueId,
                SubTipoRems = TipoRemolque

            };
            return PartialView("~/Views/ComplementosCartaPorte/Remolques.cshtml",remolques);
        }

        public int Buscar(string valor, String tipo)
        {
           
            var busqueda = 0;
            if (tipo.Equals("serv"))
            {
                busqueda = _db.ClavesProdServCP.Where(a => a.c_ClaveUnidad.Equals(valor)).Count();
            }
            if (tipo.Equals("stcc"))
            {
                busqueda = _db.ClavesProdSTCC.Where(a => a.ClaveSTCC.Equals(valor)).Count();
            }
            if (tipo.Equals("claveUnidad"))
            {
                busqueda = _db.ClavesUnidad.Where(a => a.c_ClaveUnidad.Equals(valor)).Count();
            }
            if (tipo.Equals("MaterialPeligroso"))
            {
                busqueda = _db.MaterialesPeligrosos.Where(a => a.ClaveMaterialPeligroso.Equals(valor)).Count();
            }


            return busqueda;
        }
        #region PopulaForma

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }

        #endregion

    }
}