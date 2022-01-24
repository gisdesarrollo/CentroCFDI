using API.Operaciones.ComplementoCartaPorte;
using Aplicacion.Context;
using OpenHtmlToPdf;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace Aplicacion.LogicaPrincipal.GeneraPDfCartaPorte
{
    public class CreationFile
    {
        private readonly AplicacionContext _db = new AplicacionContext();
        public ComprobanteCFDI DeserealizarXml(int complementoCartaPorteId)
        {
            var complementoCartaPorte = _db.ComplementoCartaPortes.Find(complementoCartaPorteId);

            ComprobanteCFDI oComprobante;
            XmlSerializer oSerializer = new XmlSerializer(typeof(ComprobanteCFDI));

            using (StreamReader reader = new StreamReader(new MemoryStream(complementoCartaPorte.FacturaEmitida.ArchivoFisicoXml), Encoding.UTF8))
            {
                //aqui deserializamos
                oComprobante = (ComprobanteCFDI)oSerializer.Deserialize(reader);


                //complementos
                foreach (var oComplemento in oComprobante.Complemento)
                {
                    foreach (var oComplementoInterior in oComplemento.Any)
                    {
                        if (oComplementoInterior.Name.Contains("TimbreFiscalDigital"))
                        {

                            XmlSerializer oSerializerComplemento = new XmlSerializer(typeof(TimbreFiscalDigital));
                            using (var readerComplemento = new StringReader(oComplementoInterior.OuterXml))
                            {
                                oComprobante.TimbreFiscalDigital =
                                    (TimbreFiscalDigital)oSerializerComplemento.Deserialize(readerComplemento);
                            }

                        }
                        if (oComplementoInterior.Name.Contains("CartaPorte"))
                        {
                            XmlSerializer oSerializerComplemento = new XmlSerializer(typeof(CartaPorte));
                            using (var readerComplemento = new StringReader(oComplementoInterior.OuterXml))
                            {
                                oComprobante.CartaPorte =
                                    (CartaPorte)oSerializerComplemento.Deserialize(readerComplemento);
                            }
                        }

                    }
                }
            }


            return oComprobante;
        }
        private static string GetStringOfFile(string pathFile)
        {
            string contenido = File.ReadAllText(pathFile);
            return contenido;
        }

        public byte[] GeneraPDF(ComprobanteCFDI oComprobante, int complementoCartaPorteId)
        {
            var complementoCartaPorte = _db.ComplementoCartaPortes.Find(complementoCartaPorteId);
            var sucursal = _db.Sucursales.Find(complementoCartaPorte.SucursalId);
            //add QR
            string base64QR = System.Convert.ToBase64String(complementoCartaPorte.FacturaEmitida.CodigoQR);
            string sQR = System.String.Format("data:image/gif;base64,{0}", base64QR);
            oComprobante.CodigoQR = sQR;

            //logo
            if (sucursal.Logo != null)
            {
                string base64Logo = System.Convert.ToBase64String(sucursal.Logo);
                string sLogo = System.String.Format("data:image/gif;base64,{0}", base64Logo);
                oComprobante.Logo = sLogo;
            }
            //Razor Html a PDF

            string path = AppDomain.CurrentDomain.BaseDirectory;

            string pathHTMLTemp = path + "//Content//PDFGenerados//htmlTemp.html"; //temporal
            string pathHTMLPlantilla = path + "//Content//TemplatePDFCartaPorte//PlantillaCartaPorte.html";
            string sHtml = GetStringOfFile(pathHTMLPlantilla);
            string resultHtml = "";
            resultHtml = RazorEngine.Razor.Parse(sHtml, oComprobante);
            //string pathPDF = @"D:\XML-GENERADOS-CARTAPORTE\SENATOR.pdf";

            var pdf = Pdf.From(resultHtml).OfSize(PaperSize.A4).WithoutOutline().WithMargins(1.25.Centimeters()).Portrait().Comressed().Content();
           
            //System.IO.File.WriteAllBytes(pathPDF, pdf);

            //crea el archivo temporal
            System.IO.File.WriteAllText(pathHTMLTemp, resultHtml);




            //eliminamos el archivo temporal
            System.IO.File.Delete(pathHTMLTemp);

            return pdf;
        }
    }
}
