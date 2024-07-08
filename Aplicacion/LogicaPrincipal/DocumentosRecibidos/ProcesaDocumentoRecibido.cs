using Aplicacion.Context;
using Aplicacion.LogicaPrincipal.Facturas;
using SW.Services.Authentication;
using SW.Services.Taxpayer;
using SW.Services.Validate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Schema;

namespace Aplicacion.LogicaPrincipal.DocumentosRecibidos
{
    public class ProcesaDocumentoRecibido
    {
        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();
        private readonly Decodificar _decodificar = new Decodificar();

        //private static string urlPruebas = $"http://services.test.sw.com.mx";
        private static string urlProduccion = $"https://services.sw.com.mx";
        private static string xsdFilePath = "cfdv40.xsd";
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
                                dr.RecibidosComprobante.TipoComprobante != API.Enums.c_TipoDeComprobante.P);

            // Filtro adicional si el usuario es proveedor
            if (usuario.esProveedor)
            {
                query = query.Where(dr => dr.SocioComercialId == usuario.SocioComercialId);
            }

            // Ejecutar la consulta y ordenar
            var documentoRecibido = query
                                    .OrderBy(dr => dr.EstadoComercial)
                                    .ToList();

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
                Authentication auth = new Authentication(urlProduccion, user, password);
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
                Validate validate = new Validate(urlProduccion, token);
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

        public TaxpayerResponse ValidaRFC(String token, String rfc)
        {
            TaxpayerResponse response = new TaxpayerResponse();
            try
            {
                Taxpayer taxpayer = new Taxpayer(urlProduccion, token);
                response = taxpayer.GetTaxpayer(rfc);
                if (response.status == "error")
                {
                    throw new Exception(String.Format("Error al momento de validar el RFC: {0}", response.message));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return response;
        }

        public string ValidaEstructuraCfdi(String pathXml)
        {
           // Construir la ruta relativa usando String.Format o directamente
            string relativePath = Path.Combine("Archivos", "Xsd", xsdFilePath);

            // Obtener la ruta base del directorio de la aplicación
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Obtener la ruta completa del archivo
            string xsdFullPath = Path.Combine(baseDirectory, relativePath);
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Schemas.Add(null, xsdFullPath);
            settings.ValidationType = ValidationType.Schema;
            // Configurar la validación
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationEventHandler);

            // Habilitar el procesamiento de DTD
            settings.DtdProcessing = DtdProcessing.Parse;
           
            using (XmlReader reader = XmlReader.Create(pathXml, settings))
            {
               
                try
                {
                    while (reader.Read()) {}
                  
                }
                catch (XmlException ex)
                {
                    throw new Exception(String.Format($"Error de validación XML: El XML contiene errores no es un cfdi correcto - {ex.Message}"));
                }
                catch (XmlSchemaValidationException ex)
                {
                    throw new Exception(String.Format($"Error de esquema XML: {ex.Message}"));
                }
                catch (Exception)
                {
                    throw new Exception(String.Format("Error XML incorrecto: El XML cargado no es un cfdi"));
                }
            }
            return "";
        }
        private static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
                Console.WriteLine($"Advertencia: {e.Message}");
            else if (e.Severity == XmlSeverityType.Error)
                Console.WriteLine($"Error: {e.Message}");
        }

    }
}