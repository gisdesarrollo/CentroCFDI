using iText.Kernel.Pdf.Canvas.Parser.Listener;
using iText.Kernel.Pdf.Canvas.Parser;
using System;
using System.Text;
using iText.Kernel.Pdf;
using API.Models.Expedientes;
using System.Web;
using System.Text.RegularExpressions;
using static iText.Svg.SvgConstants;
using System.Security.Cryptography;

namespace Aplicacion.LogicaPrincipal.ValidaExpediente
{
    public class CSFExtrator
    {
        public InformacionFiscalCsf GetCadenaOriginalCsf(HttpPostedFileBase src)
        {
            bool isAnotherLine = false;
            // Reiniciamos el flujo de entrada antes de procesar el PDF
            src.InputStream.Position = 0;
            using (PdfReader reader = new PdfReader(src.InputStream))
            using (PdfDocument pdfDocument = new PdfDocument(reader))
            {
                var strategy = new LocationTextExtractionStrategy();
                string text = string.Empty;
                for (int i = 1; i <= pdfDocument.GetNumberOfPages(); ++i)
                {
                    var page = pdfDocument.GetPage(i);
                    text += PdfTextExtractor.GetTextFromPage(page);
                }
                string[] lineas = Regex.Split(text, @"\r\n|\r|\n");
                InformacionFiscalCsf infoFiscal = null;
                for (int i = 0; i < lineas.Length; i++)
                {
                    string linea = lineas[i];
                    if (linea.ToLower().Contains("cadena original sello"))
                    {
                        // se recorre una linea atras si contiene los datos
                        if (i > 0)
                        {
                            string[] lineaAnterior = lineas[i - 1].Split('|', (char)StringSplitOptions.RemoveEmptyEntries);
                            if (lineaAnterior.Length == 8) 
                            {
                                infoFiscal = new InformacionFiscalCsf
                                {
                                    Fecha = DateTime.Parse(lineaAnterior[2]),
                                    RFC = lineaAnterior[3],                     
                                    TipoDocumento = lineaAnterior[4],           
                                    NumeroDocumento = lineaAnterior[5]          
                                };
                                if (infoFiscal.RFC != null)
                                {
                                    isAnotherLine = true;
                                }

                            }
                        }
                        // se recorre una linea adelante si contiene los datos
                        if (i + 1 < lineas.Length && !isAnotherLine)
                        {
                            string[] siguienteLinea = lineas[i + 1].Split('|', (char)StringSplitOptions.RemoveEmptyEntries);
                            if (siguienteLinea.Length == 8)
                            {
                                infoFiscal.Fecha = DateTime.Parse(siguienteLinea[2]);   
                                infoFiscal.RFC = siguienteLinea[3];                     
                                infoFiscal.TipoDocumento = siguienteLinea[4];           
                                infoFiscal.NumeroDocumento = siguienteLinea[5];         
                                if (infoFiscal.RFC != null)
                                {
                                    isAnotherLine = true;
                                }
                            }
                        }
                        if (!isAnotherLine)
                        {
                            // sobre la misma linea atras si contiene los datos
                            string[] partes = lineas[i].Split(':', (char)StringSplitOptions.RemoveEmptyEntries);
                            if (partes.Length > 1)
                            {

                                string[] valores = partes[1].Split('|', (char)StringSplitOptions.RemoveEmptyEntries);
                                if (valores.Length == 8)
                                {
                                    infoFiscal.Fecha = DateTime.Parse(valores[2]);
                                    infoFiscal.RFC = valores[3];                     
                                    infoFiscal.TipoDocumento = valores[4];           
                                    infoFiscal.NumeroDocumento = valores[5];         
                                }
                                else
                                {
                                    throw new Exception("Ocurrio un error al momento de obtener los datos de la cadena original sello.");
                                }

                            }
                        }
                    }
                    

                }
                return infoFiscal;
            }

        }
        public InformacionFiscalOcof GetCadenaOriginalOcof(HttpPostedFileBase src)
        {
            bool isAnotherLine = false;
            // Reiniciamos el flujo de entrada antes de procesar el PDF
            src.InputStream.Position = 0;
            using (PdfReader reader = new PdfReader(src.InputStream))
            using (PdfDocument pdfDocument = new PdfDocument(reader))
            {
                var strategy = new LocationTextExtractionStrategy();
                string text = string.Empty;
                for (int i = 1; i <= pdfDocument.GetNumberOfPages(); ++i)
                {
                    var page = pdfDocument.GetPage(i);
                    text += PdfTextExtractor.GetTextFromPage(page);
                }
                string[] lineas = Regex.Split(text, @"\r\n|\r|\n");
                InformacionFiscalOcof infoFiscal = null;
                for (int i = 0; i < lineas.Length; i++)
                {
                    string linea = lineas[i];
                    if (linea.ToLower().Contains("cadena original"))
                    {
                        // se recorre una linea atras si contiene los datos
                        if (i > 0)
                        {
                            string[] lineaAnterior = lineas[i - 1].Split('|', (char)StringSplitOptions.RemoveEmptyEntries);
                            if (lineaAnterior.Length == 10) 
                            {
                                infoFiscal = new InformacionFiscalOcof
                                {
                                    RFC = lineaAnterior[2],   
                                    Folio = lineaAnterior[3],
                                    FechaEmision = DateTime.Parse(lineaAnterior[4]),
                                    EstatusCumplimiento = lineaAnterior[5],
                                    NumeroDocumento = lineaAnterior[7]
                                };
                                if (infoFiscal.RFC != null)
                                {
                                    isAnotherLine = true;
                                }

                            }
                        }
                        // se recorre una linea adelante si contiene los datos
                        if (i + 1 < lineas.Length && !isAnotherLine)
                        {
                            string[] siguienteLinea = lineas[i + 1].Split('|', (char)StringSplitOptions.RemoveEmptyEntries);
                            if (siguienteLinea.Length == 10)
                            {
                                infoFiscal = new InformacionFiscalOcof
                                {
                                    RFC = siguienteLinea[2],
                                    Folio = siguienteLinea[3],
                                    FechaEmision = DateTime.Parse(siguienteLinea[4]),
                                    EstatusCumplimiento = siguienteLinea[5],
                                    NumeroDocumento = siguienteLinea[7]
                                };
                                if (infoFiscal.RFC != null)
                                {
                                    isAnotherLine = true;
                                }
                            }
                        }
                        
                        if (!isAnotherLine)
                        {
                            // sobre la misma linea si obtener los datos
                            string[] partes = lineas[i].Split(':', (char)StringSplitOptions.RemoveEmptyEntries);
                            if (partes.Length > 1)
                            {

                                string[] valores = partes[1].Split('|', (char)StringSplitOptions.RemoveEmptyEntries);
                                if (valores.Length == 10)
                                {
                                    infoFiscal = new InformacionFiscalOcof
                                    {
                                        RFC = valores[2],
                                        Folio = valores[3],
                                        FechaEmision = DateTime.Parse(valores[4]),
                                        EstatusCumplimiento = valores[5],
                                        NumeroDocumento = valores[7]
                                    };
                                }
                                else
                                {
                                    throw new Exception("Ocurrio un error al momento de obtener los datos de la cadena original");
                                }

                            }
                        }
                    }


                }
                return infoFiscal;
            }

        }

        public bool EsCsf(HttpPostedFileBase rutaArchivoPdf)
        {
           if (rutaArchivoPdf == null || rutaArchivoPdf.ContentLength == 0 || !rutaArchivoPdf.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return false; 
            }
            using (PdfReader reader = new PdfReader(rutaArchivoPdf.InputStream))
            using (PdfDocument pdfDoc = new PdfDocument(reader))
            {
                StringBuilder textoExtraido = new StringBuilder();

                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    string paginaTexto = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i));
                    textoExtraido.Append(paginaTexto);
                }

                if (textoExtraido.ToString().ToLower().Contains("constancia de situación fiscal"))
                {
                    return true; 
                }
            }

            return false;
        }

        public bool EsOcof(HttpPostedFileBase rutaArchivoPdf)
        {
            if (rutaArchivoPdf == null || rutaArchivoPdf.ContentLength == 0 || !rutaArchivoPdf.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return false; 
            }
            using (PdfReader reader = new PdfReader(rutaArchivoPdf.InputStream))
            using (PdfDocument pdfDoc = new PdfDocument(reader))
            {
                StringBuilder textoExtraido = new StringBuilder();

                for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
                {
                    string paginaTexto = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i));
                    textoExtraido.Append(paginaTexto);
                }
                
                if (textoExtraido.ToString().ToLower().Contains("opinión del cumplimiento de obligaciones fiscales"))
                {
                    return true; 
                }
            }

            return false; 
        }

        public bool ValidEmision(DateTime fechaVencimiento,DateTime fechaDocumento)
        {
            DateTime fechaMinima = fechaVencimiento.AddDays(-8);
            DateTime fechaHoy = DateTime.Now;
            DateTime fechaMinimaAlactual = fechaHoy.AddDays(-8);
            bool valid = false;
            if (fechaDocumento <= fechaHoy)
            {
                valid = true;
            }

            if (fechaDocumento < fechaMinima)
            {
                throw new Exception("Error: La fecha del documento está fuera del rango de vencimiento.");
            }

            if (fechaDocumento >= fechaMinima)
            {
                valid = true; 
            }

            if (fechaDocumento < fechaMinimaAlactual)
            {
                throw new Exception("Error: La fecha del documento está fuera de rango.");
            }

            return valid; 

        }

    }
}
