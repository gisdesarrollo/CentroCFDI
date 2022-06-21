using API.Operaciones.ComplementosPagos;
using Aplicacion.Context;
using System.Linq;
using System.Data.Entity.Migrations;
using System.Collections.Generic;
using System.Data.Entity;
using System;

namespace Aplicacion.LogicaPrincipal.Acondicionamientos.Operaciones
{
    public class AcondicionarComplementosPagos
    {

        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion

        public void CargaInicial(ref ComplementoPago complementoPago)
        {
            if(complementoPago.Pagos != null)
            {
                foreach (var pago in complementoPago.Pagos)
                {
                    pago.BancoOrdenante = null;
                    pago.BancoBeneficiario = null;
                    pago.ComplementoPago = null;
                }
            }

            complementoPago.Pago = null;
        }

        public void Pagos(ComplementoPago complementoPago)
        {
            if (complementoPago.Pagos != null)
            {
                var idsBorrar = complementoPago.Pagos.Select(e => e.Id);
                var pagosAnteriores = _db.Pagos.Where(es => es.ComplementoPagoId == complementoPago.Id && !idsBorrar.Contains(es.Id)).ToList();
               
                    _db.Pagos.RemoveRange(pagosAnteriores);
                    _db.SaveChanges();
                double montoTotal = 0;
                decimal totalRetencionesISR = 0;
                decimal totalRetencionesIVA = 0;
                decimal totalTrasladosBaseIVA16 = 0;
                decimal totalTrasladosImpuestoIVA16 = 0;
                decimal totalTrasladosBaseIVA8 = 0;
                decimal totalTrasladosImpuestoIVA8 = 0;
                foreach (var pago in complementoPago.Pagos.Except(pagosAnteriores))
                {
                    pago.ComplementoPagoId = complementoPago.Id;
                    pago.BancoOrdenante = null;
                    pago.BancoBeneficiario = null;
                    pago.ComplementoPago = null;
                    //calcula el monto total por cada pago
                    montoTotal += pago.Monto;
                     
                    _db.Pagos.AddOrUpdate(pago);
                    try
                    {
                        _db.SaveChanges();
                        //busca y calcula los impuestos totales
                        var pagos = _db.Pagos.Find(pago.Id);
                        if (pagos.DocumentosRelacionados != null && pagos.DocumentosRelacionados.Count > 0)
                        {
                            foreach (var DR in pagos.DocumentosRelacionados)
                            {

                                if (DR.Traslado != null)
                                {
                                                                        
                                        if (DR.Traslado.Impuesto == "002" && DR.Traslado.TasaOCuota == (decimal)0.16)
                                        {
                                            totalTrasladosBaseIVA16 += DR.Traslado.Base;
                                            totalTrasladosImpuestoIVA16 += DR.Traslado.Importe;
                                        }
                                        if (DR.Traslado.Impuesto == "002" && DR.Traslado.TasaOCuota == (decimal)0.08)
                                        {
                                            totalTrasladosBaseIVA8 += DR.Traslado.Base;
                                            totalTrasladosImpuestoIVA8 += DR.Traslado.Importe;
                                        }
                                    
                                }
                                
                                if (DR.Retencion != null)
                                {
                                    
                                        if (DR.Retencion.Impuesto == "001") { totalRetencionesISR += DR.Retencion.Importe; }
                                        if (DR.Retencion.Impuesto == "002") { totalRetencionesIVA += DR.Retencion.Importe; }
                                    
                                }
                                
                            }
                        }
                    }
                    catch (System.Exception)
                    {
                    }
                }
                if (montoTotal > 0)
                {
                    TotalesPagosImpuestos totalPagoImpuesto = _db.TotalesPagosImpuestos.Find(complementoPago.TotalesPagoImpuestoId);
                    totalPagoImpuesto.TotalRetencionesIVA = Decimal.ToDouble(totalRetencionesIVA);
                    totalPagoImpuesto.TotalRetencionesISR = Decimal.ToDouble(totalRetencionesISR);
                    totalPagoImpuesto.TotalTrasladosBaseIVA16 = Decimal.ToDouble(totalTrasladosBaseIVA16);
                    totalPagoImpuesto.TotalTrasladosBaseIVA8 = Decimal.ToDouble(totalTrasladosBaseIVA8);
                    totalPagoImpuesto.TotalTrasladosImpuestoIVA16 = Decimal.ToDouble(totalTrasladosImpuestoIVA16);
                    totalPagoImpuesto.TotalTrasladosImpuestoIVA8 = Decimal.ToDouble(totalTrasladosImpuestoIVA8);
                    totalPagoImpuesto.MontoTotalPagos = montoTotal;

                    _db.Entry(totalPagoImpuesto).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }
            else
            {
                var pagosAnteriores = _db.Pagos.Where(es => es.ComplementoPagoId == complementoPago.Id).ToList();

                _db.Pagos.RemoveRange(pagosAnteriores);
                _db.SaveChanges();
            }
        }

        public void DocumentosRelacionados(ComplementoPago complementoPago)
        {
            if (complementoPago.DocumentosRelacionados != null)
            {
                foreach (var pago in complementoPago.Pagos)
                {
                    var idsBorrar = complementoPago.DocumentosRelacionados.Select(e => e.Id);
                    var documentosAnteriores = _db.DocumentosRelacionados.Where(es => es.PagoId == pago.Id && !idsBorrar.Contains(es.Id)).ToList();

                    _db.DocumentosRelacionados.RemoveRange(documentosAnteriores);
                    _db.SaveChanges();

            //        foreach (var documento in complementoPago.DocumentosRelacionados)
            //        {
            //            documento.PagoId = pago.Id;
            //            _db.DocumentosRelacionados.AddOrUpdate(documento);
            //        }

            //        _db.SaveChanges();
                }
            }
            else
            {
                var documentosAnteriores = _db.DocumentosRelacionados.Where(es => es.Pago.ComplementoPagoId == complementoPago.Id).ToList();

                _db.DocumentosRelacionados.RemoveRange(documentosAnteriores);
                _db.SaveChanges();
            }
        }
    }
}
