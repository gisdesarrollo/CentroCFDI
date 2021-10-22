using System;
using System.IO;

namespace BusApBox.Aplicacion
{
    public static class LogErrores
    {
        public static void Insertar(String error)
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ErroresApBox.log");

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Mensaje :" + error + " Fecha :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }
        }
    }
}