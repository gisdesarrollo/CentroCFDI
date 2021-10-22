using API.Enums;
using Aplicacion.Context;
using Aplicacion.LogicaAlterna;
using Aplicacion.LogicaPrincipal.ComplementosPagos;
using Aplicacion.LogicaPrincipal.ObtencionArchivos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utilerias.LogicaPrincipal;

namespace BusApBox
{
    public class Program
    {
        public void ProcesarArchivoExterno()
        {
            //Console.WriteLine("***************Inicio Cola***************");
            LogicaFacadeFacturas _logicaFacade;
            AplicacionContext _db = new AplicacionContext();
            ManejoArchivos _manejoArchivos = new ManejoArchivos();
            OperacionesStreams _operacionesStreams = new OperacionesStreams();
            SolicitudArchivosXsa _solicitudArchivosXsa = new SolicitudArchivosXsa();
            ObtenerConfiguraciones _obtenerConfiguraciones = new ObtenerConfiguraciones();
            //var pathArchivosSftp = Properties.Settings.Default.PathSftp;
            var pathArchivosExternos = Properties.Settings.Default.PathArchivos;
            var archivos = new List<String>();
            var obtenerArchivos = new ObtenerArchivosLocales(pathArchivosExternos);
                archivos = obtenerArchivos.ObtenerArchivos();

                foreach (var archivo in archivos.Where(a => a.Contains(".xml") || a.Contains(".XML")))
                {
                    Console.WriteLine("**************Procesando archivos**************");
                    Console.WriteLine("{0}", archivo);
                        
                    try
                    {
                        _logicaFacade = new LogicaFacadeFacturas();

                        Console.WriteLine("Inicio: {0}", DateTime.Now);
                        _logicaFacade.Decodificar(archivo);
                        Console.WriteLine("Fin: {0}", DateTime.Now);
                    }
                    catch (Exception ex)
                    {
                    Console.WriteLine("Error: {0}", ex.Message);
                        _manejoArchivos.MoverErroneo(archivo);
                    }
                    Console.WriteLine("**************Finalizando Proceso de Archivos**************");
                    Console.WriteLine("");
                }


        }
        public static void Main(string[] args)
        {
          //  Console.WriteLine("***************Inicio Cola***************");

           /* var pathArchivosSftp = Properties.Settings.Default.PathSftp;
            var pathArchivosExternos = Properties.Settings.Default.PathArchivos;

            while (true)
            {
                LogicaFacadeFacturas _logicaFacade;
                AplicacionContext _db = new AplicacionContext();
                ManejoArchivos _manejoArchivos = new ManejoArchivos();
                OperacionesStreams _operacionesStreams = new OperacionesStreams();
                SolicitudArchivosXsa _solicitudArchivosXsa = new SolicitudArchivosXsa();
                ObtenerConfiguraciones _obtenerConfiguraciones = new ObtenerConfiguraciones();

                var sucursales = _db.Sucursales.Where(s => s.Status == Status.Activo).ToList();
                
                var archivos = new List<String>();
                var pdfs = new List<String>();

                //Copiar Archivos Cargados
                try
                {
                    string[] archivosExternos = Directory.GetFiles(pathArchivosExternos);
                    foreach (string archivoExterno in archivosExternos)
                    {
                        string nombreArchivo = Path.GetFileName(archivoExterno);
                        string dest = Path.Combine(pathArchivosSftp, nombreArchivo);

                        try
                        {
                            File.Move(archivoExterno, dest);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Ocurrio un error al mover los archivos de {0} a {1}", pathArchivosExternos, pathArchivosSftp);
                }*/

             /*   var obtenerArchivos = new ObtenerArchivosLocales(pathArchivosSftp);
                archivos = obtenerArchivos.ObtenerArchivos();

                foreach (var archivo in archivos.Where(a => a.Contains(".xml") || a.Contains(".XML")))
                {
                    Console.WriteLine("**************Procesando archivos**************");
                    Console.WriteLine("{0}", archivo);
                        
                    try
                    {
                        _logicaFacade = new LogicaFacadeFacturas();

                        Console.WriteLine("Inicio: {0}", DateTime.Now);
                        _logicaFacade.Decodificar(archivo);
                        Console.WriteLine("Fin: {0}", DateTime.Now);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: {0}", ex.Message);
                        _manejoArchivos.MoverErroneo(archivo);
                    }
                    Console.WriteLine("**************Finalizando Proceso de Archivos**************");
                    Console.WriteLine("");
                }*/

              /*  foreach (var sucursal in sucursales)
                {
                    //Solicitud de Archivos de XSA
                    if (sucursal.Servidor != null)
                    {
                        if(sucursal.FechaInicial.Value.Date < DateTime.Now.Date)
                        {
                            try
                            {
                                Console.WriteLine("**************Solicitando archivos XSA de {0}**************", sucursal.Nombre);
                                var fechaBase = sucursal.FechaInicial.Value;
                                var totalDias = (DateTime.Now - fechaBase).TotalDays;
                                for (int i = 0; i < totalDias; i++)
                                {
                                    try
                                    {
                                        _solicitudArchivosXsa.Solicitar(sucursal.Id, fechaBase.AddDays(i));
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Error al descargar los archivos del día {0} para la sucursal {1}: {2}", fechaBase.AddDays(i), sucursal.Nombre, ex.Message);
                                    }

                                }
                                Console.WriteLine("**************Finalizando solicitud de archivos XSA de {0}**************", sucursal.Nombre);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error de obtención de archivos para la sucursal {0}: {1}", sucursal.Nombre, ex.Message);
                            }
                            
                        }
                    }
                }
            }*/
        }
    }
}
