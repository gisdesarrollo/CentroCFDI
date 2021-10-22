using Aplicacion.Context;
using Aplicacion.LogicaAlterna;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utilerias.LogicaPrincipal;

namespace Aplicacion.LogicaPrincipal.ObtencionArchivos
{
    public class ObtenerArchivosLocales
    {

        #region Variables

        private readonly String _path;
        private readonly AplicacionContext _db = new AplicacionContext();
        private readonly ManejoArchivos _manejoArchivos = new ManejoArchivos();
        private readonly OperacionesStreams _operacionesStreams = new OperacionesStreams();

        #endregion

        #region Constructor

        public ObtenerArchivosLocales(string path)
        {
            _path = path;
        }

        #endregion

        public List<String> ObtenerArchivos()
        {
            var archivos = Directory.GetFiles(_path, "*.*", SearchOption.TopDirectoryOnly).ToList();
            var nombresArchivos = new List<String>();

            foreach (var archivo in archivos)
            {
                if (archivo.Contains(".xml") || archivo.Contains(".XML") || archivo.Contains(".pdf") || archivo.Contains(".PDF"))
                {
                    try
                    {
                        nombresArchivos.Add(archivo);
                    }
                    catch (Exception ex)
                    {
                        _manejoArchivos.MoverErroneo(archivo);
                        throw new Exception(String.Format("Error al Obtener Archivos: {0}", ex.Message));
                    }
                }
            }
            return nombresArchivos;
        }
    }
}
