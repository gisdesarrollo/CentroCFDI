using Aplicacion.Context;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Data.Entity;
using System.IO;
using System.Net;

namespace Aplicacion.LogicaPrincipal.ObtencionArchivos
{
    public class SolicitudArchivosXsa
    {

        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion

        public void Solicitar(int sucursalId, DateTime fecha)
        {
            var sucursal = _db.Sucursales.Find(sucursalId);
            var url = String.Format("https://{0}/xsamanager/DownloadExpedidosBloqueServlet?rfcEmisor={1}&key={2}&fechaInicial={3}&fechaFinal={4}&tipo={5}", sucursal.Servidor, sucursal.Rfc, sucursal.KeyXsa, fecha.ToString("yyyy-MM-dd"), fecha.AddDays(1).ToString("yyyy-MM-dd"), "XML");

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            using (WebClient client = new WebClient())
            {
                var path = Path.Combine(@"C:\inetpub\sftproot\ApBox\");
                var nombreArchivo = String.Format("{0}_{1:yyyy-MM-dd}.zip", sucursal.Rfc, fecha);
                var pathFinal = Path.Combine(path, nombreArchivo);

                try
                {
                    client.DownloadFile(url, pathFinal);
                    ExtraerZip(pathFinal);

                    var pathTexto = Path.Combine(path, "leeme.txt");
                    File.Delete(pathFinal);
                    if (File.Exists(pathTexto))
                    {
                        File.Delete(pathTexto);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("No se pudo descargar el archivo del web service {0}: {1}", url, ex.Message));
                }
            }

            sucursal.FechaInicial = fecha;
            _db.Entry(sucursal).State = EntityState.Modified;
            _db.SaveChanges();
        }

        private void ExtraerZip(String pathCompleto)
        {
            FastZip fastZip = new FastZip();
            string fileFilter = null;

            fastZip.ExtractZip(pathCompleto, Path.GetDirectoryName(pathCompleto), fileFilter);
        }
    }
}