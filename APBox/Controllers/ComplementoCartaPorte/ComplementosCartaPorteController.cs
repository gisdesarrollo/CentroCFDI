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
using Aplicacion.LogicaPrincipal.Acondicionamientos.Operaciones;
using System.Data.Entity.Validation;
using API.RelacionesCartaPorte;
using System.Data.Entity;
using API.Models.Dto;
using System.Net;
using Aplicacion.LogicaPrincipal.GeneracionComplementoCartaPorte;
using API.Enums.CartaPorteEnums;

namespace APBox.Controllers.ComplementosCartaPorte
{
    [SessionExpire]
    public class ComplementosCartaPorteController : Controller
    {
        // GET: ComplementosCartaPorte
        private readonly AplicacionContext _db = new AplicacionContext();
        private readonly AcondicionarComplementosCartaPorte _acondicionarComplementosCartaPorte = new AcondicionarComplementosCartaPorte();
        private readonly CartaPorteManager _cartaPorteManager = new CartaPorteManager();

        public ActionResult Index()
        {
            var ComplementoCartaPorteModel = new ComplementosCartaPorteModel()
            {
                Mes = (Meses)(DateTime.Now.Month),
                Anio = DateTime.Now.Year
            };
            var fechaInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            var fechaFinal = DateTime.Now;
            ComplementoCartaPorteModel.ComplementosCartaPorte = _acondicionarComplementosCartaPorte.Filtrar(fechaInicial, fechaFinal, ObtenerSucursal());

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
            PopulaTiposUbicacion();
            PopulaFiguraTransporte();
            PopulaNavegacionTrafico();
            PopulaDerechoPaso();

            
            TipoEmbalaje_Id();
           
            ClaveUnidadPeso_Id();
            SubTipoRem_Id();
            ConfigMaritima_Id();
            TipoPermiso_Id();
            ConfigAutotransporte_Id();
            ClaveTipoCarga_Id();
            NumAutorizacionNaviero_Id();
            ContenedorMaritimo_Id();
            CodigoTransporteAereo_Id();
            TipoDeServicio_Id();
            TipoCarro_Id();
            Contenedor_Id();
           
            PopulaTiPermiso();
            Random random = new Random();
            var randomNumber = random.Next(0,1000000).ToString("D6");
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
                Conceptos = new Conceptos()
                {
                    Traslado = new SubImpuestoC()
                    {
                        TipoImpuesto = "Traslado",
                        Impuesto = c_ImpuestoCP.Iva,
                        TipoFactor = c_TipoFactor.Tasa,
                        Base = 0,
                        TasaOCuota = 0,
                        Importe = 0
                    },
                    Retencion = new SubImpuestoC()
                    {
                        TipoImpuesto = "Retencion",
                        Impuesto = c_ImpuestoCP.Iva,
                        TipoFactor = c_TipoFactor.Tasa,
                        Base = 0,
                        TasaOCuota = 0,
                        Importe = 0
                    }

                },
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
                    IDUbicacion = "OR"+randomNumber,
                    DistanciaRecorrida = 0,
                    TipoUbicacion = "Origen",
                    FechaHoraSalidaLlegada = DateTime.Now,
                    
                    Domicilio = new Domicilio
                    {

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
        //[ValidateAntiForgeryToken]
        public ActionResult Create(ComplementoCartaPorte complementoCartaPorte)
        {
            ModelState.Remove("Mercancias.AutoTransporte.IdentificacionVehicular.AnioModeloVM");
            ModelState.Remove("Mercancias.TransporteAereo.ResidenciaFiscalEmbarc");
            ModelState.Remove("SucursalId");
            ModelState.Remove("ReceptorId");
            ModelState.Remove("FacturaEmitidaId");
            ModelState.Remove("CfdiRelacionadoId");
            ModelState.Remove("Receptor.RazonSocial");
            ModelState.Remove("Receptor");
            ModelState.Remove("Sucursal.RazonSocial");
           // PopulaClientes(complementoCartaPorte.ReceptorId);
            PopulaBancos(ObtenerSucursal());
            //PopulaCfdiRelacionado(complementoCartaPorte.CfdiRelacionadoId);
            PopulaTiposDeComprobante();
            PopulaTransporte();
            PopulaTiposEstacion();
            PopulaDatosSucursal(ObtenerSucursal());
            PopulaDatosEstaciones();
            PopulaPaises();
            PopularUsoCfdi();
            PopulaTiposUbicacion();
            PopulaFiguraTransporte();
            PopulaNavegacionTrafico();
            PopulaDerechoPaso();
            
            TipoEmbalaje_Id();
            
            ClaveUnidadPeso_Id();
            SubTipoRem_Id();
            ConfigMaritima_Id();
            TipoPermiso_Id();
            ConfigAutotransporte_Id();
            ClaveTipoCarga_Id();
            NumAutorizacionNaviero_Id();
            ContenedorMaritimo_Id();
            CodigoTransporteAereo_Id();
            TipoDeServicio_Id();
            TipoCarro_Id();
            Contenedor_Id();
            PopulaTiPermiso();

            if (!ModelState.IsValid)
            {
                //Identifica los mensaje de error
                var errors = ModelState.Values.Where(E => E.Errors.Count > 0)
                         .SelectMany(E => E.Errors)
                         .Select(E => E.ErrorMessage)
                         .ToList();
                //Identifica el campo del Required
                var modelErrors = ModelState.Where(m => ModelState[m.Key].Errors.Any());
                ModelState.AddModelError("", "Error revisar los campos requeridos");
                complementoCartaPorte.Ubicacion = new Ubicacion()
                {

                    Domicilio = new Domicilio()
                    {
                        Pais = complementoCartaPorte.Ubicacion.Domicilio.Pais
                    }
                };

                complementoCartaPorte.TiposFigura.PartesTransporte.Domicilio = new Domicilio();
                return View(complementoCartaPorte);
            }
            //Modelstate True
            try
            {
                
               
                
                if (complementoCartaPorte.TipoDeComprobante == c_TipoDeComprobante.T)
                {
                    complementoCartaPorte.hidden = false;
                    complementoCartaPorte.Moneda = c_Moneda.XXX;
                    complementoCartaPorte.Subtotal = 0;
                    complementoCartaPorte.Total = 0;
                    //carga conceptos
                   _acondicionarComplementosCartaPorte.cargaConceptos(ref complementoCartaPorte);
                }
                else
                {
                    complementoCartaPorte.hidden = true;
                    //carga conceptos
                    _acondicionarComplementosCartaPorte.cargaConceptos(ref complementoCartaPorte);
                }
                complementoCartaPorte.TotalDistRec = 0;
                //carga Complemento y Transporte
                _acondicionarComplementosCartaPorte.cargaTransporte(ref complementoCartaPorte);
               
                //carga ubicaciones
                _acondicionarComplementosCartaPorte.cargaUbicaciones(ref complementoCartaPorte);
                //carga Mercancias
                _acondicionarComplementosCartaPorte.cargaMercancias(ref complementoCartaPorte);
                
                //carga Figura Transporte
                _acondicionarComplementosCartaPorte.cargaFiguraTransporte(ref complementoCartaPorte);
                
                
                if (complementoCartaPorte.Conceptoss != null)
                {
                    foreach(var concepto in complementoCartaPorte.Conceptoss)
                    {
                        var relCartaPorteConceptos = new ComplementoCartaPorteConceptos()
                        {
                            ComplementoCartaPorte_Id = complementoCartaPorte.Id,
                            Conceptos_Id = concepto.Id
                        };
                    }
                }
                if (complementoCartaPorte.Ubicaciones != null)
                {
                    
                    foreach (var ubicacion in complementoCartaPorte.Ubicaciones)
                    {
                        var relCartaPorteUbicacion = new ComplementoCartaPorteUbicacion()
                        {
                            ComplementoCartaPorte_Id = complementoCartaPorte.Id,
                            Ubicacion_Id = ubicacion.Id
                        };
                        _db.ComplementoCartaPorteUbicaciones.Add(relCartaPorteUbicacion);
                        _db.SaveChanges();
                    }
                }
                if (complementoCartaPorte.FiguraTransporte != null)
                {
                    foreach(var FT in complementoCartaPorte.FiguraTransporte)
                    {
                        var relCartaPorteFTransporte = new ComplementoCartaPorteFiguraTransporte()
                        {
                            ComplementoCartaPorte_Id = complementoCartaPorte.Id,
                            FiguraTransporte_Id = FT.Id
                        };
                        _db.ComplementoCartaPorteFiguraTransporte.Add(relCartaPorteFTransporte);
                        _db.SaveChanges();
                    }
                }
               

                return RedirectToAction("Index");
            }
            catch(DbEntityValidationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                /*foreach (var eve in ex.EntityValidationErrors)
                {
                    /*Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);*/

                /* foreach (var ve in eve.ValidationErrors)
                 {
                     ModelState.AddModelError("", String.Format("- Property: \"{0}\", Error: \"{1}\"",
                         ve.PropertyName, ve.ErrorMessage));

                     /*Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                         ve.PropertyName, ve.ErrorMessage);*/
                /*}
            }*/


            }
            complementoCartaPorte.TiposFigura.PartesTransporte.Domicilio = new Domicilio();
            return View(complementoCartaPorte);
        }

        public ActionResult Generar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComplementoCartaPorte complementoCartaPorte = _db.ComplementoCartaPortes.Find(id);
            if (complementoCartaPorte == null)
            {
                return HttpNotFound();
            }
            //PopulaClientes(complementoPago.ReceptorId);
            return View(complementoCartaPorte);
        }

        [HttpPost]
        public ActionResult Generar(ComplementoCartaPorte complementoCartaPorte)
        {
            //PopulaClientes(complementoCartaPorte.ReceptorId);

            if (ModelState.IsValid)
            {
                try
                {
                    var sucursalId = ObtenerSucursal();

                    //Actualizacion Receptor

                    DateTime fechaDoc = complementoCartaPorte.FechaDocumento;
                    var horaHoy = DateTime.Now;
                    var fechaTime = new DateTime(fechaDoc.Year, fechaDoc.Month, fechaDoc.Day, horaHoy.Hour, horaHoy.Minute, horaHoy.Second);

                    var complementoCartaPorteDb = _db.ComplementoCartaPortes.Find(complementoCartaPorte.Id);
                    complementoCartaPorteDb.rfcReceptor = complementoCartaPorte.rfcReceptor;
                    complementoCartaPorteDb.FechaDocumento = fechaTime;
                    _db.Entry(complementoCartaPorteDb).State = EntityState.Modified;
                    _db.SaveChanges();

                    _cartaPorteManager.GenerarComplementoCartaPorte(sucursalId, complementoCartaPorte.Id, "");
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(complementoCartaPorte);
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

        public JsonResult FiltrarUbicaciones(List<UbicacionDropDowDto> ListUbicacionClave)
        {
            var clavesUbicacion = ListUbicacionClave;
            return Json(clavesUbicacion, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DatosCliente(int ClienteId)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            var Estados = popularDropDowns.PopulaDatosCliente(ClienteId);
            return Json(Estados, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DatosClaveUnidad(string ClaveUnidad)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            var Clave = popularDropDowns.PopulaDatosClaveUnidad(ClaveUnidad);
            return Json(Clave, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DatosClaveProdServCP(string ClaveProdServCP)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            var Clave = popularDropDowns.PopulaDatosClaveProdCP(ClaveProdServCP);
            return Json(Clave, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DatosFraccionArancelaria(string ClaveFraccion)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            var Clave = popularDropDowns.PopulaDatosFraccionArancelaria(ClaveFraccion);
            return Json(Clave, JsonRequestBehavior.AllowGet);
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

        private void PopulaClaveUnidad()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.ClaveUnidadC = (popularDropDowns.PopulaClaveUnidad());
        }
        private void PopulaClaveProdServCP()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.ClaveProdServ = (popularDropDowns.PopulaClaveProdServ());
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

        private void PopulaDerechoPaso()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.TipoDerechoPaso = (popularDropDowns.PopulaDerechodePaso());
        }

        private void PopulaTiposDeComprobante()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.TipoDeComprobante = (popularDropDowns.PopulaTipoDeComprobante());
        }

        private void PopulaTiposUbicacion()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Origen", Value = "Origen", Selected = true });
            items.Add(new SelectListItem { Text = "Destino", Value = "Destino" });
            ViewBag.TipoUbicacion = items;
        }

        private void PopulaNavegacionTrafico()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Altura", Value = "Altura", Selected = true });
            items.Add(new SelectListItem { Text = "Cabotaje", Value = "Cabotaje" });
            ViewBag.TipoNavegacionTrafico = items;
        }

        private void PopulaFiguraTransporte()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Operador", Value = "01", Selected = true });
            items.Add(new SelectListItem { Text = "Propietario", Value = "02" });
            items.Add(new SelectListItem { Text = "Arrendador", Value = "03" });
            items.Add(new SelectListItem { Text = "Notificado", Value = "04" });
            ViewBag.TipoFiguraTransporte = items;
        }

        private void TipoPermiso_Id()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.TipoPermisoMaritimo = (popularDropDowns.TipoPermiso_Id("02"));
            ViewBag.TipoPermisoAereo = (popularDropDowns.TipoPermiso_Id("03"));
            ViewBag.TipoPermisoAuto = (popularDropDowns.TipoPermiso_Id("01"));
            ViewBag.TipoPermiso_M = ViewBag.TipoPermisoMaritimo;
            ViewBag.TipoPermiso_A = ViewBag.TipoPermisoAereo;
            ViewBag.TipoPermiso_AT = ViewBag.TipoPermisoAuto;
        }
        private void PopulaTiPermiso()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.TipoPermisoList = (popularDropDowns.PopulaTipoPermiso());
        }
        private void PopulaTransporte()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.ClavesTransporte = (popularDropDowns.PopulaTransporte());
        }
        //evelio dropdopw

        private void SubTipoRem_Id()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.SubTipoRem_Id = popularDropDowns.SubTipoRem_Id();
        }
        
        private void TipoEmbalaje_Id()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.TiposEmbalaje = (popularDropDowns.TipoEmbalaje_Id());
        }
        
        private void ClaveUnidadPeso_Id()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.ClavesUnidadPeso = (popularDropDowns.ClaveUnidadPeso_Id());
        }

       
        private void ConfigMaritima_Id()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.ConfigMaritima_Id = popularDropDowns.ConfigMaritima_Id();
        }
       

        private void ConfigAutotransporte_Id()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.ConfigAutotransporte_Id = popularDropDowns.ConfigAutotransporte_Id();
        }
        private void ClaveTipoCarga_Id()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.ClaveTipoCarga_Id = popularDropDowns.ClaveTipoCarga_Id();
        }
        private void NumAutorizacionNaviero_Id()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.NumAutorizacionNaviero_Id = popularDropDowns.NumAutorizacionNaviero_Id();
        }
        private void ContenedorMaritimo_Id()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.ContenedorMaritimo_Id = popularDropDowns.ContenedorMaritimo_Id();
        }
        private void CodigoTransporteAereo_Id()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.CodigoTransporteAereo_Id = popularDropDowns.CodigoTransporteAereo_Id();
        }
        private void TipoDeServicio_Id()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.TipoDeServicio_Id = popularDropDowns.TipoDeServicio_Id();
        }
        private void TipoCarro_Id()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.TipoCarro_Id = popularDropDowns.TipoCarro_Id();
        }
        private void Contenedor_Id()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.Contenedor_Id = popularDropDowns.Contenedor_Id();
        }
        //
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