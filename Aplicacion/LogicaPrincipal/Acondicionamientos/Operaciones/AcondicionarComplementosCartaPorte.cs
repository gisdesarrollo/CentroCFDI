using API.Operaciones.ComplementoCartaPorte;
using API.RelacionesCartaPorte;
using Aplicacion.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.LogicaPrincipal.Acondicionamientos.Operaciones
{
    public class AcondicionarComplementosCartaPorte
    {
        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion

        public List<ComplementoCartaPorte> Filtrar(DateTime fechaInicial, DateTime fechaFinal, int sucursalId)
        {
            var complementos = _db.ComplementoCartaPortes.Where(cp => cp.SucursalId == sucursalId && cp.FechaDocumento >= fechaInicial && cp.FechaDocumento <= fechaFinal).ToList();
            return complementos;
        }
        public void cargaConceptos(ref ComplementoCartaPorte complementoCartaPorte)
        {
            Decimal ImpuestoRet = 0;
            Decimal ImpuestoTrasl = 0;
            Decimal Subtotal = 0;
            var errores = new List<String>();
            try
            {
                if (complementoCartaPorte.Conceptoss != null)
                {
                    foreach (var concepto in complementoCartaPorte.Conceptoss)
                    {
                        if (concepto.Retencion != null && concepto.Traslado != null && complementoCartaPorte.TipoDeComprobante == CFDI.API.Enums.CFDI33.c_TipoDeComprobante.I)
                        {
                            ImpuestoRet = concepto.Retencion.Importe;
                            ImpuestoTrasl = concepto.Traslado.Importe;
                            Subtotal = complementoCartaPorte.Subtotal;
                            concepto.Retencion.TotalImpuestosTR = ImpuestoRet;
                            concepto.Traslado.TotalImpuestosTR = ImpuestoTrasl;
                            //calcula total precio pactado + impuestosTrasl - impuestoRet
                            complementoCartaPorte.Total += (Subtotal + ImpuestoTrasl) - ImpuestoRet;
                            _db.SubImpuestoC.Add(concepto.Retencion);
                            _db.SaveChanges();
                            _db.SubImpuestoC.Add(concepto.Traslado);
                            _db.SaveChanges();

                            _db.Conceptos.Add(concepto);
                            _db.SaveChanges();

                            ConceptoSubImpuestoConcepto subImp = new ConceptoSubImpuestoConcepto();
                            subImp.Concepto_Id = concepto.Id;
                            subImp.SubImpuestoConcepto_Id = concepto.Retencion.Id;
                            _db.ConceptoSubImpuestoConcepto.Add(subImp);
                            _db.SaveChanges();
                            subImp.Concepto_Id = concepto.Id;
                            subImp.SubImpuestoConcepto_Id = concepto.Traslado.Id;
                            _db.ConceptoSubImpuestoConcepto.Add(subImp);
                            _db.SaveChanges();
                        }
                        else if (concepto.Traslado != null && concepto.Retencion == null && complementoCartaPorte.TipoDeComprobante == CFDI.API.Enums.CFDI33.c_TipoDeComprobante.I)
                        {
                            ImpuestoTrasl = concepto.Traslado.Importe;
                            Subtotal = complementoCartaPorte.Subtotal;
                            complementoCartaPorte.Total += Subtotal + ImpuestoTrasl;
                        
                            _db.SubImpuestoC.Add(concepto.Traslado);
                            _db.SaveChanges();

                            _db.Conceptos.Add(concepto);
                            _db.SaveChanges();

                            ConceptoSubImpuestoConcepto subImp = new ConceptoSubImpuestoConcepto();
                            subImp.Concepto_Id = concepto.Id;
                            subImp.SubImpuestoConcepto_Id = concepto.Traslado.Id;
                            _db.ConceptoSubImpuestoConcepto.Add(subImp);
                            _db.SaveChanges();
                        }
                        else
                        {
                            _db.Conceptos.Add(concepto);
                            _db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception e) {
               
            }

        }

        public void cargaUbicaciones(ref ComplementoCartaPorte complementoCartaPorte)
        {
            var cartaPorteDb = _db.ComplementoCartaPortes.Find(complementoCartaPorte.Id);
            cartaPorteDb.TotalDistRec = 0;
            if (complementoCartaPorte.Ubicaciones != null)
            {
                foreach (var ubicacion in complementoCartaPorte.Ubicaciones)
                {
                    if (ubicacion.TipoUbicacion == "Destino")
                    {
                        cartaPorteDb.TotalDistRec += ubicacion.DistanciaRecorrida;
                    }
                    if(ubicacion.RfcRemitenteDestinatario != "XEXX010101000")
                    {
                        ubicacion.NumRegIdTrib = null;
                        ubicacion.ResidenciaFiscal = null;
                    }
                    if(complementoCartaPorte.ClaveTransporteId == "01") {
                        ubicacion.TipoEstacion_Id = null;
                        ubicacion.TipoEstaciones = null;
                        ubicacion.NumEstacion = null;
                        ubicacion.NavegacionTrafico = null;
                        ubicacion.NombreEstacion = null;
                    }
                    _db.Domicilios.Add(ubicacion.Domicilio);
                    _db.SaveChanges();
                   // ubicacion.Domicilio_Id = ubicacion.Domicilio.Id;
                    _db.UbicacionOrigen.Add(ubicacion);
                    _db.SaveChanges();

                }
                
                _db.Entry(cartaPorteDb).State = EntityState.Modified;
                _db.SaveChanges();
            }
        }

        public void cargaMercancias(ref ComplementoCartaPorte complementoCartPorte)
        {


            var mercanciaDb = _db.Mercancias.Find(complementoCartPorte.Mercancias.Id);
            if (complementoCartPorte.Mercanciass != null)
            {
                foreach(var mercancia in complementoCartPorte.Mercanciass)
                {
                    if(mercancia.DetalleMercancia == null)
                    {
                        
                        mercancia.DetalleMercancia = null;
                        //_db.DetalleMercancias.Add(mercancia.DetalleMercancia);
                        //_db.SaveChanges();
                    }
                    else
                    {
                        if(complementoCartPorte.ClaveTransporteId=="02" || complementoCartPorte.ClaveTransporteId == "04")
                        {
                            mercanciaDb.PesoBrutoTotal += mercancia.DetalleMercancia.PesoBruto;
                            mercanciaDb.PesoNetoTotal += mercancia.DetalleMercancia.PesoNeto;
                            _db.Entry(mercanciaDb).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                    }
                    
                    _db.Mercancia.Add(mercancia);
                    _db.SaveChanges();
                    if (mercancia.Pedimentoss != null)
                    {
                        var mercanciaPedimento = new MercanciaPedimentos();
                        foreach (var pe in mercancia.Pedimentoss)
                        {
                            _db.Pedimentos.Add(pe);
                            _db.SaveChanges();
                            
                            mercanciaPedimento.Mercancia_Id = mercancia.Id;
                            mercanciaPedimento.Pedimentos_Id = pe.Id;
                            _db.MercanciaPedimentos.Add(mercanciaPedimento);
                            _db.SaveChanges();
                        }
                        
                        
                    }
                    if (mercancia.GuiasIdentificacionss != null)
                    {
                        foreach(var GI in mercancia.GuiasIdentificacionss)
                        {
                            _db.GuiasIdentificacion.Add(GI);
                            _db.SaveChanges();
                            var mercanciGuiasIdent = new MercanciaGuiasIdentificacion()
                            {
                                Mercancia_Id = mercancia.Id,
                                GuiasIdentificacion_Id = GI.Id
                            };

                            _db.MercanciasGuiasIdentificacion.Add(mercanciGuiasIdent);
                            _db.SaveChanges();
                        }
                        
                    }
                    if (mercancia.CantidadTransportadass != null)
                    {
                        foreach(var CT in mercancia.CantidadTransportadass)
                        {
                            _db.CantidadTransportadas.Add(CT);
                            _db.SaveChanges();
                            var mercanciaCantidaTransp = new MercanciaCantidadTransportada()
                            {
                                Mercancia_Id = mercancia.Id,
                                CantidadTransportada_Id = CT.Id
                            };
                            _db.MercanciaCantidadTransportadas.Add(mercanciaCantidaTransp);
                            _db.SaveChanges();
                        }
                       
                    }
                   
                    
                    var RelMercancias = new MercanciasMercancia()
                    {
                        Mercancias_Id = complementoCartPorte.Mercancias.Id,
                        Mercancia_Id = mercancia.Id
                    };
                    _db.MercanciasMercancias.Add(RelMercancias);
                    _db.SaveChanges();
                }
            }
        }

        public void cargaTransporte(ref ComplementoCartaPorte complementoCartaPorte)
        {
            //var mercanciaDb = _db.Mercancias.Find(complementoCartaPorte.Mercancias.Id);
            
            if (complementoCartaPorte.ClaveTransporteId == "01")
            {
               
                complementoCartaPorte.Mercancias.TransporteFerroviario = null;
                complementoCartaPorte.Mercancias.TransporteMaritimo = null;
                complementoCartaPorte.Mercancias.TransporteAereo = null;
               
                _db.ComplementoCartaPortes.Add(complementoCartaPorte);
                _db.SaveChanges();
                if (complementoCartaPorte.Mercancias.AutoTransporte.Remolquess != null)
                {
                    foreach(var remolque in complementoCartaPorte.Mercancias.AutoTransporte.Remolquess)
                    {
                        _db.Remolques.Add(remolque);
                        _db.SaveChanges();
                        var relAutoTransporteRemolque = new AutotransporteRemolque()
                        {
                            Autotransporte_Id = complementoCartaPorte.Mercancias.AutoTransporte.Id,
                            Remolques_Id = remolque.Id
                        };
                        _db.AutotransporteFederalRemolques.Add(relAutoTransporteRemolque);
                        _db.SaveChanges();
                    }
                    
                }
            }
            else if(complementoCartaPorte.ClaveTransporteId == "02")
            {
               
                complementoCartaPorte.Mercancias.TransporteFerroviario = null;
                complementoCartaPorte.Mercancias.AutoTransporte = null;
                complementoCartaPorte.Mercancias.TransporteAereo = null;
                _db.ComplementoCartaPortes.Add(complementoCartaPorte);
                _db.SaveChanges();
                if (complementoCartaPorte.Mercancias.TransporteMaritimo.ContenedoresM != null)
                {
                    foreach(var contenedor in complementoCartaPorte.Mercancias.TransporteMaritimo.ContenedoresM)
                    {
                        _db.ContenedoresM.Add(contenedor);
                        _db.SaveChanges();

                        var relMaritimoContenedor = new TransporteMaritimoContenedorM() { 
                        TransporteMaritimo_Id = complementoCartaPorte.Mercancias.TransporteMaritimo.Id,
                        ContenedorM_Id = contenedor.Id
                        };
                        _db.TransporteMaritimoContenedoresM.Add(relMaritimoContenedor);
                        _db.SaveChanges();
                    }
                    


                }

            }
            else if(complementoCartaPorte.ClaveTransporteId == "03")
            {
              
                complementoCartaPorte.Mercancias.TransporteFerroviario = null;
                complementoCartaPorte.Mercancias.AutoTransporte = null;
                complementoCartaPorte.Mercancias.TransporteMaritimo = null;
                _db.ComplementoCartaPortes.Add(complementoCartaPorte);
                _db.SaveChanges();

            }
            else if (complementoCartaPorte.ClaveTransporteId == "04")
            {
                
                complementoCartaPorte.Mercancias.TransporteMaritimo = null;
                complementoCartaPorte.Mercancias.AutoTransporte = null;
                complementoCartaPorte.Mercancias.TransporteAereo = null;
                _db.ComplementoCartaPortes.Add(complementoCartaPorte);
                _db.SaveChanges();
                if (complementoCartaPorte.Mercancias.TransporteFerroviario.DerechosDePasoss != null)
                {
                    foreach(var derechoDePaso in complementoCartaPorte.Mercancias.TransporteFerroviario.DerechosDePasoss)
                    {
                        _db.DerechoDePasos.Add(derechoDePaso);
                        _db.SaveChanges();

                        var relFerroviarioDDePaso = new TransporteFerroviarioDerechosDePaso() { 
                        TransporteFerroviario_Id = complementoCartaPorte.Mercancias.TransporteFerroviario.Id,
                        DerechosDePaso_Id = derechoDePaso.Id
                        };
                        _db.TransporteFerroviarioDerechosDePasos.Add(relFerroviarioDDePaso);
                        _db.SaveChanges();
                    }
                   
                }
                if (complementoCartaPorte.Mercancias.TransporteFerroviario.Carros != null)
                {
                    foreach(var carro in complementoCartaPorte.Mercancias.TransporteFerroviario.Carros)
                    {
                        _db.Carros.Add(carro);
                        _db.SaveChanges();
                        if (carro.ContenedoresC != null)
                        {
                            foreach (var contenedorC in carro.ContenedoresC)
                            {
                                _db.ContenedoresC.Add(contenedorC);
                                _db.SaveChanges();
                                var relCarroContenedorC = new CarroContenedorC() { 
                                Carro_Id = carro.Id,
                                ContenedorC_Id = contenedorC.Id
                                };
                                _db.CarrosContenedorC.Add(relCarroContenedorC);
                                _db.SaveChanges();
                            }
                        }
                        var relFerroviarioCarro = new TransporteFerroviarioCarro()
                        {
                            TransporteFerroviario_Id = complementoCartaPorte.Mercancias.TransporteFerroviario.Id,
                            Carro_Id = carro.Id
                        };
                        _db.TransporteFerroviarioCarros.Add(relFerroviarioCarro);
                        _db.SaveChanges();

                    }
                }

            }
            else
            {
                complementoCartaPorte.Mercancias.TransporteMaritimo = null;
                complementoCartaPorte.Mercancias.AutoTransporte = null;
                complementoCartaPorte.Mercancias.TransporteAereo = null;
                complementoCartaPorte.Mercancias.TransporteFerroviario = null;
                _db.ComplementoCartaPortes.Add(complementoCartaPorte);
                _db.SaveChanges();
            }
        }

        public void cargaFiguraTransporte(ref ComplementoCartaPorte complementoCartaPorte)
        {
            if (complementoCartaPorte.FiguraTransporte != null)
            {
                foreach(var FT in complementoCartaPorte.FiguraTransporte)
                {
                    _db.Tiposfigura.Add(FT);
                    _db.SaveChanges();
                    if (FT.PartesTransportes != null)
                    {
                        foreach(var PT in FT.PartesTransportes)
                        {
                            _db.Domicilios.Add(PT.Domicilio);
                            _db.SaveChanges();
                            //PT.Domicilio_Id = PT.Domicilio.Id;
                            _db.PartesTransporte.Add(PT);
                            _db.SaveChanges();

                            var FiguraParteTransporte = new TiposFiguraPartesTransporte() { 
                                TiposFigura_Id = FT.Id,
                                PartesTransporte_Id = PT.Id
                            };
                            _db.TiposFiguraPartesTransporte.Add(FiguraParteTransporte);
                            _db.SaveChanges();
                           
                        }
                    }
                }
            }
        }
    }
}
