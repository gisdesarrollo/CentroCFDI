using APBox.Context;
using API.Operaciones.ComplementosPagos;
using API.Operaciones.ComplementoCartaPorte;
using API.CatalogosCartaPorte;
using Aplicacion.LogicaPrincipal.CargasMasivas.CSV;
using System;
using System.Web.Mvc;
using System.Collections.Generic;

using System.Linq;
using API.Enums.CartaPorteEnums;
using API.Enums;
using API.Models.Dto;
using API.Operaciones.RelacionesCfdi;

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
                BancoOrdenante = _db.BancosSociosComerciales.Find(bancoEmisorId),
                BancoOrdenanteId = bancoEmisorId,
                BancoBeneficiario = _db.BancosSucursales.Find(bancoReceptorId),
                BancoBeneficiarioId = bancoReceptorId,
                CadenaPago = cadenaPago,
                FechaPago = fechaPago,
                FormaPago = formaPago,
                Moneda = (c_Moneda)Enum.Parse(typeof(c_Moneda), moneda, true),
                Monto = monto,
                NumeroOperacion = numeroOperacion,
                TipoCambio = tipoCambio,
                SucursalId = ObtenerSucursal(),
                //TipoCadenaPago = "01" //SPEI
            };



            return PartialView("~/Views/ComplementosPagos/Pagos.cshtml", pago);
        }

        public PartialViewResult AgregarFacturaComplementoPago(int pagoId, int facturaEmitidaId, int numeroParcialidad, string moneda, Double equivalenciaDR,
            double importeSaldoAnterior, double importePagado, double importeSaldoInsoluto, string objetoImpuesto, List<TrasladoDR> traslados, List<RetencionDR> retenciones)
        {
            var facturaEmitida = _db.FacturasEmitidas.Find(facturaEmitidaId);
            Pago pago = _db.Pagos.Find(pagoId);

            var documentoRelacionado = new DocumentoRelacionado
            {
                FacturaEmitidaId = facturaEmitida.Id,
                FacturaEmitida = facturaEmitida,
                ImportePagado = importePagado,
                ImporteSaldoAnterior = importeSaldoAnterior,
                ImporteSaldoInsoluto = importeSaldoInsoluto,
                NumeroParcialidad = numeroParcialidad,
                Moneda = (c_Moneda)Enum.Parse(typeof(c_Moneda), moneda, true),
                EquivalenciaDR = equivalenciaDR,
                PagoId = pagoId,

                IdDocumento = facturaEmitida.Uuid,
                Folio = facturaEmitida.Folio.ToString(),
                //MetodoPago = c_MetodoPago.PPD,
                Serie = facturaEmitida.Serie,
                ObjetoImpuestoId = objetoImpuesto,

            };
            if (traslados != null)
            {
                documentoRelacionado.Traslados = new List<TrasladoDR>();
                foreach (var traslado in traslados)
                {
                    documentoRelacionado.Traslados.Add(traslado);
                }
            }
            if (retenciones != null)
            {
                documentoRelacionado.Retenciones = new List<RetencionDR>();
                foreach (var retencion in retenciones)
                {
                    documentoRelacionado.Retenciones.Add(retencion);
                }
            }


            return PartialView("~/Views/ComplementosPagos/FacturasDetalles.cshtml", documentoRelacionado);
        }

        public PartialViewResult AgregarCfdiRelacionado(String TipoRelacion, String UUID)
        {
            var cfdiRelacionado = new CfdiRelacionado()
            {
                TipoRelacion = TipoRelacion,
                UUIDCfdiRelacionado = UUID
            };
            return PartialView("~/Views/CfdiRelacionados/CfdiRelacionado.cshtml", cfdiRelacionado);
        }
        public PartialViewResult AgregarDTraslado(Decimal Tbase, string Timpuesto, string TtipoFactor, Decimal TtasaOCuota, Decimal Timporte)
        {

            var traslado = new TrasladoDR()
            {
                Base = (double)Math.Round(Tbase, 2),//6
                Impuesto = Timpuesto,
                TipoFactor = (c_TipoFactor)Enum.Parse(typeof(c_TipoFactor), TtipoFactor, true),
                TasaOCuota = TtasaOCuota,
                Importe = (double)Math.Round(Timporte, 2)//6
            };
            return PartialView("~/Views/ComplementosPagos/TrasladoDR.cshtml", traslado);
        }

        public PartialViewResult AgregarDRetencion(Decimal Rbase, string Rimpuesto, string RtipoFactor, Decimal RtasaOCuota, Decimal Rimporte)
        {

            var retencion = new RetencionDR()
            {
                Base = (double)Math.Round(Rbase, 2), //6
                Impuesto = Rimpuesto,
                TipoFactor = (c_TipoFactor)Enum.Parse(typeof(c_TipoFactor), RtipoFactor, true),
                TasaOCuota = RtasaOCuota,
                Importe = (double)Math.Round(Rimporte, 2)//6
            };
            return PartialView("~/Views/ComplementosPagos/RetencionDR.cshtml", retencion);
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
            c_Pais? ResidenciaFiscalParse = null;
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
                ResidenciaFiscal = (API.Enums.c_Pais?)ResidenciaFiscalParse,
                NombreEstacion = NombreEstacion,
                NumEstacion = NumEstacion,
                Estaciones_Id = NumEstacion,
                NavegacionTrafico = NavegacionTrafico,
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

        public PartialViewResult AgregarMercancia(String ClaveProdServID, string ClaveProdSTCCID, string Descripcion,
            int Cantidad, string Unidad, string ClaveUnidadID, string Dimensiones, bool MaterialPeligorosoSN,
            string MaterialPeligrosoID, string DescripcionEmbalaje, string TipoEmbalajeID, Decimal PesoEnKg, string ValorMercancia,
            string Moneda, string FraccionArancelariaID, string UUIDComercioExt, string SectorCofepris, string NombreIngredienteActivo,
            string NomQuimico, string DenominacionGenericaProd, string DenominacionDistintivaProd, string Fabricante, DateTime? FechaCaducidad,
            string LoteMedicamento, string FormaFarmaceutica, string CondicionesEspecialesTransp, string RegistroSanitarioFolioAutorizacion,
            string PermisoImportacion, string FolioImpoVucem, string NumCas, string RazonSocialEmpImp, string NumRegSanPlagCofepris, string DatosFabricante,
            string DatosFormulador, string DatosMaquilador, string UsoAutorizado, string TipoMateria, string DescripcionMateria,
            string DEClaveUnidadPesoID, Decimal DEPesoBruto, Decimal DEPEsoNeto, Decimal DEPesoTara, int DENumPiezas,
            List<DocumentacionAduanera> DAduaneraArray, List<GuiasIdentificacion> GIdentificacionArray, List<CantidadTransportada> CTransportadaArray)
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
                FraccionArancelarias = FraccionArancelariaID,
                UUIDComecioExt = UUIDComercioExt,
                NombreIngredienteActivo = NombreIngredienteActivo,
                NomQuimico = NomQuimico,
                DenominacionGenericaProd = DenominacionGenericaProd,
                DenominacionDistintivaProd = DenominacionDistintivaProd,
                Fabricante = Fabricante,
                FechaCaducidad = FechaCaducidad,
                LoteMedicamento = LoteMedicamento,
                RegistroSanitarioFolioAutorizacion = RegistroSanitarioFolioAutorizacion,
                PermisoImportacion = PermisoImportacion,
                FolioImpoVucem = FolioImpoVucem,
                NumCas = NumCas,
                RazonSocialEmpImp = RazonSocialEmpImp,
                NumRegSanPlagCofepris = NumRegSanPlagCofepris,
                DatosFabricante = DatosFabricante,
                DatosFormulador = DatosFormulador,
                DatosMaquilador = DatosMaquilador,
                UsoAutorizado = UsoAutorizado,
                DescripcionMateria = DescripcionMateria,

                DetalleMercancia = new DetalleMercancia()
                {
                    ClaveUnidadPeso_Id = DEClaveUnidadPesoID,
                    PesoBruto = DEPesoBruto,
                    PesoNeto = DEPEsoNeto,
                    PesoTara = DEPesoTara,
                    NumPiezas = DENumPiezas
                }

            };
            //nuevo datos version 3.0

            if (SectorCofepris != "")
            {
                mercancia.SectorCofepris = (c_SectorCofepris)Enum.Parse(typeof(c_SectorCofepris), SectorCofepris, true);
            }
            else { mercancia.SectorCofepris = null; }
            if (FormaFarmaceutica != "")
            {
                mercancia.FormaFarmaceutica = (c_FormaFarmaceutica)Enum.Parse(typeof(c_FormaFarmaceutica), FormaFarmaceutica, true);
            }
            else { mercancia.FormaFarmaceutica = null; }
            if (CondicionesEspecialesTransp != "")
            {
                mercancia.CondicionesEspecialesTransp = (c_CondicionesEspeciales)Enum.Parse(typeof(c_CondicionesEspeciales), CondicionesEspecialesTransp, true);
            }
            else { mercancia.CondicionesEspecialesTransp = null; }
            if (TipoMateria != "")
            {
                mercancia.TipoMateria = (c_TipoMateria)Enum.Parse(typeof(c_TipoMateria), TipoMateria, true);
            }
            else { mercancia.TipoMateria = null; }
            if (DAduaneraArray != null)
            {
                mercancia.DocumentacionAduaneras = new List<DocumentacionAduanera>();
                foreach (var DA in DAduaneraArray)
                {
                    mercancia.DocumentacionAduaneras.Add(DA);
                }
            }
            if (GIdentificacionArray != null)
            {
                mercancia.GuiasIdentificacionss = new List<GuiasIdentificacion>();
                foreach (var GIdent in GIdentificacionArray)
                {
                    mercancia.GuiasIdentificacionss.Add(GIdent);
                }
            }
            if (CTransportadaArray != null)
            {
                mercancia.CantidadTransportadass = new List<CantidadTransportada>();
                foreach (var CTrans in CTransportadaArray)
                {
                    mercancia.CantidadTransportadass.Add(CTrans);
                }
            }
            return PartialView("~/Views/ComplementosCartaPorte/Mercancia.cshtml", mercancia);
        }

        public PartialViewResult AgregarMercanciaEdit(String ClaveProdServID, string ClaveProdSTCCID, string Descripcion,
            int Cantidad, string Unidad, string ClaveUnidadID, string Dimensiones, bool MaterialPeligorosoSN,
            string MaterialPeligrosoID, string DescripcionEmbalaje, string TipoEmbalajeID, Decimal PesoEnKg, string ValorMercancia,
            string Moneda, string FraccionArancelariaID, string UUIDComercioExt, string SectorCofepris, string NombreIngredienteActivo,
            string NomQuimico, string DenominacionGenericaProd, string DenominacionDistintivaProd, string Fabricante, DateTime? FechaCaducidad,
            string LoteMedicamento, string FormaFarmaceutica, string CondicionesEspecialesTransp, string RegistroSanitarioFolioAutorizacion,
            string PermisoImportacion, string FolioImpoVucem, string NumCas, string RazonSocialEmpImp, string NumRegSanPlagCofepris, string DatosFabricante,
            string DatosFormulador, string DatosMaquilador, string UsoAutorizado, string TipoMateria, string DescripcionMateria,
            string DEClaveUnidadPesoID, Decimal DEPesoBruto, Decimal DEPEsoNeto, Decimal DEPesoTara, int DENumPiezas,
            List<DocumentacionAduanera> DAduaneraArray, List<GuiasIdentificacion> GIdentificacionArray, List<CantidadTransportada> CTransportadaArray)
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
                FraccionArancelarias = FraccionArancelariaID,
                UUIDComecioExt = UUIDComercioExt,
                NombreIngredienteActivo = NombreIngredienteActivo,
                NomQuimico = NomQuimico,
                DenominacionGenericaProd = DenominacionGenericaProd,
                DenominacionDistintivaProd = DenominacionDistintivaProd,
                Fabricante = Fabricante,
                FechaCaducidad = FechaCaducidad,
                LoteMedicamento = LoteMedicamento,
                RegistroSanitarioFolioAutorizacion = RegistroSanitarioFolioAutorizacion,
                PermisoImportacion = PermisoImportacion,
                FolioImpoVucem = FolioImpoVucem,
                NumCas = NumCas,
                RazonSocialEmpImp = RazonSocialEmpImp,
                NumRegSanPlagCofepris = NumRegSanPlagCofepris,
                DatosFabricante = DatosFabricante,
                DatosFormulador = DatosFormulador,
                DatosMaquilador = DatosMaquilador,
                UsoAutorizado = UsoAutorizado,
                DescripcionMateria = DescripcionMateria,
                DetalleMercancia = new DetalleMercancia()
                {
                    ClaveUnidadPeso_Id = DEClaveUnidadPesoID,
                    PesoBruto = DEPesoBruto,
                    PesoNeto = DEPEsoNeto,
                    PesoTara = DEPesoTara,
                    NumPiezas = DENumPiezas
                }

            };
            //nuevo datos version 3.0
            if (SectorCofepris != "")
            {
                mercancia.SectorCofepris = (c_SectorCofepris)Enum.Parse(typeof(c_SectorCofepris), SectorCofepris, true);
            }
            else { mercancia.SectorCofepris = null; }
            if (FormaFarmaceutica != "")
            {
                mercancia.FormaFarmaceutica = (c_FormaFarmaceutica)Enum.Parse(typeof(c_FormaFarmaceutica), FormaFarmaceutica, true);
            }
            else { mercancia.FormaFarmaceutica = null; }
            if (CondicionesEspecialesTransp != "")
            {
                mercancia.CondicionesEspecialesTransp = (c_CondicionesEspeciales)Enum.Parse(typeof(c_CondicionesEspeciales), CondicionesEspecialesTransp, true);
            }
            else { mercancia.CondicionesEspecialesTransp = null; }
            if (TipoMateria != "")
            {
                mercancia.TipoMateria = (c_TipoMateria)Enum.Parse(typeof(c_TipoMateria), TipoMateria, true);
            }
            else { mercancia.TipoMateria = null; }
            if (DAduaneraArray != null)
            {
                mercancia.DocumentacionAduaneras = new List<DocumentacionAduanera>();
                foreach (var DA in DAduaneraArray)
                {
                    mercancia.DocumentacionAduaneras.Add(DA);
                }
            }
            if (GIdentificacionArray != null)
            {
                mercancia.GuiasIdentificacionss = new List<GuiasIdentificacion>();
                foreach (var GIdent in GIdentificacionArray)
                {
                    mercancia.GuiasIdentificacionss.Add(GIdent);
                }
            }
            if (CTransportadaArray != null)
            {
                mercancia.CantidadTransportadass = new List<CantidadTransportada>();
                foreach (var CTrans in CTransportadaArray)
                {
                    mercancia.CantidadTransportadass.Add(CTrans);
                }
            }
            return PartialView("~/Views/ComplementosCartaPorte/MercanciaEdit.cshtml", mercancia);
        }

        public PartialViewResult AgregarFTransporte(string FTransporte, string RFCFigura, string NumLicencia, string NombreFigura,
            string NumRegIdTribFigura, string ResidenciaFiscalFigura, string Pais, string PaisText, string Estado, string EstadoText, string Municipio, string MunicipioText, string Localidad,
            string LocalidadText, string CodigoPostal, string Colonia, string ColoniaText, string Calle, string NumExterior, string NumInterior, string Referencia, List<PartesTransporteDto> PartesTransporte)
        {

            var TiposFigura = new TiposFigura()
            {
                FiguraTransporte = FTransporte,
                RFCFigura = RFCFigura,
                NumLicencia = NumLicencia,
                NombreFigura = NombreFigura,
                NumRegIdTribFigura = NumRegIdTribFigura,
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
            if (ResidenciaFiscalFigura != null && ResidenciaFiscalFigura != "")
            {
                TiposFigura.ResidenciaFiscalFigura = (API.Enums.c_Pais?)(c_Pais)Enum.Parse(typeof(c_Pais), ResidenciaFiscalFigura, true);
            }
            else
            {
                TiposFigura.ResidenciaFiscalFigura = null;
            }
            //var listPartTrans = new List<PartesTransporte>();
            TiposFigura.PartesTransportes = new List<PartesTransporte>();
            if (PartesTransporte != null)
            {
                // var parteTransporte = new PartesTransporte();
                foreach (var parteTransporteDto in PartesTransporte)
                {
                    PartesTransporte PTransporte = new PartesTransporte()
                    {
                        ParteTransporte = (c_ParteTransporte)Enum.Parse(typeof(c_ParteTransporte), parteTransporteDto.ParteTransporte, true)
                    };

                    TiposFigura.PartesTransportes.Add(PTransporte);
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


        public PartialViewResult AgregarPTransporte(string PTransporte)
        {
            var PartesTransporte = new PartesTransporte()
            {
                ParteTransporte = (c_ParteTransporte)Enum.Parse(typeof(c_ParteTransporte), PTransporte, true)
            };
            return PartialView("~/Views/ComplementosCartaPorte/PartesTransporte.cshtml", PartesTransporte);
        }

        public PartialViewResult AgregarTMContenedor(string MatContenedor, string ContenedorMaritId, string NumPrecinto)
        {
            var ContenedorM = new ContenedorM()
            {
                MatriculaContenedor = MatContenedor,
                ContenedorMaritimo_Id = ContenedorMaritId,
                NumPrecinto = NumPrecinto
            };
            return PartialView("~/Views/ComplementosCartaPorte/ContenedoresM.cshtml", ContenedorM);
        }

        public PartialViewResult AgregarRemolqueCCP(string SubTipoRemCCP, string PlacaCCP)
        {
            var remolqueCCP = new RemolqueCCP()
            {
                SubTipoRemCCP = SubTipoRemCCP,
                PlacaCCP = PlacaCCP
            };
            return PartialView("~/Views/ComplementosCartaPorte/RemolqueCCP.cshtml", remolqueCCP);
        }
        public PartialViewResult AgregarTMContenedorEdit(string MatContenedor, string ContenedorMaritId, string NumPrecinto)
        {
            var ContenedorM = new ContenedorM()
            {
                MatriculaContenedor = MatContenedor,
                ContenedorMaritimo_Id = ContenedorMaritId,
                NumPrecinto = NumPrecinto
            };
            return PartialView("~/Views/ComplementosCartaPorte/ContenedoresMEdit.cshtml", ContenedorM);
        }

        public PartialViewResult AgregarTFDPaso(string TipoDPasoID, Decimal KilPagado)
        {
            var DerechosDePasos = new DerechosDePasos()
            {
                TipoDerechoDePaso_Id = TipoDPasoID,
                KilometrajePagado = KilPagado
            };

            return PartialView("~/Views/ComplementosCartaPorte/DerechosDePaso.cshtml", DerechosDePasos);
        }

        public PartialViewResult AgregarTFDPasoEdit(string TipoDPasoID, Decimal KilPagado)
        {
            var DerechosDePasos = new DerechosDePasos()
            {
                TipoDerechoDePaso_Id = TipoDPasoID,
                KilometrajePagado = KilPagado
            };

            return PartialView("~/Views/ComplementosCartaPorte/DerechoDePasoEdit.cshtml", DerechosDePasos);
        }

        public PartialViewResult AgregarTFCarro(string TipoCarroID, string MatriculaCarro, string GuiaCarro, Decimal TonNetasCarro, List<ContenedorC> contenedorC)
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
                foreach (var contenedor in contenedorC)
                {
                    Carro.ContenedoresC.Add(contenedor);
                }
            }

            return PartialView("~/Views/ComplementosCartaPorte/Carro.cshtml", Carro);
        }

        public PartialViewResult AgregarTFCarroEdit(string TipoCarroID, string MatriculaCarro, string GuiaCarro, Decimal TonNetasCarro, List<ContenedorC> contenedorC)
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
                foreach (var contenedor in contenedorC)
                {
                    Carro.ContenedoresC.Add(contenedor);
                }
            }

            return PartialView("~/Views/ComplementosCartaPorte/CarroEdit.cshtml", Carro);
        }
        public PartialViewResult AgregarTFContenedor(string ContenedorID, Decimal PesoContenedorVacio, Decimal pesoNetoMercancia)
        {
            var ContenedorC = new ContenedorC()
            {
                Contenedor_Id = ContenedorID,
                PesoContenedorVacio = PesoContenedorVacio,
                PesoNetoMercancia = pesoNetoMercancia
            };
            return PartialView("~/Views/ComplementosCartaPorte/ContenedoresC.cshtml", ContenedorC);
        }

        public PartialViewResult AgregarDocumentacionAduanera(string TipoDocumento, string NumPedimento, string IdentDocAduanero, string RfcImpo)
        {
            var documentacionAduanera = new DocumentacionAduanera()
            {
                TipoDocumento = (c_DocumentoAduanero)Enum.Parse(typeof(c_DocumentoAduanero), TipoDocumento, true),
                NumPedimento = NumPedimento,
                IdentDocAduanero = IdentDocAduanero,
                RfcImpo = RfcImpo
            };
            return PartialView("~/Views/ComplementosCartaPorte/DocumentacionAduanera.cshtml", documentacionAduanera);
        }

        public PartialViewResult AgregarGIdentificacion(string NumGuiIdentificacion, string DescripGuiaIdentificacion, Decimal PesoGuiaIdentificacion)
        {
            var GuiasIdent = new GuiasIdentificacion()
            {
                NumeroGuiaIdentificacion = NumGuiIdentificacion,
                DescripGuiaIdentificacion = DescripGuiaIdentificacion,
                PesoGuiaIdentificacion = PesoGuiaIdentificacion
            };
            return PartialView("~/Views/ComplementosCartaPorte/GuiasIdentificacion.cshtml", GuiasIdent);
        }

        public PartialViewResult AgregarCTransportadas(Decimal Cantidad, string CveTransporteID, string UbicacionOrigenID, string UbicacionDestinoID)
        {
            var CTransportadas = new CantidadTransportada()
            {
                Cantidad = Cantidad,
                CveTransporte_Id = CveTransporteID,
                IDOrigen = UbicacionOrigenID,
                IDDestino = UbicacionDestinoID
            };
            return PartialView("~/Views/ComplementosCartaPorte/CantidadTransportadas.cshtml", CTransportadas);
        }

        public PartialViewResult AgregarConceptos(string ClaveProdServID, string ClaveUnidadID, string Unidad, string Descripcion, string NumIdentificacion,
            string Cantidad, string ValorUnitario, string Importe, string ObjetoImpuesto, string TTipoImpuesto, Decimal TBase
            , string TImpuesto, string TTipoFactor, Decimal TTasaOCuota, Decimal TImporte, string RTipoImpuesto, Decimal RBase, string RImpuesto
            , string RTipofactor, Decimal RTasaOCuota, Decimal RImporte)
        {

            var Conceptos = new Conceptos()
            {
                ClavesProdServ = ClaveProdServID,
                ClavesUnidad = ClaveUnidadID,
                Unidad = Unidad,
                Descripcion = Descripcion,
                NoIdentificacion = NumIdentificacion,
                Cantidad = Cantidad,
                ValorUnitario = ValorUnitario,
                Importe = Convert.ToDouble(Importe),
                ObjetoImpuestoId = ObjetoImpuesto,
                Traslado = new TrasladoCP()
                {
                    TipoImpuesto = TTipoImpuesto,
                    Base = TBase,
                    Impuesto = TImpuesto,
                    TipoFactor = (API.Enums.CartaPorteEnums.c_TipoFactor)(c_TipoFactor)Enum.Parse(typeof(c_TipoFactor), TTipoFactor, true),
                    TasaOCuota = TTasaOCuota,
                    Importe = TImporte,
                    // TotalImpuestosTR = TTImpuestosTR,
                },
                Retencion = new RetencionCP()
                {
                    TipoImpuesto = RTipoImpuesto,
                    Base = RBase,
                    Impuesto = RImpuesto,
                    TipoFactor = (API.Enums.CartaPorteEnums.c_TipoFactor)(c_TipoFactor)Enum.Parse(typeof(c_TipoFactor), RTipofactor, true),
                    TasaOCuota = RTasaOCuota,
                    Importe = RImporte,
                    //TotalImpuestosTR = RTImpuestoTR
                }
            };
            return PartialView("~/Views/ComplementosCartaPorte/Conceptos.cshtml", Conceptos);
        }

        public PartialViewResult AgregarConceptosComprobante(string ClaveProdServID, string ClaveUnidadID, string Unidad, string Descripcion, string NumIdentificacion,
            string Cantidad, string ValorUnitario, string Importe, string ObjetoImpuesto, string TTipoImpuesto, Decimal TBase
            , string TImpuesto, string TTipoFactor, Decimal TTasaOCuota, Decimal TImporte, string RTipoImpuesto, Decimal RBase, string RImpuesto
            , string RTipofactor, Decimal RTasaOCuota, Decimal RImporte)
        {

            var Conceptos = new Conceptos()
            {
                ClavesProdServ = ClaveProdServID,
                ClavesUnidad = ClaveUnidadID,
                Unidad = Unidad,
                Descripcion = Descripcion,
                NoIdentificacion = NumIdentificacion,
                Cantidad = Cantidad,
                ValorUnitario = ValorUnitario,
                Importe = Convert.ToDouble(Importe),
                ObjetoImpuestoId = ObjetoImpuesto,
                Traslado = new TrasladoCP()
                {
                    TipoImpuesto = TTipoImpuesto,
                    Base = TBase,
                    Impuesto = TImpuesto,
                    TipoFactor = (API.Enums.CartaPorteEnums.c_TipoFactor)(c_TipoFactor)Enum.Parse(typeof(c_TipoFactor), TTipoFactor, true),
                    TasaOCuota = TTasaOCuota,
                    Importe = TImporte,
                    // TotalImpuestosTR = TTImpuestosTR,
                },
                Retencion = new RetencionCP()
                {
                    TipoImpuesto = RTipoImpuesto,
                    Base = RBase,
                    Impuesto = RImpuesto,
                    TipoFactor = (API.Enums.CartaPorteEnums.c_TipoFactor)(c_TipoFactor)Enum.Parse(typeof(c_TipoFactor), RTipofactor, true),
                    TasaOCuota = RTasaOCuota,
                    Importe = RImporte,
                    //TotalImpuestosTR = RTImpuestoTR
                }
            };
            return PartialView("~/Views/ComprobantesCfdi/Concepto.cshtml", Conceptos);
        }

        public PartialViewResult AgregarRemolque(string Placa, string TipoRemolqueId, string TipoRemolque)
        {
            var remolques = new Remolques()
            {
                Placa = Placa,
                SubTipoRem_Id = TipoRemolqueId,
                SubTipoRems = TipoRemolque

            };
            return PartialView("~/Views/ComplementosCartaPorte/Remolques.cshtml", remolques);
        }

        public PartialViewResult AgregarRemolqueEdit(string Placa, string TipoRemolqueId, string TipoRemolque)
        {
            var remolques = new Remolques()
            {
                Placa = Placa,
                SubTipoRem_Id = TipoRemolqueId,
                SubTipoRems = TipoRemolque

            };
            return PartialView("~/Views/ComplementosCartaPorte/RemolquesEdit.cshtml", remolques);
        }
        public int Buscar(string valor, String tipo)
        {

            var busqueda = 0;
            if (tipo.Equals("serv"))
            {
                busqueda = _db.ClavesProdServCP.Where(a => a.c_ClaveUnidad.Equals(valor)).Count();
            }
            if (tipo.Equals("claveServ"))
            {
                busqueda = _db.claveProdServ.Where(a => a.c_ClaveUnidad.Equals(valor)).Count();
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

        public JsonResult GetMontoPago(int PagoId)
        {
            var pago = _db.Pagos.Find(PagoId);
            List<PagosDto> listPagos = new List<PagosDto>();
            PagosDto pagoDto = new PagosDto();
            if (pago != null)
            {
                pagoDto.Monto = pago.Monto;
                pagoDto.TipoCambio = pago.TipoCambio;
                listPagos.Add(pagoDto);
            }
            return Json(listPagos, JsonRequestBehavior.AllowGet);
        }

        #region PopulaForma

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }



        #endregion

    }
}