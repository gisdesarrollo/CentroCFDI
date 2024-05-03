using API.Operaciones.OperacionesProveedores;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.LogicaPrincipal.Validacion
{
    public class ValidacionesPagos
    {
        public String ValidaComplementoPago(PagosDR pago, ComprobanteCFDI cfdi)
        {
            StringBuilder sb = new StringBuilder();
            if (cfdi.Pagos.Pago.Length < 2)
            {
                foreach (var pagoXml in cfdi.Pagos.Pago)
                {
                    if (pago.DocumentoRecibido.SocioComercial.RazonSocial != cfdi.Emisor.Nombre)
                    {
                        sb.Append("error" + ":" + "Emisor no coincide con el complemento cargado" + ",");
                    }
                    else { sb.Append("ok" + ":" + "Emisor coincide correctamente" + ","); }
                    if (pago.DocumentoRecibido.Sucursal.RazonSocial != cfdi.Receptor.Nombre)
                    {
                        sb.Append("error" + ":" + "Receptor no coincide con el complemento cargado" + ",");
                    }
                    else { sb.Append("ok" + ":" + "Receptor coincide correctamente" + ","); }
                    if (pago.DocumentoRecibido.RecibidosComprobante.FormaPago != pagoXml.FormaDePagoP)
                    {
                        sb.Append("error" + ":"+ "Forma de Pago no coincide con el complemento cargado"  +",");
                    }
                    else { sb.Append("ok" + ":" + "Forma de Pago coincide correctamente" + ","); }

                    if (pago.DocumentoRecibido.RecibidosComprobante.TipoComprobante != cfdi.TipoDeComprobante)
                    {
                        sb.Append("error" + ":" + "Tipo comprobante no coincide con el complemento cargado" + ",");
                    }
                    else { sb.Append("ok" + ":" + "Tipo comprobante coincide correctamente" + ","); }

                    if (pago.TipoCambio != (double)pagoXml.TipoCambioP)
                    {
                        sb.Append("error" + ":" + "Tipo cambio no coincide con el complemento cargado" + ",");
                    }
                    else { sb.Append("ok" + ":" + "Tipo cambio coincide correctamente" + ","); }

                    if (pago.Moneda != pagoXml.MonedaP)
                    {
                        sb.Append("error" + ":" + "Moneda no coincide con el complemento cargado" + ",");
                    }
                    else { sb.Append("ok" + ":" + "Moneda coincide correctamente" + ","); }

                    if (pago.Total != (double)pagoXml.Monto)
                    {
                        sb.Append("error" + ":" + "Monto no coincide con el complemento cargado" + ",");
                    }
                    else { sb.Append("ok" + ":" + "Monto coincide correctamente" + ","); }

                    if (pago.DocumentoRecibido.RecibidosComprobante.Serie != cfdi.Serie)
                    {
                        sb.Append("error" + ":" + "Serie no coincide con el complemento cargado" + ",");
                    }
                    else { sb.Append("ok" + ":" + "Serie coincide correctamente" + ","); }

                    if (pago.DocumentoRecibido.RecibidosComprobante.Folio != cfdi.Folio)
                    {
                        sb.Append("error" + ":" + "Folio no coincide con el complemento cargado" + ",");
                    }
                    else { sb.Append("ok" + ":" + "Folio coincide correctamente" + ","); }

                    if (pago.DocumentoRecibido.CfdiRecibidos_UUID != cfdi.TimbreFiscalDigital.UUID)
                    {
                        sb.Append("error" + ":" + "UUID no coincide con el complemento cargado" + ",");
                    }
                    else { sb.Append("ok" + ":" + "UUID coincide correctamente" + ","); }
                    DateTime fechaPagoXml = DateTime.ParseExact(pagoXml.FechaPago.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    if (pago.FechaPago.Date != fechaPagoXml.Date)
                    {
                        sb.Append("error" + ":" + "Fecha pago no coincide con el complemento cargado"+",");
                    }
                    else { sb.Append("ok" + ":" + "Fecha pago coincide correctamente"+ ","); }
                }

            }
            return sb.ToString();
;        }
    }
}
