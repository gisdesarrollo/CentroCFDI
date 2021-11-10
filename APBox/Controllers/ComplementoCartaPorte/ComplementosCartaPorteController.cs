using System;
using API.Models.Operaciones;
using API.Operaciones.ComplementoCartaPorte;
using CFDI.API.Enums.CFDI33;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using API.Enums;
using Aplicacion.Context;
using APBox.Control;

namespace APBox.Controllers.ComplementosCartaPorte
{
    [SessionExpire]
    public class ComplementosCartaPorteController : Controller
    {
        // GET: ComplementosCartaPorte
        private readonly AplicacionContext _db = new AplicacionContext();
        public ActionResult Index()
        {
            var ComplementoCartaPorteModel = new ComplementosCartaPorteModel()
            {
                Mes = (Meses)(DateTime.Now.Month),
                Anio = DateTime.Now.Year
            };

            return View(ComplementoCartaPorteModel);
        }

        public ActionResult Create()
        {
            PopulaClientes();
            PopulaBancos(ObtenerSucursal());
            PopulaCfdiRelacionado();

            PopulaTiposDeComprobante();
            PopulaTransporte();
            PopulaTiposEstacion();

            PopulaDatosSucursal(ObtenerSucursal());
            PopulaDatosEstaciones();

            PopulaPaises();
            PopularUsoCfdi();

            var ComplementoCartaPorte = new ComplementoCartaPorte()
            {
                Generado = false,
                Status = Status.Activo,
                FechaDocumento = DateTime.Now,
                Mes = (Meses)Enum.ToObject(typeof(Meses), DateTime.Now.Month),
                SucursalId = ObtenerSucursal(),
                Version = "2.0",
                TotalDistRec = 0,
                Moneda = c_Moneda.MXN,
                hidden = false,
                Mercancias = new Mercancias
                {
                    Mercancia = new Mercancia()
                    {
                        Moneda = c_Moneda.MXN,
                        Cantidad = 0,
                        PesoEnKg = 0,
                        DetalleMercancia = new DetalleMercancia()
                        {
                            NumPiezas = 0,
                            PesoBruto = 0,
                            PesoNeto = 0,
                            PesoTara = 0
                        },
                        CantidadTransportada = new CantidadTransportada()
                        {
                            Cantidad = 0
                        },
                        GuiasIdentificacion = new GuiasIdentificacion()
                        {
                            PesoGuiaIdentificacion = 0
                        }
                    },
                    TransporteFerroviario = new TransporteFerroviario()
                    {
                        DerechosDePasos = new DerechosDePasos()
                        {
                            KilometrajePagado = 0
                        },
                        Carro = new Carro()
                        {
                            ToneladasNetasCarro = 0,
                            ContenedorC = new ContenedorC()
                            {
                                PesoContenedorVacio = 0,
                                PesoNetoMercancia = 0
                            }
                        }
                    },
                    TransporteMaritimo = new TransporteMaritimo()
                    {
                        ContenedorM = new ContenedorM() { }
                    }
                    
                },
                
                Ubicacion = new Ubicacion
               
                {
                    UbicacionOrigen = new UbicacionOrigen
                    {
                        Sucursal_Id = ObtenerSucursal(),
                        RfcRemitente = ViewBag.DatosSucursal.Items[0].Rfc,
                        NombreRemitente = ViewBag.DatosSucursal.Items[0].Nombre,
                        ResidenciaFiscal = ViewBag.DatosSucursal.Items[0].Pais,
                        FechaHoraSalida = DateTime.Now,
                        IDUbicacionOrigen = "OR",
                        Domicilio = new Domicilio
                        {

                        }
                    },

                    UbicacionDestino = new UbicacionDestino
                    {
                        FechaHoraLlegada = DateTime.Now,
                        IDUbicacionDestino = "DE",
                        Domicilio  = new Domicilio
                        {

                        }
                    }
                },
                TiposFigura = new TiposFigura()
                {
                    PartesTransporte = new PartesTransporte()
                    {
                        Domicilio = new Domicilio()
                        {

                        }
                    }
                }
            };
            return View(ComplementoCartaPorte);
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public ActionResult Create(ComplementoCartaPorte complementoCartaPorte)
        {
            PopulaClientes(complementoCartaPorte.ReceptorId);
            PopulaBancos(ObtenerSucursal());
            PopulaCfdiRelacionado(complementoCartaPorte.CfdiRelacionadoId);

            PopulaTiposDeComprobante();
            PopulaTransporte();
            PopulaTiposEstacion();

            PopulaDatosSucursal(ObtenerSucursal());
            PopulaDatosEstaciones();

            PopulaPaises();
            PopularUsoCfdi();
            if (ModelState.IsValid)
            {
                if (complementoCartaPorte.TipoDeComprobante == c_TipoDeComprobante.T)
                {
                    complementoCartaPorte.Moneda = null;
                    complementoCartaPorte.Subtotal = 0;
                    complementoCartaPorte.Total = 0;
                }
                return RedirectToAction("Index"); ;
            }
            if (complementoCartaPorte.TipoDeComprobante == c_TipoDeComprobante.I){complementoCartaPorte.hidden = true;}
            else{
                complementoCartaPorte.hidden = false;
            }
            
            complementoCartaPorte.Ubicacion = new Ubicacion() {
                    UbicacionOrigen = new UbicacionOrigen() {
                    Sucursal_Id = ObtenerSucursal(),
                    RfcRemitente = ViewBag.DatosSucursal.Items[0].Rfc,
                    NombreRemitente = ViewBag.DatosSucursal.Items[0].Nombre,
                    ResidenciaFiscal = ViewBag.DatosSucursal.Items[0].Pais,
                    Domicilio = new Domicilio()
                    {

                    }
                },
                UbicacionDestino = new UbicacionDestino()
                {
                    Domicilio = new Domicilio()
                    {

                    }
                }

            };

            complementoCartaPorte.TiposFigura = new TiposFigura()
            {
                PartesTransporte = new PartesTransporte()
                {
                    Domicilio = new Domicilio()
                    {

                    }
                }
            };

            return View(complementoCartaPorte);
        }

        public void serealizaJson(ComplementoCartaPorte complementoCartaPorte)
        {

        }

            public JsonResult FiltrarEstados(string PaisId)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            var Estados = popularDropDowns.PopulaEstados(PaisId);
            return Json(Estados,JsonRequestBehavior.AllowGet);
        }

        public JsonResult FiltrarMunicipios(string EstadoId)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            var Estados = popularDropDowns.PopulaMunicipios(EstadoId);
            return Json(Estados, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FiltrarLocalidades(string EstadoId)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            var Estados = popularDropDowns.PopulaLocalidades(EstadoId);
            return Json(Estados, JsonRequestBehavior.AllowGet);
        }

        public JsonResult FiltrarColonias(string CodigoPostalId)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            var Estados = popularDropDowns.PopulaColonias(CodigoPostalId);
            return Json(Estados, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DatosCliente(int ClienteId)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            var Estados = popularDropDowns.PopulaDatosCliente(ClienteId);
            return Json(Estados, JsonRequestBehavior.AllowGet);
        }


        #region Popula Forma

        private int ObtenerGrupo()
        {

            return Convert.ToInt32(Session["GrupoId"]);
        }
        private int ObtenerSucursal()
        {
                return Convert.ToInt32(Session["SucursalId"]);
        }
        #endregion

        #region Popula CartaPorte

        private void PopulaPaises()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.Paises = (popularDropDowns.PopulaPaises());
        }

        private void PopularUsoCfdi()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.UsoCfdi = (popularDropDowns.PopularUsocfdi());
        }

        private void PopulaDatosEstaciones()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.NombreEstacionMaritimo = (popularDropDowns.PopulaDatosEstaciones("02"));
            ViewBag.NombreEstacionAereo = (popularDropDowns.PopulaDatosEstaciones("03"));
            ViewBag.NombreEstacionFerroviario = (popularDropDowns.PopulaDatosEstaciones("04"));
            ViewBag.NombreEstacionMaritimoD = ViewBag.NombreEstacionMaritimo;
            ViewBag.NombreEstacionAereoD = ViewBag.NombreEstacionAereo;
            ViewBag.NombreEstacionFerroviarioD = ViewBag.NombreEstacionFerroviario;
        }

        private void PopulaDatosSucursal(int SucursalId)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.DatosSucursal = (popularDropDowns.PopulaDatosSucursal(SucursalId));
        }

        private void PopulaTiposEstacion()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.TipoEstacion = (popularDropDowns.PopulaTiposEstacion());
        }

        private void PopulaTiposDeComprobante()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.TipoDeComprobante = (popularDropDowns.PopulaTipoDeComprobante());
        }

        private void PopulaTransporte()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.ClavesTransporte = (popularDropDowns.PopulaTransporte());
        }
        #endregion

        #region Popula CFDI
        private void PopulaBancos(int sucursalId, int? bancoReceptorId = null, int? bancoEmisorId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerGrupo(), true);

            ViewBag.BancoOrdenanteId = popularDropDowns.PopulaBancosClientes(0, bancoReceptorId);
            ViewBag.BancoBeneficiarioId = popularDropDowns.PopulaBancosSucursales(sucursalId, bancoEmisorId);
        }

        private void PopulaClientes(int? receptorId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);

            ViewBag.ReceptorId = popularDropDowns.PopulaClientes(receptorId);
        }

        private void PopulaCfdiRelacionado(int? cfdiRelacionadoId = null)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);

            ViewBag.CfdiRelacionadoId = popularDropDowns.PopulaFacturasEmitidas(false, 0, cfdiRelacionadoId);
        }
        #endregion
    }
}