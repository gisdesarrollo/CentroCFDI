using API.Models.Dto;
using API.Operaciones.OperacionesProveedores;
using Aplicacion.Context;
using Aplicacion.LogicaPrincipal.Facturas;
using Newtonsoft.Json;
using SW.Services.Authentication;
using SW.Services.Validate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.LogicaPrincipal.DocumentosRecibidos
{
    public class ProcesaDocumentoRecibido
    {
        #region Variables
        private readonly AplicacionContext _db = new AplicacionContext();
        private readonly Decodificar _decodificar = new Decodificar();
        private static string urlPruebas = $"http://services.test.sw.com.mx";
        private static string urlProduccion = $"https://services.test.sw.com.mx";
        private static string user = "eduardo.ayala@gisconsultoria.com";
        private static string password = "Dr5$%5jHefg9";
        #endregion

        public List<DocumentosRecibidosDR> Filtrar(DateTime fechaInicial, DateTime fechaFinal, int usuarioId, int? socioComercialId)
        {
            var usuario = _db.Usuarios.Find(usuarioId);
            var documentoRecibido = new List<DocumentosRecibidosDR>();
            List<DocumentosRecibidosDR> documentoRecibidoAprobador = new List<DocumentosRecibidosDR>();
            List<DocumentosRecibidosDR> documentoRecibidos = new List<DocumentosRecibidosDR>();
            if (socioComercialId != null)
            {
                documentoRecibido = _db.DocumentoRecibidoDr
                    .Where(dr => dr.FechaEntrega >= fechaInicial && dr.FechaEntrega <= fechaFinal && dr.Usuario_Id == usuarioId && dr.SocioComercial_Id == socioComercialId)
                    .OrderBy(dr => dr.EstadoComercial)
                    .ToList();
            }
            else
            {
                
                documentoRecibidoAprobador = _db.DocumentoRecibidoDr
                    .Where(dr => dr.FechaEntrega >= fechaInicial && dr.FechaEntrega <= fechaFinal && dr.Aprobador_Id == usuarioId)
                    .OrderBy(dr => dr.EstadoComercial)
                    .ToList();
                
                documentoRecibidos = _db.DocumentoRecibidoDr
                    .Where(dr => dr.FechaEntrega >= fechaInicial && dr.FechaEntrega <= fechaFinal && dr.Usuario_Id == usuarioId)
                    .OrderBy(dr => dr.EstadoComercial)
                    .ToList();
                
                documentoRecibido.AddRange(documentoRecibidoAprobador);
                documentoRecibido.AddRange(documentoRecibidos);
                
            }
            return documentoRecibido;
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

        public AuthResponse GetToken()
        {

            AuthResponse response = new AuthResponse();
            try
            {
                Authentication auth = new Authentication(urlPruebas, user, password);
                response = auth.GetToken();
                if (response.status == "error")
                {
                    throw new Exception(String.Format("Error al momento de autenticar: {0}", response.message));
                }
            }
            catch (WebException ex)
            {
                throw new Exception(ex.Message);
            }

            return response;
        }

        public ValidateXmlResponse ValidaCfdi(String token, String pathXml)
        {

            ValidateXmlResponse response = new ValidateXmlResponse();

            try
            {
                //Creamos una instancia de tipo Validate
                Validate validate = new Validate(urlPruebas, token);
                //var xml = GetXml(build);
                string contents = System.IO.File.ReadAllText(pathXml);
                response = validate.ValidateXml(contents);
                if (response.status == "error")
                {
                    throw new Exception(String.Format("Error al momento de validar el CFDI: {0}", response.message));
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return response;
        }

    }
}
