using API.Catalogos;
using API.Enums;
using API.Models.Dto;
using API.Operaciones.ComplementoCartaPorte;
using API.Operaciones.ComplementosPagos;
using API.Operaciones.ComprobantesCfdi;
using API.Operaciones.Facturacion;
using Aplicacion.Context;
using MessagingToolkit.QRCode.Codec;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aplicacion.LogicaPrincipal.GeneracionXSA
{
    public class XsaManager
    {
        private readonly AplicacionContext _db = new AplicacionContext();
        /*private static string pathXml = @"D:\XML-GENERADOS-CARTAPORTE\carta-porteCRANETimbrado.xml";*/

        public int GenerarCFDI(String xml, Sucursal sucursal, int folio, string serie, ComprobanteDto comprobanteDto, String pathXml)
        {
            DataResponseXsaDto data = new DataResponseXsaDto();
            int idFacturaEmitida = 0;
            // string cadenaFormateada = ReplaceCadena(xml); 
            try
            {

                string response = RequestGeneracionCfdi(sucursal, folio, serie, xml);
                if (response == null) { throw new Exception("Error response : null"); }
                //deserealizamos el json response
                data = JsonConvert.DeserializeObject<DataResponseXsaDto>(response);
                //downloda XML
                byte[] cfdi = GetByteCfdiGenerado(data, sucursal);
                //guardar file temp
                //guardar string en un archivo
                //System.IO.File.WriteAllBytes(pathXml, cfdi);

                //guardar facturaEmitida 
                idFacturaEmitida = GuardarComprobante(data, comprobanteDto, sucursal.Id, cfdi);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error al momento de generar: {0}", ex.Message));
            }
            return idFacturaEmitida;
        }

        public string RequestGeneracionCfdi(Sucursal sucursal, int folio, string serie, string path)
        {
            string responseBody = null;
            var tipoCfdXsa = "";
            //URL produccion

            if (sucursal.Id == 29)
            { tipoCfdXsa = "3fe557e55f8691df583d406e53cb7dd6"; }
            else if (sucursal.Id == 1)
            { tipoCfdXsa = "b4b7fb57ebfd8641a054e7905fb6de5b"; }
            else if (sucursal.Id == 52)
            { tipoCfdXsa = "d2e621f71389ff426a62952bc76bd14f"; }
            else if (sucursal.Id == 31) 
            { tipoCfdXsa = "6e421462b79fb7a052713a99fca1836b"; }
            else if(sucursal.Id == 47) 
            { tipoCfdXsa = "862cfe2fd78ec45167399b3a2d98a372"; }
            else 
            { tipoCfdXsa = sucursal.TipoCfdXsa; }

            var urlXsa = $"https://" + sucursal.Servidor + ":9050/" + sucursal.KeyXsa + "/cfdis";
            if (sucursal.TipoCfdXsa == null) { throw new Exception("Error: Tipo CFD XSA NULL"); }
            if (sucursal.IdSucursalXsa == null) { throw new Exception("Error: Id Sucursal XSA NULL"); }
            var request = (HttpWebRequest)WebRequest.Create(urlXsa);
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            //nombre del archivo xml
            string nameFile = String.Format("{0}{1}_{2}.xml", serie, folio, sucursal.Rfc);

            //Objeto de produccion
            var dataJsonXsa = new DataApiXSA()
            {
                idSucursal = sucursal.IdSucursalXsa,
                idTipoCfd = tipoCfdXsa,
                nombre = nameFile,
                archivoFuente = path
            };

            //serealizamos json
            string jsonString = System.Text.Json.JsonSerializer.Serialize(dataJsonXsa);
            var objetoDeserializado = JsonConvert.DeserializeObject<DataApiXSA>(jsonString);
            // Serializa nuevamente el objeto sin escapes
            string cadenaJsonSinEscapes = JsonConvert.SerializeObject(objetoDeserializado);
            //enviaamos request
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(cadenaJsonSinEscapes);
                streamWriter.Flush();
                streamWriter.Close();
            }
            try
            {
                using (WebResponse response = request.GetResponse())
                {
                    using (Stream strReader = response.GetResponseStream())
                    {
                        if (strReader == null) { throw new Exception("Error al momento de obtener el response: null"); };
                        using (StreamReader objReader = new StreamReader(strReader))
                        {
                            responseBody = objReader.ReadToEnd();

                        }
                    }
                }
            }
            catch (WebException ex)
            {
                string message = "";
                using (WebResponse response = ex.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    message = httpResponse.StatusCode.ToString();
                    using (Stream data = response.GetResponseStream())
                    using (var reader = new StreamReader(data))
                    {
                        message += ":" + reader.ReadToEnd();
                    }
                }
                throw new Exception("Error: " + message);
            }

            return responseBody;
        }
        private static string RemoveEscapes(string json)
        {
            // Utilizar una expresión regular para quitar los escapes "\"
            // de cada campo del JSON
            return Regex.Replace(json, @"\\", "");
        }
        private int GuardarComprobante(DataResponseXsaDto data, ComprobanteDto comprobanteDto, int sucursalId, byte[] xml)
        {
            var utf8 = new UTF8Encoding();
            var facturaInternaEmitida = new FacturaEmitida
            {
                ComplementosPago = new List<ComplementoPago>(),
                EmisorId = sucursalId,
                ReceptorId = comprobanteDto.ReceptorId,
                Fecha = comprobanteDto.FechaDocumento,
                Folio = data.folio.ToString(),
                Moneda = (c_Moneda)comprobanteDto.Moneda,
                Serie = data.serie,
                Subtotal = (double)comprobanteDto.Subtotal,
                TipoCambio = comprobanteDto.TipoCambio,
                TipoComprobante = comprobanteDto.TipoComprobante,
                Total = comprobanteDto.Total,
                TotalImpuestosTrasladados = comprobanteDto.TotalImpuestoTrasladado,
                TotalImpuestosRetenidos = comprobanteDto.TotalImpuestoRetenidos,
                Uuid = data.uuid,
                ArchivoFisicoXml = xml,
                CodigoQR = GeneraQR(comprobanteDto, data),
                Status = API.Enums.Status.Activo,
                ReferenciaAddenda = comprobanteDto.Referencia
            };
            if (comprobanteDto.FormaPago != null)
            {
                facturaInternaEmitida.FormaPago = comprobanteDto.FormaPago;
            }
            else { facturaInternaEmitida.FormaPago = null; }
            if (comprobanteDto.MetodoPago != null)
            {
                facturaInternaEmitida.MetodoPago = (c_MetodoPago)Enum.ToObject(typeof(c_MetodoPago), comprobanteDto.MetodoPago);
            }
            else { facturaInternaEmitida.MetodoPago = null; }

            _db.FacturasEmitidas.Add(facturaInternaEmitida);
            _db.SaveChanges();

            return facturaInternaEmitida.Id;
        }

        public byte[] GeneraQR(ComprobanteDto comprobanteDto, DataResponseXsaDto data)
        {
            //string URL
            string str1 = string.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//Temp//QR-{0}.jpg", (object)DateTime.Now.ToString("yyyyMMddHHmmssffff"));
            //string data
            string str2 = string.Format("?re={0}&rr={1}&tt={2:0000000000.000000}&id={3}", (object)comprobanteDto.RfcSucursal, (object)comprobanteDto.RfcReceptor,
                (object)comprobanteDto.Total, (object)data.uuid);
            //genera QR Temp
            new QRCodeEncoder().Encode(str2).Save(str1);

            //get byte imagen
            byte[] imgdata = System.IO.File.ReadAllBytes(str1);

            return imgdata;
        }

        public String ReplaceCadena(string cadena)
        {
            string formatString = cadena.Replace("\\u0022", "\"");
            formatString = Regex.Unescape(formatString);
            return formatString;
        }
        public byte[] GetByteCfdiGenerado(DataResponseXsaDto data, Sucursal sucursal)
        {
            byte[] xml = new byte[1024];
            //url api xsa dowload Files prueba
            //var url = urlXsaPrueba + data.xmlDownload;
            //url produccion
            var url = $"https://" + sucursal.Servidor + ":9050" + data.xmlDownload;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";


            try
            {
                //get byte response
                WebResponse response = request.GetResponse();
                MemoryStream ms = new MemoryStream();
                response.GetResponseStream().CopyTo(ms);

                xml = ms.ToArray();


            }
            catch (WebException ex)
            {
                throw new Exception(ex.Message);
            }
            return xml;
        }
    }




}
