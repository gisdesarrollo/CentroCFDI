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

        public void cargaConceptos(ref ComplementoCartaPorte complementoCartaPorte)
        {
            Decimal ImpuestoRet = 0;
            Decimal ImpuestoTrasl = 0;
            Decimal Subtotal = 0;
            if (complementoCartaPorte.Conceptoss != null)
            {
                foreach (var concepto in complementoCartaPorte.Conceptoss)
                {
                    if (concepto.Retencion != null && concepto.Traslado != null)
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

        public void cargaUbicaciones(ref ComplementoCartaPorte complementoCartaPorte)
        {
            if (complementoCartaPorte.Ubicaciones != null)
            {
                foreach (var ubicacion in complementoCartaPorte.Ubicaciones)
                {
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
                    if (mercancia.Pedimentos != null)
                    {
                        _db.Pedimentos.Add(mercancia.Pedimentos);
                        _db.SaveChanges();
                        var mercanciaPedimento = new MercanciaPedimentos();
                        mercanciaPedimento.Mercancia_Id = mercancia.Id;
                        mercanciaPedimento.Pedimentos_Id = mercancia.Pedimentos.Id;
                        _db.MercanciaPedimentos.Add(mercanciaPedimento);
                        _db.SaveChanges();
                    }
                    if (mercancia.GuiasIdentificacion != null)
                    {
                        _db.GuiasIdentificacion.Add(mercancia.GuiasIdentificacion);
                        _db.SaveChanges();
                        var mercanciGuiasIdent = new MercanciaGuiasIdentificacion() { 
                            Mercancia_Id = mercancia.Id,
                            GuiasIdentificacion_Id = mercancia.GuiasIdentificacion.Id
                        };
                        
                        _db.MercanciasGuiasIdentificacion.Add(mercanciGuiasIdent);
                        _db.SaveChanges();
                    }
                    if (mercancia.CantidadTransportada != null)
                    {
                        _db.CantidadTransportadas.Add(mercancia.CantidadTransportada);
                        _db.SaveChanges();
                    }
                    if (mercancia.DetalleMercancia != null)
                    {
                        _db.DetalleMercancias.Add(mercancia.DetalleMercancia);
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
    }
}
