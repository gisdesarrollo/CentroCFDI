using API.Catalogos;
using API.Operaciones.Facturacion;
using CFDI.API.CFDI33.CFDI;
using CFDI.API.Complementos.Pagos10;
using CFDI.API.Complementos.Timbre11;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Aplicacion.LogicaPrincipal.Validacion
{
    public class DecodificaFacturas
    {
        public void DecodificarFactura(ref FacturaRecibida facturaRecibida, String pathCompleto)
        {
            var serializador = new XmlSerializer(typeof(Comprobante));
            var comprobante = (Comprobante)serializador.Deserialize(new MemoryStream(facturaRecibida.ArchivoFisicoXml));

            //Complementos
            XmlElement timbreFiscalDigitalFisico = null;
            XmlElement complementoPagosFisico = null;

            foreach (var complemento in comprobante.Complemento)
            {
                timbreFiscalDigitalFisico = complemento.Any.FirstOrDefault(p => p.OuterXml.Contains("tfd"));
                complementoPagosFisico = complemento.Any.FirstOrDefault(p => p.OuterXml.Contains("pago10"));
            }

            var timbreFiscalDigital = new TimbreFiscalDigital();
            if (timbreFiscalDigitalFisico != null)
            {
                timbreFiscalDigital = ObtenerComplemento<TimbreFiscalDigital>(timbreFiscalDigitalFisico);
            }

            var complementoPagos = new Pagos();
            if (complementoPagosFisico != null)
            {
                complementoPagos = ObtenerComplemento<Pagos>(complementoPagosFisico);
            }

            //Datos
            facturaRecibida.Fecha = Convert.ToDateTime(comprobante.Fecha);
            facturaRecibida.NoCertificado = comprobante.NoCertificado;
            facturaRecibida.TipoComprobante = comprobante.TipoDeComprobante;
            facturaRecibida.Version = comprobante.Version;
            facturaRecibida.LugarExpedicion = comprobante.LugarExpedicion;
            facturaRecibida.FormaPago = comprobante.FormaPago;
            facturaRecibida.MetodoPago = comprobante.MetodoPago;
            facturaRecibida.Serie = comprobante.Serie;
            facturaRecibida.Folio = comprobante.Folio;
            
            //Totales
            facturaRecibida.Subtotal = Convert.ToDouble(comprobante.SubTotal);
            facturaRecibida.Total = Convert.ToDouble(comprobante.Total);
            
            //Timbrado
            facturaRecibida.FechaTimbrado = Convert.ToDateTime(timbreFiscalDigital.FechaTimbrado);
            facturaRecibida.NoCertificadoSat = timbreFiscalDigital.NoCertificadoSAT;
            facturaRecibida.SelloDigitalCfdi = timbreFiscalDigital.SelloCFD;
            facturaRecibida.SelloSat = timbreFiscalDigital.SelloSAT;
            facturaRecibida.Certificado = comprobante.Certificado;
            facturaRecibida.Uuid = timbreFiscalDigital.UUID;

            facturaRecibida.Emisor = new Proveedor
            {
                RazonSocial = comprobante.Emisor.Nombre,
                Rfc = comprobante.Emisor.Rfc
            };

            facturaRecibida.Receptor = new Sucursal
            {
                Rfc = comprobante.Receptor.Rfc
            };

            try
            {
                facturaRecibida.Descuento = Convert.ToDouble(comprobante.Descuento);
            }
            catch (Exception)
            {
            }
        }

        #region Funciones

        private T ObtenerComplemento<T>(XmlElement element)
        {
            var serializador = new XmlSerializer(typeof(T));
            var nodo = (T)serializador.Deserialize(new XmlNodeReader(element));
            return nodo;
        }

        #endregion

    }
}
