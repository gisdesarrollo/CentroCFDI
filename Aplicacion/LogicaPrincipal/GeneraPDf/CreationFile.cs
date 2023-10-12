using API.Operaciones.ComplementoCartaPorte;
using API.Operaciones.ComprobantesCfdi;
using Aplicacion.Context;
using Aplicacion.LogicaPrincipal.Validacion;
using MessagingToolkit.QRCode.Codec;
using OpenHtmlToPdf;
using System;
using System.IO;
using System.Net;
using System.Text;
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
              
                    foreach (var oComplementoInterior in oComprobante.Complemento.Any)
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
            return oComprobante;
        }

        public ComprobanteCFDI DeserealizarComprobanteXML(int comprobanteId)
        {
            var comprobanteCfdi = _db.ComprobantesCfdi.Find(comprobanteId);

            ComprobanteCFDI oComprobante;
            XmlSerializer oSerializer = new XmlSerializer(typeof(ComprobanteCFDI));

            using (StreamReader reader = new StreamReader(new MemoryStream(comprobanteCfdi.FacturaEmitida.ArchivoFisicoXml), Encoding.UTF8))
            {
                //aqui deserializamos
                oComprobante = (ComprobanteCFDI)oSerializer.Deserialize(reader);


                //complementos Timbre Fiscal

                foreach (var oComplementoInterior in oComprobante.Complemento.Any)
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
                }

            }
            return oComprobante;
        }

        public ComprobanteCFDI DeserealizarXmlPagos20(int complementoPago)
        {
            var complementoPPago = _db.ComplementosPago.Find(complementoPago);

            ComprobanteCFDI oComprobante;
            
                XmlSerializer oSerializer = new XmlSerializer(typeof(ComprobanteCFDI));

                using (StreamReader reader = new StreamReader(new MemoryStream(complementoPPago.FacturaEmitida.ArchivoFisicoXml), Encoding.UTF8))
                {
                    //aqui deserializamos
                    oComprobante = (ComprobanteCFDI)oSerializer.Deserialize(reader);


                    //complementos

                    foreach (var oComplementoInterior in oComprobante.Complemento.Any)
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
                        if (oComplementoInterior.Name.Contains("Pagos"))
                        {
                            XmlSerializer oSerializerComplemento = new XmlSerializer(typeof(Pagos));
                            using (var readerComplemento = new StringReader(oComplementoInterior.OuterXml))
                            {
                                oComprobante.Pagos =
                                    (Pagos)oSerializerComplemento.Deserialize(readerComplemento);
                            }

                        }

                    }

                }
            return oComprobante;
        }

        public ComprobanteCFDI33 DeserealizarXmlPagos10(int complementoPago)
        {
            var complementoPPago = _db.ComplementosPago.Find(complementoPago);

            ComprobanteCFDI33 oComprobante;

            XmlSerializer oSerializer = new XmlSerializer(typeof(ComprobanteCFDI33));

            using (StreamReader reader = new StreamReader(new MemoryStream(complementoPPago.FacturaEmitida.ArchivoFisicoXml), Encoding.UTF8))
            {
                //aqui deserializamos
                oComprobante = (ComprobanteCFDI33)oSerializer.Deserialize(reader);


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
                        if (oComplementoInterior.Name.Contains("Pagos"))
                        {
                            XmlSerializer oSerializerComplemento = new XmlSerializer(typeof(Pagos10));
                            using (var readerComplemento = new StringReader(oComplementoInterior.OuterXml))
                            {
                                oComprobante.Pagos =
                                    (Pagos10)oSerializerComplemento.Deserialize(readerComplemento);
                            }
                        }

                    }
                }               
            }

            return oComprobante;
        }

        public ComprobanteCFDI33 DeserealizarXml33CartaPorte(int complementoCartaPorteId)
        {
            var complementoCartaPorte = _db.ComplementoCartaPortes.Find(complementoCartaPorteId);
            //byte[] xmlLocal = System.IO.File.ReadAllBytes(@"D:\Descargas(C)\CP - 1413 - 20220829124852255.xml");

            ComprobanteCFDI33 oComprobante;
            XmlSerializer oSerializer = new XmlSerializer(typeof(ComprobanteCFDI33));

            //using (StreamReader reader = new StreamReader(new MemoryStream(xmlLocal), Encoding.UTF8))
            using (StreamReader reader = new StreamReader(new MemoryStream(complementoCartaPorte.FacturaEmitida.ArchivoFisicoXml), Encoding.UTF8))
            {
                //aqui deserializamos
                oComprobante = (ComprobanteCFDI33)oSerializer.Deserialize(reader);


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
        public byte[] GeneraPDFComprobante(ComprobanteCFDI oComprobante,int comprobanteId)
        {
            var comprobanteCfdi = _db.ComprobantesCfdi.Find(comprobanteId);
            var sucursal = _db.Sucursales.Find(comprobanteCfdi.SucursalId);
            //add QR
            string base64QR = System.Convert.ToBase64String(comprobanteCfdi.FacturaEmitida.CodigoQR);
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
            string pathHTMLPlantilla = path + "//Content//TemplatePDFCartaPorte//PlantillaCartaPorte.cshtml";
            string sHtml = GetStringOfFile(pathHTMLPlantilla);
            string resultHtml = "";
            resultHtml = RazorEngine.Razor.Parse(sHtml, oComprobante);

            var pdf = Pdf.From(resultHtml).OfSize(PaperSize.A4).WithoutOutline().WithMargins(1.25.Centimeters()).Portrait().Comressed().Content();

            return pdf;

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
            string pathHTMLPlantilla = path + "//Content//TemplatePDFCartaPorte//PlantillaCartaPorte.cshtml";
            string sHtml = GetStringOfFile(pathHTMLPlantilla);
            string resultHtml = "";
            resultHtml = RazorEngine.Razor.Parse(sHtml, oComprobante);
            
            var pdf = Pdf.From(resultHtml).OfSize(PaperSize.A4).WithoutOutline().WithMargins(1.25.Centimeters()).Portrait().Comressed().Content();
           
            return pdf;
        }
        public byte[] GeneraPDF33CartaPorte(ComprobanteCFDI33 oComprobante, int complementoCartaPorteId)
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
            string pathHTMLPlantilla = path + "//Content//TemplatePDFCartaPorte//PlantillaCartaPorte33.cshtml";
            string sHtml = GetStringOfFile(pathHTMLPlantilla);
            string resultHtml = "";
            resultHtml = RazorEngine.Razor.Parse(sHtml, oComprobante);

            var pdf = Pdf.From(resultHtml).OfSize(PaperSize.A4).WithoutOutline().WithMargins(1.25.Centimeters()).Portrait().Comressed().Content();

            return pdf;
        }
        public byte[] GeneraPDFPagos(ComprobanteCFDI oComprobante, int complementoPago)
        {
            var complementoPPago = _db.ComplementosPago.Find(complementoPago);
            var sucursal = _db.Sucursales.Find(complementoPPago.SucursalId);
            //add QR
            string base64QR = System.Convert.ToBase64String(complementoPPago.FacturaEmitida.CodigoQR);
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

            string pathHTMLPlantilla = path + "//Content//TemplatePDFPagos//PlantillaPagos.cshtml";
            string sHtml = GetStringOfFile(pathHTMLPlantilla);
            string resultHtml = "";
            resultHtml = RazorEngine.Razor.Parse(sHtml, oComprobante);
            var pdf = Pdf.From(resultHtml).OfSize(PaperSize.A4).WithoutOutline().WithMargins(1.25.Centimeters()).Portrait().Comressed().Content();
            var size = pdf.Length;

            return pdf;
        }

        public byte[] GeneraPDFPagos33(ComprobanteCFDI33 oComprobante, int complementoPago)
        {
            var complementoPPago = _db.ComplementosPago.Find(complementoPago);
            var sucursal = _db.Sucursales.Find(complementoPPago.SucursalId);
            //Genera QR version 33
            
            //string URL
            string str1 = string.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//Temp//QR-{0}.jpg", (object)DateTime.Now.ToString("yyyyMMddHHmmssffff"));
            //string data
            string str2 = string.Format("?re={0}&rr={1}&tt={2:0000000000.000000}&id={3}", (object)oComprobante.Emisor.Rfc, (object)oComprobante.Receptor.Rfc, 
                (object)oComprobante.Total, (object)oComprobante.TimbreFiscalDigital.UUID);
            //genera QR Temp
            new QRCodeEncoder().Encode(str2).Save(str1);
           
            //get byte imagen
            byte[] imgdata = System.IO.File.ReadAllBytes(str1);

            
            string base64QR = System.Convert.ToBase64String(imgdata);
            string sQR = System.String.Format("data:image/gif;base64,{0}", base64QR);
            oComprobante.CodigoQR = sQR;
            //delete temp QR
            System.IO.File.Delete(str1);
            //logo
            if (sucursal.Logo != null)
            {
                string base64Logo = System.Convert.ToBase64String(sucursal.Logo);
                string sLogo = System.String.Format("data:image/gif;base64,{0}", base64Logo);
                oComprobante.Logo = sLogo;
            }
            //Razor Html a PDF

            string path = AppDomain.CurrentDomain.BaseDirectory;

            string pathHTMLPlantilla = path + "//Content//TemplatePDFPagos//PlantillaPagos33.cshtml";
            string sHtml = GetStringOfFile(pathHTMLPlantilla);
            string resultHtml = "";
            resultHtml = RazorEngine.Razor.Parse(sHtml, oComprobante);
            var pdf = Pdf.From(resultHtml).OfSize(PaperSize.A4).WithoutOutline().WithMargins(1.25.Centimeters()).Portrait().Comressed().Content();
            

            return pdf;
        }

        
        public static byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        public string GetPathPDf(byte[] byteXml,string serie, string folio)
        {
            var path = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//FileCfdiGenerados//{0} - {1} - {2}.pdf", serie, folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            //guardar string en un archivo
            System.IO.File.WriteAllBytes(path, byteXml);

            return path;
        }
    }
}
