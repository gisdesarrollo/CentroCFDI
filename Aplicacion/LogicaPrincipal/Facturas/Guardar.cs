using API.Catalogos;
using API.Operaciones.Facturacion;
using Aplicacion.Context;
using CFDI.API.CFDI33.CFDI;
using System;
using System.Linq;
using Utilerias.LogicaPrincipal;

namespace Aplicacion.LogicaPrincipal.Facturas
{
    public class Guardar
    {
        #region Variables

        private readonly Decodificar _decodificar = new Decodificar();
        private readonly AplicacionContext _db = new AplicacionContext();
        private readonly OperacionesStreams _operacionesStreams = new OperacionesStreams();

        #endregion

        public FacturaEmitida GuardarFacturaEmitida(Comprobante comprobante, byte[] xml)
        {
            var facturaEmitida = new FacturaEmitida
            {
                Fecha = Convert.ToDateTime(comprobante.Fecha),
                Folio = comprobante.Folio,
                FormaPago = comprobante.FormaPago,
                MetodoPago = comprobante.MetodoPago,
                Moneda = comprobante.Moneda,
                Serie = comprobante.Serie,
                Subtotal = Convert.ToDouble(comprobante.SubTotal),
                TipoCambio = Convert.ToDouble(comprobante.TipoCambio),
                TipoComprobante = comprobante.TipoDeComprobante,
                Total = Convert.ToDouble(comprobante.Total),
                Uuid = _decodificar.DecodificarTimbre(comprobante).UUID,
                Version = comprobante.Version,
                ArchivoFisicoXml = xml,

                //TODO: Arreglar esta fecha decodificando el TFD
                FechaTimbrado = DateTime.Now
            };

            if(comprobante.Impuestos != null)
            {
                facturaEmitida.TotalImpuestosRetenidos = Convert.ToDouble(comprobante.Impuestos.TotalImpuestosRetenidos ?? "0.0");
                facturaEmitida.TotalImpuestosTrasladados = Convert.ToDouble(comprobante.Impuestos.TotalImpuestosTrasladados ?? "0.0");
            }

            var emisor = _db.Sucursales.FirstOrDefault(s => s.Rfc == comprobante.Emisor.Rfc);
            if(emisor == null)
            {
                throw new Exception(String.Format("No se encontró el RFC del emisor {0}", comprobante.Emisor.Rfc));
            }

            Cliente receptor;
            if (comprobante.Receptor.Rfc == "XEXX010101000" || comprobante.Receptor.Rfc == "XAXX010101000")
            {
                receptor = _db.Clientes.FirstOrDefault(s => s.Rfc == comprobante.Receptor.Rfc && s.RazonSocial == comprobante.Receptor.Nombre && s.SucursalId == emisor.Id);
                if (receptor == null)
                {
                    throw new Exception(String.Format("No se encontró receptor con RFC {0} y Razón Social {1}", comprobante.Emisor.Rfc, comprobante.Emisor.Nombre));
                }
            }
            else
            {
                receptor = _db.Clientes.FirstOrDefault(s => s.Rfc == comprobante.Receptor.Rfc && s.SucursalId == emisor.Id);
                if (receptor == null)
                {
                    throw new Exception(String.Format("No se encontró receptor con RFC {0}", comprobante.Emisor.Rfc));
                }
            }

            //Asignaciones Facturas
            facturaEmitida.EmisorId = emisor.Id;
            facturaEmitida.ReceptorId = receptor.Id;

            facturaEmitida.Emisor = null;
            facturaEmitida.Receptor = null;

            _db.FacturasEmitidas.Add(facturaEmitida);
            _db.SaveChanges();

            return facturaEmitida;
        }

        public FacturaRecibida GuardarFacturaRecibida(Comprobante comprobante, byte[] xml)
        {
            var facturaRecibida = new FacturaRecibida
            {
                Fecha = Convert.ToDateTime(comprobante.Fecha),
                Folio = comprobante.Folio,
                FormaPago = comprobante.FormaPago,
                MetodoPago = comprobante.MetodoPago,
                Moneda = comprobante.Moneda,
                Serie = comprobante.Serie,
                Subtotal = Convert.ToDouble(comprobante.SubTotal),
                TipoCambio = Convert.ToDouble(comprobante.TipoCambio),
                TipoComprobante = comprobante.TipoDeComprobante,
                TotalImpuestosRetenidos = Convert.ToDouble(comprobante.Impuestos.TotalImpuestosRetenidos ?? "0.0"),
                TotalImpuestosTrasladados = Convert.ToDouble(comprobante.Impuestos.TotalImpuestosTrasladados ?? "0.0"),
                Total = Convert.ToDouble(comprobante.Total),
                Uuid = _decodificar.DecodificarTimbre(comprobante).UUID,
                ArchivoFisicoXml = xml,

                FechaTimbrado = DateTime.Now
            };

            var receptor = _db.Sucursales.First(s => s.Rfc == comprobante.Receptor.Rfc);
            var emisor = _db.Proveedores.First(s => s.Rfc == comprobante.Emisor.Rfc);

            //Asignaciones Facturas
            facturaRecibida.EmisorId = emisor.Id;
            facturaRecibida.ReceptorId = receptor.Id;

            facturaRecibida.Emisor = null;
            facturaRecibida.Receptor = null;

            _db.FacturasRecibidas.Add(facturaRecibida);
            _db.SaveChanges();

            return facturaRecibida;
        }
    }
}
