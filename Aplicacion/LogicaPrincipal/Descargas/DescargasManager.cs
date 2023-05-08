using API.Catalogos;
using API.Operaciones.ComplementoCartaPorte;
using API.Operaciones.ComplementosPagos;
using API.Operaciones.ComprobantesCfdi;
using Aplicacion.Context;
using Aplicacion.LogicaPrincipal.Validacion;
using MessagingToolkit.QRCode.Codec;
using OpenHtmlToPdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.LogicaPrincipal.Descargas
{
    public class DescargasManager
    {
        private readonly AplicacionContext _db = new AplicacionContext();
        private readonly DecodificaFacturas _decodifica = new DecodificaFacturas();

        //Genera los xml byte en archivo temp de cualquier tipo de factura 
        public String GeneraFilePathXml(byte[] byteXml,string serie,string folio)
        {
            try
            {
                var path = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//FileCfdiGenerados//{0} - {1} - {2}.xml", serie, folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                //guardar string en un archivo
                System.IO.File.WriteAllText(path, Encoding.UTF8.GetString(byteXml));

                return path;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] GeneraPDF33(ComprobanteCFDI33 oComprobante, string tipoDocumento, int id, bool isFacturaEmitida)
        {
            var sucursal = new Sucursal();
            string base64QR = null;
            string plantilla = null;
            if (tipoDocumento == "CartaPorte33")
            {
                var cartaPorte = new ComplementoCartaPorte();
                if (isFacturaEmitida)
                {
                    cartaPorte = _db.ComplementoCartaPortes.Where(cp => cp.FacturaEmitidaId == id).FirstOrDefault();
                }
                else
                {
                    cartaPorte = _db.ComplementoCartaPortes.Find(id);
                }
                //get QR
                base64QR = System.Convert.ToBase64String(cartaPorte.FacturaEmitida.CodigoQR);
                //get sucursal
                sucursal = _db.Sucursales.Find(cartaPorte.SucursalId);
                //asigna plantilla
                plantilla = "TemplatePDFCartaPorte//PlantillaCartaPorte33.cshtml";
            }
            else if (tipoDocumento == "Pagos33") {
                var pagos = new ComplementoPago();
                if (isFacturaEmitida) { pagos = _db.ComplementosPago.Where(p => p.FacturaEmitidaId == id).FirstOrDefault(); }
                else { pagos = _db.ComplementosPago.Find(id); };
                //get sucursal
                sucursal = _db.Sucursales.Find(pagos.SucursalId);

                //Genera QR para CFDI Externos 33
                oComprobante.CodigoQR = GeneraQR(oComprobante.Emisor.Rfc, oComprobante.Receptor.Rfc, oComprobante.Total, oComprobante.TimbreFiscalDigital.UUID);
                //asigna plantilla
                plantilla = "TemplatePDFPagos//PlantillaPagos33.cshtml";
            }
            else if (tipoDocumento == "Cfdi33") {

                var comprobante = new ComprobanteCfdi();

                if (isFacturaEmitida)
                {
                    comprobante = _db.ComprobantesCfdi.Where(c => c.FacturaEmitidaId == id).FirstOrDefault();
                    if(comprobante == null)
                    {
                        var cfdi = _db.FacturasEmitidas.Find(id);
                        //get sucursal
                        sucursal = _db.Sucursales.Find(cfdi.EmisorId);
                    }
                    
                }
                else
                {
                    comprobante = _db.ComprobantesCfdi.Find(id);
                    if(comprobante != null)
                    {
                        //get sucursal
                        sucursal = _db.Sucursales.Find(comprobante.SucursalId);

                    }
                }

                
                //Genera QR para CFDI Externos
                oComprobante.CodigoQR = GeneraQR(oComprobante.Emisor.Rfc, oComprobante.Receptor.Rfc, oComprobante.Total, oComprobante.TimbreFiscalDigital.UUID);
                //asigna plantilla
                plantilla = "TemplatePDFCartaPorte//PlantillaCartaPorte33.cshtml";
            }
            else {
                var facturasEmtidas = _db.FacturasEmitidas.Find(id);
                //get sucursal
                sucursal = _db.Sucursales.Find(facturasEmtidas.EmisorId);
                //Genera QR para CFDI Externos
                oComprobante.CodigoQR = GeneraQR(oComprobante.Emisor.Rfc, oComprobante.Receptor.Rfc, oComprobante.Total, oComprobante.TimbreFiscalDigital.UUID);
                //asigna plantilla
                plantilla = "TemplatePDFCartaPorte//PlantillaCartaPorte33.cshtml";
            }

            //add QR
            if (base64QR != null) { 
                string sQR = System.String.Format("data:image/gif;base64,{0}", base64QR);
                oComprobante.CodigoQR = sQR;
            }
           

            //logo
            if (sucursal.Logo != null)
            {
                string base64Logo = System.Convert.ToBase64String(sucursal.Logo);
                string sLogo = System.String.Format("data:image/gif;base64,{0}", base64Logo);
                oComprobante.Logo = sLogo;
            }
            //Razor Html a PDF

            string path = AppDomain.CurrentDomain.BaseDirectory;
            string pathHTMLPlantilla = path + "//Content//"+plantilla;
            string sHtml = GetStringOfFile(pathHTMLPlantilla);
            string resultHtml = "";
            resultHtml = RazorEngine.Razor.Parse(sHtml, oComprobante);

            var pdf = Pdf.From(resultHtml).OfSize(PaperSize.A4).WithoutOutline().WithMargins(1.25.Centimeters()).Portrait().Comressed().Content();

            return pdf;
        }

        public byte[] GeneraPDF40(ComprobanteCFDI oComprobante, string tipoDocumento, int id, bool isFacturaEmitida)
        {
            var sucursal = new Sucursal();
            string base64QR = null;
            string plantilla = null;
            var comprobante = new ComprobanteCfdi();
            var cartaPorte = new ComplementoCartaPorte();
            var pagos = new ComplementoPago();
            if (tipoDocumento == "CartaPorte40")
            {
                if (isFacturaEmitida)
                {
                    cartaPorte = _db.ComplementoCartaPortes.Where(cp => cp.FacturaEmitidaId == id).FirstOrDefault();
                }
                else
                {
                    cartaPorte = _db.ComplementoCartaPortes.Find(id);
                }
                //get QR
                base64QR = System.Convert.ToBase64String(cartaPorte.FacturaEmitida.CodigoQR);
                //get sucursal
                sucursal = _db.Sucursales.Find(cartaPorte.SucursalId);
                //asigna plantilla
                plantilla = "TemplatePDFCartaPorte//PlantillaCartaPorte.cshtml";

                //set referencia 
                
                if (cartaPorte.Sucursal.Rfc == "CME090205NS5") { 
                    foreach (var ubicacion in oComprobante.CartaPorte.Ubicaciones)
                    {
                        if (ubicacion.TipoUbicacion == "Origen")
                        {
                            oComprobante.Referencia = ubicacion.Domicilio.Referencia;
                            break;
                        }
                    }
                }
            }
            else if (tipoDocumento == "Pagos40")
            {
                if (isFacturaEmitida)
                {
                    pagos = _db.ComplementosPago.Where(p => p.FacturaEmitidaId == id).FirstOrDefault();
                }
                else
                {
                    pagos = _db.ComplementosPago.Find(id);

                }
                //get QR
                base64QR = System.Convert.ToBase64String(pagos.FacturaEmitida.CodigoQR);
                //get sucursal
                sucursal = _db.Sucursales.Find(pagos.SucursalId);
                
                //asigna plantilla
                plantilla = "TemplatePDFPagos//PlantillaPagos.cshtml";
            }
            else if (tipoDocumento == "Cfdi40")
            {
                if (isFacturaEmitida) {
                     comprobante = _db.ComprobantesCfdi.Where( c=> c.FacturaEmitidaId == id).FirstOrDefault();
                }
                else {
                    comprobante = _db.ComprobantesCfdi.Find(id);
                }
                
                if(comprobante != null)
                {
                    //get QR
                    base64QR = System.Convert.ToBase64String(comprobante.FacturaEmitida.CodigoQR);
                    //get sucursal
                    sucursal = _db.Sucursales.Find(comprobante.SucursalId);
                    
                }
                else
                {
                    var cfdi = _db.FacturasEmitidas.Find(id);
                    //get sucursal
                    sucursal = _db.Sucursales.Find(cfdi.EmisorId);
                    //Genera QR para CFDI Externos
                    oComprobante.CodigoQR = GeneraQR(oComprobante.Emisor.Rfc,oComprobante.Receptor.Rfc,oComprobante.Total,oComprobante.TimbreFiscalDigital.UUID);
                    
                }
                //asigna plantilla
                plantilla = "TemplatePDFCartaPorte//PlantillaCartaPorte.cshtml";

            }
            else
            {
                var facturasEmtidas = _db.FacturasEmitidas.Find(id);
                //get sucursal
                sucursal = _db.Sucursales.Find(facturasEmtidas.EmisorId);
                //Genera QR para CFDI Externos
                oComprobante.CodigoQR = GeneraQR(oComprobante.Emisor.Rfc, oComprobante.Receptor.Rfc, oComprobante.Total, oComprobante.TimbreFiscalDigital.UUID);
                //asigna plantilla
                plantilla = "TemplatePDFCartaPorte//PlantillaCartaPorte.cshtml";
            }

            //add QR
            if (base64QR != null)
            {
                string sQR = System.String.Format("data:image/gif;base64,{0}", base64QR);
                oComprobante.CodigoQR = sQR;
            }


            //logo
            if (sucursal.Logo != null)
            {
                string base64Logo = System.Convert.ToBase64String(sucursal.Logo);
                string sLogo = System.String.Format("data:image/gif;base64,{0}", base64Logo);
                oComprobante.Logo = sLogo;
            }
            //Razor Html a PDF

            string path = AppDomain.CurrentDomain.BaseDirectory;
            string pathHTMLPlantilla = path + "//Content//" + plantilla;
            string sHtml = GetStringOfFile(pathHTMLPlantilla);
            string resultHtml = "";
            resultHtml = RazorEngine.Razor.Parse(sHtml, oComprobante);

            var pdf = Pdf.From(resultHtml).OfSize(PaperSize.A4).WithoutOutline().WithMargins(1.25.Centimeters()).Portrait().Comressed().Content();
            
            return pdf;
        }

        public string DowloadAcuseCancelacion(int sucursalId , byte[] byteXml)
        {
            var sucursal = _db.Sucursales.Find(sucursalId);
            string xmlCancelacion = null;

            //Obtenemos el contenido del XML seleccionado.
            string CadenaXML = System.Text.Encoding.UTF8.GetString(byteXml);
            string RFCEmisor = _decodifica.LeerValorXML(CadenaXML, "Rfc", "Emisor");
            string UUID = _decodifica.LeerValorXML(CadenaXML, "UUID", "TimbreFiscalDigital");
            if (UUID == "")
            {
                throw new Exception(String.Format("El CFDI seleccionado no está timbrado."));

            }

            RVCFDI33.WSConsultasCFDReal.Service1 objConsulta = new RVCFDI33.WSConsultasCFDReal.Service1();
            RVCFDI33.WSConsultasCFDReal.acuse_cancel_struct objCancel = new RVCFDI33.WSConsultasCFDReal.acuse_cancel_struct();
            objCancel = objConsulta.Consultar_Acuse_cancelado_UUID(UUID, "fgomez", "12121212", RFCEmisor);
            //consulta a produccion
            //objCancel = objConsulta.Consultar_Acuse_cancelado_UUID(UUID, sucursal.Rfc, sucursal.Rfc, RFCEmisor);
            if (objCancel._ERROR == "")
            {
                if (objCancel.xml != null && objCancel.xml != "")
                {
                    xmlCancelacion = objCancel.xml;
                }
            }

            return xmlCancelacion;
        }
        private static string GetStringOfFile(string pathFile)
        {
            string contenido = File.ReadAllText(pathFile);
            return contenido;
        }

        private static string GeneraQR(string rfcSucursal ,string rfcReceptor,decimal total,string uuid)
        {
            //Genera QR para CFDI Externos
            //string URL
            string str1 = string.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//Temp//QR-{0}.jpg", (object)DateTime.Now.ToString("yyyyMMddHHmmssffff"));
            //string data
            string str2 = string.Format("?re={0}&rr={1}&tt={2:0000000000.000000}&id={3}", (object)rfcSucursal, (object)rfcReceptor,
                (object)total, (object)uuid);
            //genera QR Temp
            new QRCodeEncoder().Encode(str2).Save(str1);

            //get byte imagen
            byte[] imgdata = System.IO.File.ReadAllBytes(str1);
            var base64QRTemp = System.Convert.ToBase64String(imgdata);
            string sQRTemp = System.String.Format("data:image/gif;base64,{0}", base64QRTemp);
           // oComprobante.CodigoQR = sQRTemp;
            //delete temp QR
            System.IO.File.Delete(str1);

            return sQRTemp;
        }
    }
}
