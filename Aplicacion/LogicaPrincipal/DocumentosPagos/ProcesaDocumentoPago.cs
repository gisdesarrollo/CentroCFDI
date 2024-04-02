using API.Operaciones.OperacionesProveedores;
using Aplicacion.Context;
using Aplicacion.LogicaPrincipal.Facturas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.LogicaPrincipal.DocumentosPagos
{
    public class ProcesaDocumentoPago
    {
        #region variables
        private readonly AplicacionContext _db = new AplicacionContext();
        private readonly Decodificar _decodificar = new Decodificar();
        #endregion
        public List<PagosDR> Filtrar(DateTime fechaInicial, DateTime fechaFinal, bool esProveedor, int? socioComercialId, int sucursalId)
        {
            var pagos = new List<PagosDR>();

            if (esProveedor)
            {
                pagos = _db.PagoDr
                        .Where(dr => dr.FechaPago >= fechaInicial && dr.FechaPago <= fechaFinal && dr.SucursalId == sucursalId && dr.SocioComercial_Id == socioComercialId)
                        .ToList();
            }
            else
            {

                pagos = _db.PagoDr
                        .Where(dr => dr.FechaPago >= fechaInicial && dr.FechaPago <= fechaFinal && dr.SucursalId == sucursalId)
                        .ToList();
            }

            return pagos;
        }

        public ComprobanteCFDI DecodificaXML(String pathXml)
        {
            ComprobanteCFDI comprobante40 = new ComprobanteCFDI();
            var version = string.Empty;
            try
            {
                version = _decodificar.ObtenerPropiedad(pathXml, "cfdi:Comprobante", "Version");

                if (version == "4.0")
                {
                    comprobante40 = _decodificar.DecodificarComprobante40(pathXml, version);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return comprobante40;
        }
    }

}
