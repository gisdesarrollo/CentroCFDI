using API.Operaciones.ComplementosPagos;
using Aplicacion.Context;
using System.Linq;
using System.Data.Entity.Migrations;
using System.Collections.Generic;
using System.Data.Entity;
using System;
using Aplicacion.LogicaPrincipal.Facturas;

namespace Aplicacion.LogicaPrincipal.Acondicionamientos.Operaciones
{
    public class AcondicionarComplementosPagos
    {

        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();
        private readonly GetTipoCambioDocRel _conversionTipoCambio = new GetTipoCambioDocRel();
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
                    //calcula el monto total por cada pago y por moneda
                    if (pago.Moneda.ToString() != "MXN")
                    {
                        montoTotal += pago.TipoCambio * pago.Monto;
                    }
                    else
                    {
                        montoTotal += pago.Monto;
                    }
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
                                Decimal tipoCambioDR = 1;
                                var pagoDR = _db.Pagos.Find(DR.PagoId);
                                if (DR.Traslados != null)
                                {
                                    foreach (var traslado in DR.Traslados)
                                    {
                                        decimal baseDR = (decimal)traslado.Base;
                                        decimal ImporteDR = (decimal)traslado.Importe;
                                            
                                        /****/
                                        if (pagoDR.Moneda != DR.Moneda)
                                        {
                                            if (DR.Moneda.ToString() == "USD" && pagoDR.Moneda.ToString() == "MXN")
                                            {
                                                tipoCambioDR = _conversionTipoCambio.GetTipoCambioDocRelacionadoUSD(DR, pagoDR.TipoCambio, pagoDR.Monto);
                                                decimal baseDRFormt = ((decimal)traslado.Base * (decimal)tipoCambioDR);
                                                decimal ImporteDRFormt = (decimal)traslado.Importe * (decimal)tipoCambioDR;
                                                baseDR = decimal.Round(baseDRFormt, 6);
                                                ImporteDR = decimal.Round(ImporteDRFormt, 6);
                                            }
                                            else if (DR.Moneda.ToString() == "MXN" && pagoDR.Moneda.ToString() == "USD")
                                            {
                                                baseDR = (decimal)traslado.Base;
                                                ImporteDR = (decimal)traslado.Importe;

                                            }
                                            else if ((DR.Moneda.ToString() != "MXN" && pagoDR.Moneda.ToString() == "USD") || (DR.Moneda.ToString() == "USD" && pagoDR.Moneda.ToString() != "MXN"))
                                            {
                                                tipoCambioDR = _conversionTipoCambio.GetTipoCambioDocRelacionadoUSD(DR, pagoDR.TipoCambio, pagoDR.Monto);
                                                decimal baseDRFormt = ((decimal)traslado.Base * (decimal)tipoCambioDR);
                                                decimal ImporteDRFormt = (decimal)traslado.Importe * (decimal)tipoCambioDR;

                                                var tipoCambioPgo = (Decimal)pagoDR.TipoCambio;
                                                baseDR = (decimal)baseDRFormt * tipoCambioPgo;
                                                ImporteDR = (decimal)ImporteDRFormt * tipoCambioPgo;
                                            }
                                        }
                                        else if (DR.Moneda.ToString() != "MXN" && pagoDR.Moneda.ToString() != "MXN" && DR.Moneda == pagoDR.Moneda)
                                        {
                                            tipoCambioDR = (Decimal)pagoDR.TipoCambio;
                                            baseDR = (decimal)traslado.Base * tipoCambioDR;
                                            ImporteDR = (decimal)traslado.Importe * tipoCambioDR;
                                        }
                                        /***/
                                        if (traslado.Impuesto == "002" && traslado.TasaOCuota == (decimal)0.16)
                                        {
                                            totalTrasladosBaseIVA16 += baseDR;
                                            totalTrasladosImpuestoIVA16 += ImporteDR;
                                        }
                                        if (traslado.Impuesto == "002" && traslado.TasaOCuota == (decimal)0.08)
                                        {
                                            totalTrasladosBaseIVA8 += baseDR;
                                            totalTrasladosImpuestoIVA8 += ImporteDR;
                                        }

                                    }
                                }
                                    if (DR.Retenciones != null)
                                    {
                                        foreach (var retencion in DR.Retenciones)
                                        {
                                            decimal RImporteDR = (decimal)retencion.Importe;
                                            if (pagoDR.Moneda != DR.Moneda)
                                            {
                                                if (DR.Moneda.ToString() == "USD" && pagoDR.Moneda.ToString() == "MXN")
                                                {
                                                    tipoCambioDR = _conversionTipoCambio.GetTipoCambioDocRelacionadoUSD(DR, pagoDR.TipoCambio, pagoDR.Monto);
                                                    decimal ImporteDRFormt = (decimal)retencion.Importe * (decimal)tipoCambioDR;

                                                    RImporteDR = decimal.Round(ImporteDRFormt, 6);
                                                }
                                                else if (DR.Moneda.ToString() == "MXN" && pagoDR.Moneda.ToString() == "USD")
                                                {
                                                    tipoCambioDR = (Decimal)pagoDR.TipoCambio;
                                                    RImporteDR = (decimal)retencion.Importe / (decimal)tipoCambioDR;

                                                }
                                                
                                        }else if (DR.Moneda.ToString() == "USD" && pagoDR.Moneda.ToString() == "USD")
                                                {
                                                    tipoCambioDR = (Decimal)pagoDR.TipoCambio;
                                                    RImporteDR = (decimal)retencion.Importe * tipoCambioDR;
                                                }

                                            if (retencion.Impuesto == "001") { totalRetencionesISR += (decimal)retencion.Importe; }
                                            if (retencion.Impuesto == "002") { totalRetencionesIVA += (decimal)retencion.Importe; }
                                        }
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
                    if (totalPagoImpuesto == null)
                    {
                         totalPagoImpuesto = new TotalesPagosImpuestos();
                    }
                    
                        totalPagoImpuesto.TotalRetencionesIVA = Decimal.ToDouble(totalRetencionesIVA);
                        totalPagoImpuesto.TotalRetencionesISR = Decimal.ToDouble(totalRetencionesISR);
                        totalPagoImpuesto.TotalTrasladosBaseIVA16 = Decimal.ToDouble(totalTrasladosBaseIVA16);
                        totalPagoImpuesto.TotalTrasladosBaseIVA8 = Decimal.ToDouble(totalTrasladosBaseIVA8);
                        totalPagoImpuesto.TotalTrasladosImpuestoIVA16 = Decimal.ToDouble(totalTrasladosImpuestoIVA16);
                        totalPagoImpuesto.TotalTrasladosImpuestoIVA8 = Decimal.ToDouble(totalTrasladosImpuestoIVA8);
                        totalPagoImpuesto.MontoTotalPagos = montoTotal;
                    if (totalPagoImpuesto.Id > 0)
                    {
                        _db.Entry(totalPagoImpuesto).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
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
