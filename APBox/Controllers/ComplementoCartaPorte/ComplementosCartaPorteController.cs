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
using Aplicacion.LogicaPrincipal.Facturas.Timbrado;
using System.ServiceModel;
using System.Xml;
using System.Web.Services.Description;
using System.Xml.Serialization;
using System.IO;

namespace APBox.Controllers.ComplementosCartaPorte
{
    [SessionExpire]
    public class ComplementosCartaPorteController : Controller
    {
        // GET: ComplementosCartaPorte
        private readonly AplicacionContext _db = new AplicacionContext();
        private readonly AcondicionarComplementosCartaPorte _acondicionarComplementosCartaPorte = new AcondicionarComplementosCartaPorte();
        private readonly CartaPorteManager _cartaPorteManager = new CartaPorteManager();
        private readonly Timbrar _TimbrarCFDI = new Timbrar();

        public ActionResult Index()
        {
            ViewBag.Mensaje = null;
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

        [HttpPost]
        public ActionResult Index(ComplementosCartaPorteModel complementosCPorteModel, string actionName)
        {
            if (actionName == "Filtrar")
            {
                var dia = DateTime.DaysInMonth(complementosCPorteModel.Anio, (int)complementosCPorteModel.Mes);

                var fechaInicial = new DateTime(complementosCPorteModel.Anio, (int)complementosCPorteModel.Mes, 1, 0, 0, 0);
                var fechaFinal = new DateTime(complementosCPorteModel.Anio, (int)complementosCPorteModel.Mes, dia, 23, 59, 59);

                complementosCPorteModel.ComplementosCartaPorte = _acondicionarComplementosCartaPorte.Filtrar(fechaInicial, fechaFinal, ObtenerSucursal());
            }
            return View(complementosCPorteModel);
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
            PopulaEntradaSalidaMerc();

            PopulaConceptos();
            PopulaImpuestoR();
            PopulaImpuestoT();
            PopulaImpuestoSat();
            PopulaFormaPago();
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
                    Traslado = new TrasladoCP()
                    {
                        TipoImpuesto = "Traslado",
                        //Impuesto = c_Impuesto.Iva,
                        TipoFactor = c_TipoFactor.Tasa,
                        Base = 0,
                        TasaOCuota = 0,
                        Importe = 0
                    },
                    Retencion = new RetencionCP()
                    {
                        TipoImpuesto = "Retencion",
                        //Impuesto = c_Impuesto.Iva,
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
                    Domicilio = new Domicilio(){},
                    PartesTransporte = new PartesTransporte(){}
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
            PopulaClientes(complementoCartaPorte.ReceptorId);
            
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
            PopulaEntradaSalidaMerc();
            PopulaConceptos();
            PopulaImpuestoR();
            PopulaImpuestoT();
            PopulaImpuestoSat();
            PopulaFormaPago();
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

                complementoCartaPorte.TiposFigura.Domicilio = new Domicilio();
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
                    complementoCartaPorte.UsoCfdi = c_UsoCfdiCP.P01;
                    complementoCartaPorte.FormaPago = null;
                    complementoCartaPorte.MetodoPago = null;
                    complementoCartaPorte.TipoCambio = null;
                    complementoCartaPorte.CondicionesPago = null;
                    
                }
                else
                {
                    complementoCartaPorte.hidden = true;
                   
                }
                complementoCartaPorte.TotalDistRec = 0;
                _acondicionarComplementosCartaPorte.CargaInicial(ref complementoCartaPorte);
                
                
                    //copia conceptos
                    var conceptos = complementoCartaPorte.Conceptoss;
                    conceptos.ForEach(c => c.ComplementoCP = null);
                    //copia ubicaciones
                    var ubicaciones = complementoCartaPorte.Ubicaciones;
                    ubicaciones.ForEach(u => u.ComplementoCP = null);
                    //copia mercancia - mercancia
                    
                 var mercancias = complementoCartaPorte.Mercancias.Mercanciass;
                Mercancia[] arraysDataMercancia = new Mercancia[10];
                if (complementoCartaPorte.Mercancias.Mercanciass.Count > 0)
                {
                    var totalMercancia = complementoCartaPorte.Mercancias.Mercanciass.Count;
                    
                    for (int x = 0; x < complementoCartaPorte.Mercancias.Mercanciass.Count; x++)
                    {
                        arraysDataMercancia[x] = new Mercancia();
                        arraysDataMercancia[x].Pedimentoss = new List<Pedimentos>();
                        arraysDataMercancia[x].GuiasIdentificacionss = new List<GuiasIdentificacion>();
                        arraysDataMercancia[x].CantidadTransportadass = new List<CantidadTransportada>();
                        arraysDataMercancia[x].Pedimentoss = complementoCartaPorte.Mercancias.Mercanciass[x].Pedimentoss;
                        arraysDataMercancia[x].GuiasIdentificacionss = complementoCartaPorte.Mercancias.Mercanciass[x].GuiasIdentificacionss;
                        arraysDataMercancia[x].CantidadTransportadass = complementoCartaPorte.Mercancias.Mercanciass[x].CantidadTransportadass;
                    }
                }
                    mercancias.ForEach(m => m.Mercancias = null);


                //copia autotransporte -remolque
                List<Remolques> remolques = new List<Remolques>();
                if (complementoCartaPorte.Mercancias.AutoTransporte != null)
                {
                    if (complementoCartaPorte.Mercancias.AutoTransporte.Remolquess.Count > 0)
                    {
                        remolques = complementoCartaPorte.Mercancias.AutoTransporte.Remolquess;
                        remolques.ForEach(r => r.AutoTransporte = null);
                    }
                }

                //copia transporteMaritimo - contenedoresM
                List<ContenedorM> contenedorM = new List<ContenedorM>();
                if (complementoCartaPorte.Mercancias.TransporteMaritimo != null)
                {
                    if (complementoCartaPorte.Mercancias.TransporteMaritimo.ContenedoresM.Count > 0 )
                    {
                        contenedorM = complementoCartaPorte.Mercancias.TransporteMaritimo.ContenedoresM;
                        contenedorM.ForEach(cm => cm.TransporteMaritimo = null);
                    }
                }

                //copia trasnporte ferroviario -derecho paso
                List<DerechosDePasos> derechoPasos = new List<DerechosDePasos>();
                List<Carro> carro = new List<Carro>();
                Carro[] arraysDataCarroContenedor = new Carro[10];
                if (complementoCartaPorte.Mercancias.TransporteFerroviario != null)
                {
                    if (complementoCartaPorte.Mercancias.TransporteFerroviario.DerechosDePasoss != null)
                    {
                        if (complementoCartaPorte.Mercancias.TransporteFerroviario.DerechosDePasoss.Count > 0)
                        {
                            derechoPasos = complementoCartaPorte.Mercancias.TransporteFerroviario.DerechosDePasoss;
                            derechoPasos.ForEach(dp => dp.TransporteFerroviario = null);
                        }
                    }

                    //copia trasnporte ferroviaro - carro
                    if (complementoCartaPorte.Mercancias.TransporteFerroviario.Carros != null)
                    {
                        if (complementoCartaPorte.Mercancias.TransporteFerroviario.Carros.Count > 0)
                        {
                            carro = complementoCartaPorte.Mercancias.TransporteFerroviario.Carros;


                            var TotalCarro = complementoCartaPorte.Mercancias.TransporteFerroviario.Carros.Count;
                            carro.ForEach(c => c.TransporteFerroviario = null);

                            for (int x = 0; x < complementoCartaPorte.Mercancias.TransporteFerroviario.Carros.Count; x++)
                            {
                                arraysDataCarroContenedor[x] = new Carro();
                                arraysDataCarroContenedor[x].ContenedoresC = new List<ContenedorC>();
                                arraysDataCarroContenedor[x].ContenedoresC = complementoCartaPorte.Mercancias.TransporteFerroviario.Carros[x].ContenedoresC;
                            }
                        }
                    }
                }
                    //copia Figura transporte
                 
                    var figuraTransporte = complementoCartaPorte.FiguraTransporte;
                TiposFigura[] arraysDataTiposFigura = new TiposFigura[10];
                if (complementoCartaPorte.FiguraTransporte.Count > 0)
                {
                    var TotalFiguraTransporte = complementoCartaPorte.FiguraTransporte.Count;
                    figuraTransporte.ForEach(f => f.ComplementoCP = null);
                    
                    for (int x = 0; x < complementoCartaPorte.FiguraTransporte.Count; x++)
                    {
                        arraysDataTiposFigura[x] = new TiposFigura();
                        arraysDataTiposFigura[x].PartesTransportes = new List<PartesTransporte>();
                        arraysDataTiposFigura[x].PartesTransportes = complementoCartaPorte.FiguraTransporte[x].PartesTransportes;
                    }
                }
                    complementoCartaPorte.Conceptoss = null;
                    complementoCartaPorte.Ubicaciones = null;
                    complementoCartaPorte.FiguraTransporte = null;
                    complementoCartaPorte.Mercancias.Mercanciass = null;
                if (complementoCartaPorte.Mercancias.AutoTransporte != null)
                {
                    complementoCartaPorte.Mercancias.AutoTransporte.Remolquess = null;
                }
                if (complementoCartaPorte.Mercancias.TransporteMaritimo != null)
                {
                    complementoCartaPorte.Mercancias.TransporteMaritimo.ContenedoresM = null;
                }
                if (complementoCartaPorte.Mercancias.TransporteFerroviario != null)
                {
                    complementoCartaPorte.Mercancias.TransporteFerroviario.DerechosDePasoss = null;
                    complementoCartaPorte.Mercancias.TransporteFerroviario.Carros = null;
                }
                    complementoCartaPorte.FiguraTransporte = null;
                    _db.ComplementoCartaPortes.Add(complementoCartaPorte);
                    _db.SaveChanges();
                    //guarda data y relacion
                    //conceptos
                    foreach (var concepto in conceptos)
                    {
                        concepto.ComplementoCP = null;
                        concepto.Complemento_Id = complementoCartaPorte.Id;
                        _db.Conceptos.Add(concepto);
                    }
                    _db.SaveChanges();

                    //mercancia - mercancia
                    foreach(var mercancia in mercancias)
                    {
                        mercancia.Mercancias = null;
                        mercancia.Pedimentoss = null;
                        mercancia.GuiasIdentificacionss = null;
                        mercancia.CantidadTransportadass = null;
                        mercancia.Mercancias_Id = complementoCartaPorte.Mercancias.Id;
                        _db.Mercancia.Add(mercancia);
                        
                    }
                    _db.SaveChanges();

                    //mercancia - pedimentos
                    for (int x = 0; x < mercancias.Count; x++)
                    {
                    if (arraysDataMercancia[x] != null)
                    {
                        if (arraysDataMercancia[x].Pedimentoss != null)
                        {
                            if (arraysDataMercancia[x].Pedimentoss.Count > 0)
                            {
                                foreach (var pedimento in arraysDataMercancia[x].Pedimentoss)
                                {
                                    pedimento.Mercancia = null;
                                    pedimento.Mercancia_Id = mercancias[x].Id;
                                    _db.Pedimentos.Add(pedimento);
                                }
                                _db.SaveChanges();
                            }
                        }
                    }
                    }

                    //mercancia GuiaIdentificacion
                    for (int x = 0; x < mercancias.Count; x++)
                    {
                    if (arraysDataMercancia[x] != null)
                    {
                        if (arraysDataMercancia[x].GuiasIdentificacionss != null)
                        {
                            if (arraysDataMercancia[x].GuiasIdentificacionss.Count > 0)
                            {
                                foreach (var gIdentificacion in arraysDataMercancia[x].GuiasIdentificacionss)
                                {
                                    gIdentificacion.Mercancia = null;
                                    gIdentificacion.Mercancia_Id = mercancias[x].Id;
                                    _db.GuiasIdentificacion.Add(gIdentificacion);
                                }
                                _db.SaveChanges();
                            }
                        }
                    }
                    }

                    //mercancia cantidad trasportadas
                    
                    for (int x = 0; x < mercancias.Count; x++)
                    {
                    if (arraysDataMercancia[x] != null)
                    {
                        if (arraysDataMercancia[x].CantidadTransportadass != null)
                        {
                            if (arraysDataMercancia[x].CantidadTransportadass.Count > 0)
                            {
                                foreach (var cTransportadas in arraysDataMercancia[x].CantidadTransportadass)
                                {
                                    cTransportadas.Mercancia = null;
                                    cTransportadas.Mercancia_Id = mercancias[x].Id;
                                    _db.CantidadTransportadas.Add(cTransportadas);
                                }
                                _db.SaveChanges();
                            }
                        }
                    }
                    }
                // autoTransporte remolque
                if (remolques != null)
                {
                    if (remolques.Count > 0)
                    {
                        foreach (var remolque in remolques)
                        {
                            remolque.AutoTransporte = null;
                            remolque.AutoTransporte_Id = complementoCartaPorte.Mercancias.AutoTransporte.Id;
                            _db.Remolques.Add(remolque);
                        }
                        _db.SaveChanges();
                    }
                }
                //transporteMaritimo contenedor
                if (contenedorM != null)
                {
                    if (contenedorM.Count > 0)
                    {
                        foreach (var cMaritimo in contenedorM)
                        {
                            cMaritimo.TransporteMaritimo = null;
                            cMaritimo.TransporteMaritimo_Id = complementoCartaPorte.Mercancias.TransporteMaritimo.Id;
                            _db.ContenedoresM.Add(cMaritimo);
                        }
                        _db.SaveChanges();
                    }
                }

                //transporte ferroviario carro
                if (carro != null)
                {
                    if (carro.Count > 0)
                    {
                        foreach (var TFCarro in carro)
                        {
                            TFCarro.TransporteFerroviario = null;
                            TFCarro.ContenedoresC = null;
                            TFCarro.TransporteFerroviario_Id = complementoCartaPorte.Mercancias.TransporteFerroviario.Id;
                            _db.Carros.Add(TFCarro);
                        }
                        _db.SaveChanges();
                    }
                }
                    //transporte ferroviario carro contenedor
                    for (int x = 0; x < carro.Count; x++)
                    {
                    if (arraysDataCarroContenedor[x] != null)
                    {
                        if(arraysDataCarroContenedor[x].ContenedoresC !=null){
                    if (arraysDataCarroContenedor[x].ContenedoresC.Count > 0)
                    {
                        foreach (var contenedor in arraysDataCarroContenedor[x].ContenedoresC)
                        {
                            contenedor.Carro = null;
                            contenedor.Carro_Id = carro[x].Id;
                            _db.ContenedoresC.Add(contenedor);
                        }
                        _db.SaveChanges();
                    }
                    }
                    }
                    }

                //ubicaciones
                if (ubicaciones != null)
                {
                    if (ubicaciones.Count > 0)
                    {
                        foreach (var ubicacion in ubicaciones)
                        {
                            ubicacion.ComplementoCP = null;
                            ubicacion.Complemento_Id = complementoCartaPorte.Id;
                            _db.UbicacionOrigen.Add(ubicacion);
                        }
                        _db.SaveChanges();
                    }
                }

                //figura transporte
                if (figuraTransporte != null)
                {
                    if (figuraTransporte.Count > 0)
                    {
                        foreach (var figura in figuraTransporte)
                        {
                            figura.ComplementoCP = null;
                            figura.PartesTransportes = null;
                            figura.Complemento_Id = complementoCartaPorte.Id;
                            _db.Tiposfigura.Add(figura);
                        }
                        _db.SaveChanges();
                    }
                }
                    //figura partes transporte
                    if(figuraTransporte.Count >0)
                    {
                        
                        for (int x = 0; x < figuraTransporte.Count; x++)
                        {
                        if (arraysDataTiposFigura[x] != null)
                        {
                            if (arraysDataTiposFigura[x].PartesTransportes != null)
                            {
                                if (arraysDataTiposFigura[x].PartesTransportes.Count > 0)
                                {
                                    foreach (var pTransporte in arraysDataTiposFigura[x].PartesTransportes)
                                    {
                                        pTransporte.TiposFigura = null;
                                        pTransporte.TiposFigura_Id = figuraTransporte[x].Id;
                                        _db.PartesTransporte.Add(pTransporte);
                                    }
                                    _db.SaveChanges();
                                }
                            }
                        }
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
            complementoCartaPorte.TiposFigura.Domicilio = new Domicilio();
            return View(complementoCartaPorte);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComplementoCartaPorte complementoCP = _db.ComplementoCartaPortes.Find(id);
           // complementoCP = setDataList(complementoCP);
            if (complementoCP.TipoDeComprobante == c_TipoDeComprobante.I)
            {
                complementoCP.hidden = true;
            }
            else { complementoCP.hidden = false; }

            if (complementoCP == null)
            {
                return HttpNotFound();
            }
            PopulaClientes(complementoCP.ReceptorId);
            PopulaTiposDeComprobanteFiltro(complementoCP.TipoDeComprobante);
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
            PopulaEntradaSalidaMerc();
            PopulaConceptos();
            PopulaImpuestoR();
            PopulaImpuestoT();
            PopulaImpuestoSat();
            PopulaFormaPagoFiltro(complementoCP.FormaPago);
            Random random = new Random();
            var randomNumber = random.Next(0, 1000000).ToString("D6");

            
            complementoCP.Conceptos = new Conceptos()
            {
                Traslado = new TrasladoCP()
                {
                    TipoImpuesto = "Traslado",
                    //Impuesto = c_Impuesto.Iva,
                    TipoFactor = c_TipoFactor.Tasa,
                    Base = 0,
                    TasaOCuota = 0,
                    Importe = 0
                },
                Retencion = new RetencionCP()
                {
                    TipoImpuesto = "Retencion",
                    //Impuesto = c_Impuesto.Iva,
                    TipoFactor = c_TipoFactor.Tasa,
                    Base = 0,
                    TasaOCuota = 0,
                    Importe = 0
                }

            };


            complementoCP.Mercancias.Mercancia = new Mercancia()
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
            };
            /*complementoCP.Mercancias.TransporteFerroviario = new TransporteFerroviario()
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
            };
            complementoCP.Mercancias.TransporteMaritimo = new TransporteMaritimo()
                {
                    ContenedorM = new ContenedorM() { }
                };

            */

            complementoCP.Ubicacion = new Ubicacion

            {
                IDUbicacion = "OR" + randomNumber,
                DistanciaRecorrida = 0,
                TipoUbicacion = "Origen",
                FechaHoraSalidaLlegada = DateTime.Now,

                Domicilio = new Domicilio
                {

                }
            };

            complementoCP.TiposFigura = new TiposFigura()
            {
                Domicilio = new Domicilio() { },
                PartesTransporte = new PartesTransporte() { }
            };
            

            return View(complementoCP);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(ComplementoCartaPorte complementoCP)
        {
            ModelState.Remove("Mercancias.AutoTransporte.IdentificacionVehicular.AnioModeloVM");
            ModelState.Remove("Mercancias.TransporteAereo.ResidenciaFiscalEmbarc");
            ModelState.Remove("Mercancias.TransporteMaritimo.AnioEmbarcacion");
            ModelState.Remove("Mercancias.TransporteMaritimo.Calado");
            ModelState.Remove("Mercancias.TransporteMaritimo.Eslora");
            ModelState.Remove("Mercancias.TransporteMaritimo.Manga");
            ModelState.Remove("Mercancias.TransporteMaritimo.UnidadesDeArqBruto");
            ModelState.Remove("Mercancias.TransporteFerroviario.TipoDeTrafico");
            ModelState.Remove("Mercancias.TransporteFerroviario.Carro.ToneladasNetasCarro");
            ModelState.Remove("Mercancias.TransporteFerroviario.Carro.ContenedorC.PesoContenedorVacio");
            ModelState.Remove("Mercancias.TransporteFerroviario.Carro.ContenedorC.PesoNetoMercancia");
            ModelState.Remove("Mercancias.TransporteFerroviario.DerechosDePasos.KilometrajePagado");
            ModelState.Remove("SucursalId");
            ModelState.Remove("ReceptorId");
            ModelState.Remove("FacturaEmitidaId");
            ModelState.Remove("CfdiRelacionadoId");
            ModelState.Remove("Receptor.RazonSocial");
            ModelState.Remove("Receptor");
            ModelState.Remove("Sucursal.RazonSocial");
            PopulaClientes(complementoCP.ReceptorId);

            PopulaTiposDeComprobanteFiltro(complementoCP.TipoDeComprobante);
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
            PopulaEntradaSalidaMerc();
            PopulaConceptos();
            PopulaImpuestoR();
            PopulaImpuestoT();
            PopulaImpuestoSat();
            PopulaFormaPagoFiltro(complementoCP.FormaPago);

            if (ModelState.IsValid)
            {
                _acondicionarComplementosCartaPorte.cargaConceptosEdit(ref complementoCP);
                _acondicionarComplementosCartaPorte.cargaUbicacionesEdit(ref complementoCP);
                _acondicionarComplementosCartaPorte.cargaMercanciasEdit(ref complementoCP);
                _acondicionarComplementosCartaPorte.cargaTransportesEdit(ref complementoCP);
                _acondicionarComplementosCartaPorte.cargaFiguraTransporteEdit(ref complementoCP);
                _db.Entry(complementoCP).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            //Identifica los mensaje de error
            var errors = ModelState.Values.Where(E => E.Errors.Count > 0)
                     .SelectMany(E => E.Errors)
                     .Select(E => E.ErrorMessage)
                     .ToList();
            //Identifica el campo del Required
            var modelErrors = ModelState.Where(m => ModelState[m.Key].Errors.Any());
            ModelState.AddModelError("", "Error revisar los campos requeridos: " + errors);
            complementoCP.Ubicacion = new Ubicacion()
            {

                Domicilio = new Domicilio()
                {
                    Pais = complementoCP.Ubicacion.Domicilio.Pais
                }
            };

            complementoCP.TiposFigura.Domicilio = new Domicilio();

            return View(complementoCP);
        }


        public ActionResult Generar(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var sucursalId = ObtenerSucursal();
            ComplementoCartaPorte complementoCartaPorte = _db.ComplementoCartaPortes.Find(id);

            if (complementoCartaPorte == null)
            {
                return HttpNotFound();
            }
            PopulaClientes(complementoCartaPorte.ReceptorId);
            return View(complementoCartaPorte);
            
        }

        

        [HttpPost]
        public ActionResult Generar(ComplementoCartaPorte complementoCartaPorte)
        {
            
            ModelState.Remove("Sucursal.RazonSocial");
            PopulaClientes(complementoCartaPorte.ReceptorId);

            string error = "";
                try
                {
                    var sucursalId = ObtenerSucursal();

                    //Actualizacion Receptor

                    DateTime fechaDoc = complementoCartaPorte.FechaDocumento;
                    var horaHoy = DateTime.Now;
                    var fechaTime = new DateTime(fechaDoc.Year, fechaDoc.Month, fechaDoc.Day, horaHoy.Hour, horaHoy.Minute, horaHoy.Second);

                    var complementoCartaPorteDb = _db.ComplementoCartaPortes.Find(complementoCartaPorte.Id);
                    
                    complementoCartaPorteDb.ReceptorId = complementoCartaPorte.ReceptorId;
                    complementoCartaPorteDb.FechaDocumento = fechaTime;
                    complementoCartaPorteDb.Hora = fechaTime;
                    //_db.Entry(complementoCartaPorteDb).State = EntityState.Modified;
                    //_db.SaveChanges();

                    _cartaPorteManager.GenerarComplementoCartaPorte(sucursalId, complementoCartaPorte.Id, "");
                }
                catch (Exception ex)
                {
                error = ex.Message;
                    
                }
                if(error == "")
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", error);
                }
            
            return View(complementoCartaPorte);
        }

        

        public ActionResult Descargar(int id)
        {
            var pathCompleto = _cartaPorteManager.GenerarZipComplementoCartaPorte(id);
            byte[] archivoFisico = System.IO.File.ReadAllBytes(pathCompleto);
            string contentType = MimeMapping.GetMimeMapping(pathCompleto);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = Path.GetFileName(pathCompleto),
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(archivoFisico, contentType);
        }

        public ActionResult DescargarPDF(int id)
        {
            /*var pathCompleto =*/ _cartaPorteManager.GeneraPDF(id);
            //byte[] archivoFisico = System.IO.File.ReadAllBytes(pathCompleto);
            //string contentType = MimeMapping.GetMimeMapping(pathCompleto);

            var cd = new System.Net.Mime.ContentDisposition
            {
              //  FileName = Path.GetFileName(pathCompleto),
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return RedirectToAction("Index"); /*File(archivoFisico, contentType)*/;
        }



        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ComplementoCartaPorte complementoCP = _db.ComplementoCartaPortes.Find(id);
            if (complementoCP == null)
            {
                return HttpNotFound();
            }
            PopulaClientes(complementoCP.ReceptorId);
           
             
            _db.ComplementoCartaPortes.Remove(complementoCP);
            _db.SaveChanges();
            return RedirectToAction("Index");
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

        public JsonResult DatosCatalogoConcepto(int  IdConcepto)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            var Clave = popularDropDowns.PopulaDatosConceptos(IdConcepto);
            return Json(Clave, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DatosCatalogoImpuesto(int IdImpuesto)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            var Clave = popularDropDowns.PopulaDatosImpuestos(IdImpuesto);
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

        private void PopulaImpuestoSat()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.ImpuestoSat = (popularDropDowns.PopulaImpuestoSat());
        }

        private void PopulaFormaPagoFiltro(string formaPago)
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.FormaPAgo = (popularDropDowns.PopulaFormaPagoFiltro(formaPago));
        }
        private void PopulaFormaPago()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.FormaPAgo = (popularDropDowns.PopulaFormaPago());
        }

        private void PopulaConceptos()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.Conceptos = (popularDropDowns.PopulaConceptos());
        }

        private void PopulaImpuestoT()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.ImpuestoT = (popularDropDowns.PopulaImpuestoT());
        }

        private void PopulaImpuestoR()
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.ImpuestoR = (popularDropDowns.PopulaImpuestoR());
        }

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

        private void PopulaTiposDeComprobanteFiltro(c_TipoDeComprobante tipoComprobante )
        {
            var popularDropDowns = new PopularDropDowns(ObtenerSucursal(), true);
            ViewBag.TipoDeComprobante = (popularDropDowns.PopulaTipoDeComprobanteFiltro(tipoComprobante));
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

        private void PopulaEntradaSalidaMerc()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "Entrada", Value = "Entrada", Selected = true });
            items.Add(new SelectListItem { Text = "Salida", Value = "Salida" });
            
            ViewBag.EntradaSMercancia = items;
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