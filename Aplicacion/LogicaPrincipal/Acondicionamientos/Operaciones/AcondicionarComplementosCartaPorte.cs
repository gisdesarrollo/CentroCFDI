using API.Operaciones.ComplementoCartaPorte;
using API.RelacionesCartaPorte;
using Aplicacion.Context;
using System;
using System.Collections.Generic;
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
                        if (concepto.Retencion != null && concepto.Traslado != null && complementoCartaPorte.TipoDeComprobante ==CFDI.API.Enums.CFDI33.c_TipoDeComprobante.I)
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
            if (complementoCartaPorte.Ubicaciones != null)
            {
                foreach (var ubicacion in complementoCartaPorte.Ubicaciones)
                {
                    if (ubicacion.TipoUbicacion == "Destino")
                    {
                        complementoCartaPorte.TotalDistRec += ubicacion.DistanciaRecorrida;
                    }
                    _db.Domicilios.Add(ubicacion.Domicilio);
                    _db.SaveChanges();
                    ubicacion.Domicilio_Id = ubicacion.Domicilio.Id;
                    _db.UbicacionOrigen.Add(ubicacion);
                    _db.SaveChanges();

                }
            }
        }

        public void cargaMercancias(ref ComplementoCartaPorte complementoCartPorte)
        {

            _db.Mercancias.Add(complementoCartPorte.Mercancias);
            _db.SaveChanges();
            if (complementoCartPorte.Mercanciass != null)
            {
                foreach(var mercancia in complementoCartPorte.Mercanciass)
                {
                    
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
                    if (mercancia.DetalleMercancia != null)
                    {
                        _db.DetalleMercancias.Add(mercancia.DetalleMercancia);
                        _db.SaveChanges();
                        var mercanciaDetalle = new MercanciaDetalleMercancia()
                        {
                            Mercancia_Id = mercancia.Id,
                            DetalleMercancia_Id = mercancia.DetalleMercancia.Id
                        };
                        _db.MercanciaDetalleMercancia.Add(mercanciaDetalle);
                        _db.SaveChanges();
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
            if (complementoCartaPorte.ClaveTransporteId == "1")
            {
                //error en esta parte
                _db.AutoTransporte.Add(complementoCartaPorte.Mercancias.AutoTransporte);
                _db.SaveChanges();
            }
            if(complementoCartaPorte.ClaveTransporteId == "2")
            {
                _db.TransporteMaritimos.Add(complementoCartaPorte.Mercancias.TransporteMaritimo);
                _db.SaveChanges();
            }
            if(complementoCartaPorte.ClaveTransporteId == "3")
            {
                _db.TransporteAereos.Add(complementoCartaPorte.Mercancias.TransporteAereo);
                _db.SaveChanges();
            }
            if(complementoCartaPorte.ClaveTransporteId == "4")
            {
                _db.TransporteFerroviarios.Add(complementoCartaPorte.Mercancias.TransporteFerroviario);
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
                            _db.PartesTransporte.Add(PT);
                            _db.SaveChanges();

                            var FiguraParteTransporte = new TiposFiguraPartesTransporte() { 
                                TiposFigura_Id = FT.Id,
                                PartesTransporte_Id = PT.Id
                            };
                            _db.TiposFiguraPartesTransporte.Add(FiguraParteTransporte);
                            _db.SaveChanges();
                            _db.Domicilios.Add(PT.Domicilio);
                            _db.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}
