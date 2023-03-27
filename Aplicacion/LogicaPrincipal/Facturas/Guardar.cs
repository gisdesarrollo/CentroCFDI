using API.Catalogos;
using API.Operaciones.Facturacion;
using Aplicacion.Context;
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

        public FacturaEmitida GuardarFacturaEmitida40(ComprobanteCFDI comprobante40, byte[] xml)
        {
            
            var facturaEmitida = new FacturaEmitida
            {
                Fecha = Convert.ToDateTime(comprobante40.Fecha),
                Folio = comprobante40.Folio,
                FormaPago = comprobante40.FormaPago,
                MetodoPago = comprobante40.MetodoPago,
                Moneda = comprobante40.Moneda,
                Serie = comprobante40.Serie,
                Subtotal = Convert.ToDouble(comprobante40.SubTotal),
                TipoCambio = Convert.ToDouble(comprobante40.TipoCambio),
                TipoComprobante = comprobante40.TipoDeComprobante,
                Total = Convert.ToDouble(comprobante40.Total),
                Uuid = _decodificar.DecodificarTimbre(comprobante40,null).UUID,
                Version = comprobante40.Version,
                ArchivoFisicoXml = xml,
                Status = API.Enums.Status.Activo,
                //TODO: Arreglar esta fecha decodificando el TFD
                FechaTimbrado = DateTime.Now
            };

            if(comprobante40.Impuestos != null)
            {
                facturaEmitida.TotalImpuestosRetenidos = Convert.ToDouble(comprobante40.Impuestos.TotalImpuestosRetenidos);
                facturaEmitida.TotalImpuestosTrasladados = Convert.ToDouble(comprobante40.Impuestos.TotalImpuestosTrasladados);
            }

            var emisor = _db.Sucursales.FirstOrDefault(s => s.Rfc == comprobante40.Emisor.Rfc);
            if(emisor == null)
            {
                throw new Exception(String.Format("No se encontró el RFC del emisor {0}", comprobante40.Emisor.Rfc));
            }

            Cliente receptor;
            if (comprobante40.Receptor.Rfc == "XEXX010101000" || comprobante40.Receptor.Rfc == "XAXX010101000")
            {
                receptor = _db.Clientes.FirstOrDefault(s => s.Rfc == comprobante40.Receptor.Rfc && s.RazonSocial == comprobante40.Receptor.Nombre && s.SucursalId == emisor.Id);
                if (receptor == null)
                {
                    throw new Exception(String.Format("No se encontró receptor con RFC {0} y Razón Social {1}", comprobante40.Emisor.Rfc, comprobante40.Emisor.Nombre));
                }
            }
            else
            {
                receptor = _db.Clientes.FirstOrDefault(s => s.Rfc == comprobante40.Receptor.Rfc && s.SucursalId == emisor.Id);
                if (receptor == null)
                {
                    throw new Exception(String.Format("No se encontró receptor con RFC {0}", comprobante40.Emisor.Rfc));
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
        public FacturaEmitida GuardarFacturaEmitida33(ComprobanteCFDI33 comprobante33, byte[] xml)
        {

            var facturaEmitida = new FacturaEmitida
            {
                Fecha = Convert.ToDateTime(comprobante33.Fecha),
                Folio = comprobante33.Folio,
                FormaPago = comprobante33.FormaPago,
                MetodoPago = comprobante33.MetodoPago,
                Moneda = comprobante33.Moneda,
                Serie = comprobante33.Serie,
                Subtotal = Convert.ToDouble(comprobante33.SubTotal),
                TipoCambio = Convert.ToDouble(comprobante33.TipoCambio),
                TipoComprobante = comprobante33.TipoDeComprobante,
                Total = Convert.ToDouble(comprobante33.Total),
                Uuid = _decodificar.DecodificarTimbre(null, comprobante33).UUID,
                Version = comprobante33.Version,
                ArchivoFisicoXml = xml,
                Status = API.Enums.Status.Activo,
                //TODO: Arreglar esta fecha decodificando el TFD
                FechaTimbrado = DateTime.Now
            };

            if (comprobante33.Impuestos != null)
            {
                facturaEmitida.TotalImpuestosRetenidos = Convert.ToDouble(comprobante33.Impuestos.TotalImpuestosRetenidos);
                facturaEmitida.TotalImpuestosTrasladados = Convert.ToDouble(comprobante33.Impuestos.TotalImpuestosTrasladados);
            }

            var emisor = _db.Sucursales.FirstOrDefault(s => s.Rfc == comprobante33.Emisor.Rfc);
            if (emisor == null)
            {
                throw new Exception(String.Format("No se encontró el RFC del emisor {0}", comprobante33.Emisor.Rfc));
            }

            Cliente receptor;
            if (comprobante33.Receptor.Rfc == "XEXX010101000" || comprobante33.Receptor.Rfc == "XAXX010101000")
            {
                receptor = _db.Clientes.FirstOrDefault(s => s.Rfc == comprobante33.Receptor.Rfc && s.RazonSocial == comprobante33.Receptor.Nombre && s.SucursalId == emisor.Id);
                if (receptor == null)
                {
                    throw new Exception(String.Format("No se encontró receptor con RFC {0} y Razón Social {1}", comprobante33.Emisor.Rfc, comprobante33.Emisor.Nombre));
                }
            }
            else
            {
                receptor = _db.Clientes.FirstOrDefault(s => s.Rfc == comprobante33.Receptor.Rfc && s.SucursalId == emisor.Id);
                if (receptor == null)
                {
                    throw new Exception(String.Format("No se encontró receptor con RFC {0}", comprobante33.Emisor.Rfc));
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

        /*public FacturaRecibida GuardarFacturaRecibida(Comprobante comprobante, byte[] xml)
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
        }*/
    }
}
