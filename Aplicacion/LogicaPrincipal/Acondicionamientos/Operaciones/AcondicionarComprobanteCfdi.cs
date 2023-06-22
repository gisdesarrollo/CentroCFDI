using API.Enums;
using API.Operaciones.ComprobantesCfdi;
using Aplicacion.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.LogicaPrincipal.Acondicionamientos.Operaciones
{
    public class AcondicionarComprobanteCfdi
    {
        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion

        public void CargaInicial(ref ComprobanteCfdi comprobanteCfdi)
        {
            comprobanteCfdi.FacturaEmitida = null;
            //Cfdi Global
            var cliente = _db.Clientes.Find(comprobanteCfdi.ReceptorId);
            if (cliente != null)
            {
                if (cliente.Rfc != "XAXX010101000" && cliente.RazonSocial != "PUBLICO EN GENERAL")
                {
                    comprobanteCfdi.Periodicidad = null;
                    comprobanteCfdi.Meses = null;
                    comprobanteCfdi.Ano = null;
                }
            }
            
            //carga conceptos
            if (comprobanteCfdi.Conceptoss != null)
            {
                foreach (var concepto in comprobanteCfdi.Conceptoss)
                {
                   
                    concepto.ComplementoCP = null;
                    concepto.ComprobanteCfdi = null;
                    //concepto.ComplementoCP = complementoCP;
                    if (concepto.Retencion == null)
                    {

                        concepto.Retencion = null;
                    }
                    else
                    {
                        if (concepto.Retencion.Importe > 0)
                        {
                            comprobanteCfdi.TotalImpuestoRetenidos += decimal.Round(concepto.Retencion.Importe, 2);
                        }
                        else { concepto.Retencion = null; }
                    }
                    if (concepto.Traslado == null)
                    {

                        concepto.Traslado = null;
                    }
                    else
                    {
                        if (concepto.Traslado.Importe >= 0 && concepto.Traslado.Base > 0)
                        {
                            comprobanteCfdi.TotalImpuestoTrasladado += decimal.Round(concepto.Traslado.Importe, 2);
                        }
                        else { concepto.Traslado = null; }
                    }
                }
                comprobanteCfdi.Conceptos = null;

            }
            
        }

        public void CargaRelacion(ComprobanteCfdi comprobanteCfdi)
        {
            //Cfdi Global
            var cliente = _db.Clientes.Find(comprobanteCfdi.ReceptorId);
            if (cliente != null)
            {
                if (cliente.Rfc != "XAXX010101000" && cliente.RazonSocial != "PUBLICO EN GENERAL")
                {
                    comprobanteCfdi.Periodicidad = null;
                    comprobanteCfdi.Meses = null;
                    comprobanteCfdi.Ano = null;
                }
            }
            //carga conceptos
            if (comprobanteCfdi.Conceptoss != null)
            {
                var idsBorrar = comprobanteCfdi.Conceptoss.Select(e => e.Id);
                var comprobanteAnteriores = _db.Conceptos.Where(es => es.Comprobante_Id == comprobanteCfdi.Id && !idsBorrar.Contains(es.Id)).ToList();
                if(comprobanteAnteriores.Count > 0)
                {
                    decimal importeR = 0;
                    decimal importeT = 0;
                    comprobanteAnteriores.ForEach(p => { 
                        if (p.Retencion_Id != null) { importeR = p.Retencion.Importe; }
                        if (p.Traslado_Id != null) { importeT = p.Traslado.Importe; }
                    });
                    if (comprobanteCfdi.TotalImpuestoTrasladado != 0)
                    {
                        comprobanteCfdi.TotalImpuestoTrasladado -= importeT;
                    }
                    if (comprobanteCfdi.TotalImpuestoRetenidos != 0)
                    {
                        comprobanteCfdi.TotalImpuestoRetenidos -= importeR;
                    }
                }
                _db.Conceptos.RemoveRange(comprobanteAnteriores);
                _db.SaveChanges();
                
                foreach (var concepto in comprobanteCfdi.Conceptoss.Except(comprobanteAnteriores))
                {
                    concepto.Comprobante_Id = comprobanteCfdi.Id;
                    concepto.Complemento_Id = null;
                    concepto.ComplementoCP = null;
                    concepto.ComprobanteCfdi = null;
                    if (concepto.Traslado == null)
                    {
                        concepto.Traslado = null;
                    }
                    else
                    {
                        if (concepto.Traslado.Importe >= 0 && concepto.Traslado.Base > 0 && concepto.Traslado_Id == null)
                        {
                            comprobanteCfdi.TotalImpuestoTrasladado += concepto.Traslado.Importe;
                        }
                        else { concepto.Traslado = null; }
                    }

                    if (concepto.Retencion == null)
                    {
                        concepto.Retencion = null;
                    }
                    else
                    {
                        if (concepto.Retencion.Importe > 0 && concepto.Retencion_Id == null)
                        {
                            comprobanteCfdi.TotalImpuestoRetenidos += concepto.Retencion.Importe;
                        }
                        else { concepto.Retencion = null; }
                    }


                    _db.Conceptos.AddOrUpdate(concepto);
                    try
                    {
                        _db.SaveChanges();
                    }
                    catch (System.Exception)
                    {
                    }
                }

            }
            else
            {
                var conceptosAnteriores = _db.Conceptos.Where(es => es.Comprobante_Id == comprobanteCfdi.Id).ToList();

                _db.Conceptos.RemoveRange(conceptosAnteriores);
                _db.SaveChanges();
            }
        }

        public void CargaValidacion(ref ComprobanteCfdi comprobanteCfdi)
        {
            comprobanteCfdi.Total = 0;
            //calcula total impuestos
            if (comprobanteCfdi.TotalImpuestoRetenidos > 0 && comprobanteCfdi.TotalImpuestoTrasladado > 0)
            {
                comprobanteCfdi.Total += (comprobanteCfdi.Subtotal + comprobanteCfdi.TotalImpuestoTrasladado) - comprobanteCfdi.TotalImpuestoRetenidos;
            }
            else if (comprobanteCfdi.TotalImpuestoTrasladado > 0)
            {
                comprobanteCfdi.Total += comprobanteCfdi.Subtotal + comprobanteCfdi.TotalImpuestoTrasladado;
            }
            else if (comprobanteCfdi.TotalImpuestoRetenidos > 0)
            {
                comprobanteCfdi.Total += comprobanteCfdi.Subtotal - comprobanteCfdi.TotalImpuestoRetenidos;
            }
            else
            {
                
                    comprobanteCfdi.Total = comprobanteCfdi.Subtotal;
                
            }
        }
    }
}
