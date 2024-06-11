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
        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();
        private readonly Decodificar _decodificar = new Decodificar();

        //private static string urlPruebas = $"http://services.test.sw.com.mx";
        private static string urlProduccion = $"https://services.sw.com.mx";

        //private static string userPruebas = "eduardo.ayala@gisconsultoria.com";
        //private static string passwordPruebas = "Dr5$%5jHefg9";
        private static string user = "desarrollo@gisconsultoria.com";

        private static string password = "GI/2201*qA";

        #endregion Variables

        public List<API.Operaciones.OperacionesProveedores.DocumentoRecibido> FiltrarDocumentos(DateTime fechaInicial, DateTime fechaFinal, int sucursalId, int usuarioId)
        {
            var usuario = _db.Usuarios.Find(usuarioId);

            if (usuario == null)
            {
                // Manejar el caso donde el usuario no se encuentra
                return new List<API.Operaciones.OperacionesProveedores.DocumentoRecibido>();
            }

            // Ajustar las fechas
            fechaInicial = fechaInicial.Date; // Establece la hora en 00:00:00
            fechaFinal = fechaFinal.Date.AddDays(1).AddTicks(-1); // Establece la hora en 23:59:59


            // Base query
            var query = _db.DocumentosRecibidos.AsQueryable();

            // Filtro común
            query = query.Where(dr =>
                                dr.FechaEntrega >= fechaInicial &&
                                dr.FechaEntrega <= fechaFinal &&
                                dr.SucursalId == sucursalId &&
                                dr.RecibidosComprobante.TipoComprobante != API.Enums.c_TipoDeComprobante.P &&
                                dr.EstadoComercial == API.Enums.c_EstadoComercial.Aprobado);

            // Filtro adicional si el usuario es proveedor
            if (usuario.esProveedor)
            {
                query = query.Where(dr => dr.SocioComercialId == usuario.SocioComercialId);
            }

            // Ejecutar la consulta y ordenar
            var documentoRecibido = query
                                    .OrderBy(dr => dr.EstadoPago)
                                    .ToList();

            return documentoRecibido;
        }

        public List<PagosDR> Filtrar(DateTime fechaInicial, DateTime fechaFinal, bool esProveedor, int? socioComercialId, int sucursalId)
        {
            var pagos = new List<PagosDR>();

            if (esProveedor)
            {
                pagos = _db.PagoDr
                        .Where(dr => dr.FechaPago >= fechaInicial && dr.FechaPago <= fechaFinal && dr.SucursalId == sucursalId && dr.SocioComercialId == socioComercialId)
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
