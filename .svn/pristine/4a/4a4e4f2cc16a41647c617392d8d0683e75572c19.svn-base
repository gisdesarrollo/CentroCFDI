using API.Operaciones.ComplementosPagos;
using API.Operaciones.Facturacion;
using Aplicacion.Context;
using Aplicacion.LogicaPrincipal.Facturas;
using CFDI.API.CFDI33.CFDI;
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

        public List<ComplementoPago> Filtrar(DateTime fechaInicial, DateTime fechaFinal, int sucursalId)
        {
            var complementos = _db.ComplementosPago.Where(cp => cp.SucursalId == sucursalId && cp.FechaDocumento >= fechaInicial && cp.FechaDocumento <= fechaFinal).ToList();
            return complementos;
        }

        public FacturaEmitida Decodificar(String pathXml)
        {
            Comprobante comprobante;

            var xml = _operacionesStreams.ArchivoByteArray(pathXml);
            try
            {
                comprobante = _decodificar.DecodificarComprobante(pathXml);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            try
            {
                _validar.ChecarUuidRepetido(comprobante);
            }
            catch (Exception ex)
            {
                File.Delete(pathXml);
                throw ex;
            }

            try
            {
                _validar.ChecarRfcReceptor(comprobante);
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

            FacturaEmitida factura;
            try
            {
                factura = _guardar.GuardarFacturaEmitida(comprobante, xml);
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
