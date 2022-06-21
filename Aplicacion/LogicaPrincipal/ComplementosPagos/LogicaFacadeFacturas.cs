using API.Enums;
using API.Operaciones.ComplementosPagos;
using API.Operaciones.ComprobantesCfdi;
using API.Operaciones.Facturacion;
using Aplicacion.Context;
using Aplicacion.LogicaPrincipal.Facturas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utilerias.LogicaPrincipal;

namespace Aplicacion.LogicaPrincipal.ComplementosPagos
{
    public class LogicaFacadeFacturas
    {

        #region Variables

        private readonly Validar _validar = new Validar();
        private readonly Guardar _guardar = new Guardar();
        private readonly Decodificar _decodificar = new Decodificar();
        private readonly AplicacionContext _db = new AplicacionContext();
        private readonly OperacionesStreams _operacionesStreams = new OperacionesStreams();

        #endregion

        public List<ComplementoPago> Filtrar(DateTime fechaInicial, DateTime fechaFinal,bool? status, int sucursalId)
        {
            var complementos = new List<ComplementoPago>();
            if (status != null)
            {
                complementos = _db.ComplementosPago.Where(cp => cp.SucursalId == sucursalId && cp.FechaDocumento >= fechaInicial && cp.FechaDocumento <= fechaFinal && cp.Generado == status).ToList();
            }
            else
            {
                complementos = _db.ComplementosPago.Where(cp => cp.SucursalId == sucursalId && cp.FechaDocumento >= fechaInicial && cp.FechaDocumento <= fechaFinal).ToList();
            }
            return complementos;
        }

        public List<ComprobanteCfdi> FiltrarComprobanteCFDI(DateTime fechaInicial, DateTime fechaFinal, bool? status,c_TipoDeComprobante? tipoComprobante , int sucursalId)
        {
            var comprobante = new List<ComprobanteCfdi>();
            if (status != null && tipoComprobante != null)
            {
                comprobante = _db.ComprobantesCfdi.Where(cp => cp.SucursalId == sucursalId && cp.FechaDocumento >= fechaInicial && cp.FechaDocumento <= fechaFinal && cp.Generado == status && cp.TipoDeComprobante == tipoComprobante).ToList();

            }
            else if(status != null)
            {
                comprobante = _db.ComprobantesCfdi.Where(cp => cp.SucursalId == sucursalId && cp.FechaDocumento >= fechaInicial && cp.FechaDocumento <= fechaFinal && cp.Generado == status).ToList();

            }
            else if (tipoComprobante !=null)
            {
                comprobante = _db.ComprobantesCfdi.Where(cp => cp.SucursalId == sucursalId && cp.FechaDocumento >= fechaInicial && cp.FechaDocumento <= fechaFinal && cp.TipoDeComprobante == tipoComprobante).ToList();
            }
            else
            {
                comprobante = _db.ComprobantesCfdi.Where(cp => cp.SucursalId == sucursalId && cp.FechaDocumento >= fechaInicial && cp.FechaDocumento <= fechaFinal).ToList();
            }
            return comprobante;
        }

        public FacturaEmitida Decodificar(String pathXml)
        {
            ComprobanteCFDI comprobante40 = new ComprobanteCFDI();
            ComprobanteCFDI33 comprobante33 = new ComprobanteCFDI33();
            var version = string.Empty;
            var xml = _operacionesStreams.ArchivoByteArray(pathXml);
            try
            {
                version  = _decodificar.ObtenerPropiedad(pathXml, "cfdi:Comprobante", "Version");
                if(version == "3.3") 
                {
                    comprobante33 = _decodificar.DecodificarComprobante33(pathXml,version);
                    comprobante40 = null;
                }
                if(version == "4.0") 
                {
                    comprobante40 = _decodificar.DecodificarComprobante40(pathXml,version);
                    comprobante33 = null;
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

            try
            {
                if (comprobante33 == null && comprobante40 != null)
                {
                    _validar.ChecarUuidRepetido(comprobante40, null);
                }
                if (comprobante33 != null && comprobante40 == null)
                {
                    _validar.ChecarUuidRepetido(null, comprobante33);
                }
            }
            catch (Exception ex)
            {
                File.Delete(pathXml);
                throw ex;
            }

            try
            {
                
                   _validar.ChecarRfcReceptor(comprobante40,comprobante33);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

            try
            {

                //_cfdiInfodextra.Validar(xml, comprobante.Emisor.Rfc);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            FacturaEmitida factura = new FacturaEmitida();
            try
            {
                if (comprobante33 != null && comprobante40 == null) 
                {
                    factura = _guardar.GuardarFacturaEmitida33(comprobante33, xml);
                }
                if (comprobante33 == null && comprobante40 != null)
                {
                    factura = _guardar.GuardarFacturaEmitida40(comprobante40, xml);
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

            File.Delete(pathXml);

            return factura;
        }


    }
}
