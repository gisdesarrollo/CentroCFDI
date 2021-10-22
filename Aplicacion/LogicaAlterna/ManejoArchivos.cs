using System;
using System.IO;

namespace Aplicacion.LogicaAlterna
{
    public class ManejoArchivos
    {
        public void MoverErroneo(String archivo)
        {
            var pathNuevo = Path.Combine(Path.GetDirectoryName(archivo), "Erroneos", Path.GetFileName(archivo));

            if (!Directory.Exists(Path.GetDirectoryName(pathNuevo)))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(pathNuevo));
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("No se pudo generar el directorio de erroneos en {0}: {1}", Path.GetDirectoryName(pathNuevo), ex.Message));
                }
            }

            if (File.Exists(pathNuevo))
            {
                try
                {
                    File.Delete(pathNuevo);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("No se pudo borrar el archivo {0} para mover a la carpeta de los erroneos: {1}", archivo, ex.Message));
                }

            }

            try
            {
                using (Stream streamArchivo = new FileStream(archivo, FileMode.Open))
                {
                    using (var fileStream = File.Create(pathNuevo))
                    {
                        streamArchivo.Seek(0, SeekOrigin.Begin);
                        streamArchivo.CopyTo(fileStream);
                        streamArchivo.Close();
                        fileStream.Close();
                    }
                    File.Delete(archivo);
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(String.Format("No se pudo mover el archivo {0} a la carpeta de los erroneos: {1}", archivoPath, ex.Message));
            }

            var archivoPdf = archivo.Replace(".xml", ".pdf");
            if (File.Exists(archivoPdf))
            {
                MoverErroneo(archivoPdf);
            }
        }
    }
}
