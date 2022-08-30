using API.Operaciones.ComplementoCartaPorte;
using Aplicacion.Context;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.LogicaPrincipal.CargasMasivas.CSV
{
   public class CargarConceptos
    {
        #region Variables
        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion
        public List<Conceptos> Importar(string path,int sucursalId)
        {
            var errores = new List<String>();
           
            List<Conceptos> conceptos = new List<Conceptos>();
            
           
            using (StreamReader archivo = File.OpenText(path))
            {
                try
                {
                    var csv = new CsvReader(archivo);

                    var registros = new List<List<String>>();
                   

                    while (csv.Read())
                    {
                        var registro = new List<String>();
                        for (int i = 0; csv.TryGetField(i, out string value); i++)
                        {
                            registro.Add(value);
                        }
                        registros.Add(registro);
                    }

                  
                    for (int i = 1; i < registros.Count(); i++)
                    {
                        Conceptos concepto = new Conceptos();
                        concepto.Traslado = new TrasladoCP();
                        concepto.Retencion = new RetencionCP();
                        try
                        {
                            var NoIdentificacion = registros[i][0];
                            if (String.IsNullOrEmpty(NoIdentificacion))
                            {
                                throw new Exception("No identifcación no puede ir vacio o null");
                            }
                            
                            var cantidad = registros[i][1];
                            if (String.IsNullOrEmpty(cantidad))
                            {
                                throw new Exception("Cantidad no puede ir vacio o null");
                            }
                            var VUnitario = registros[i][2];
                            if (String.IsNullOrEmpty(VUnitario))
                            {
                                throw new Exception("Valor Unitario no puede ir vacio o null");
                            }
                            var descuento = registros[i][3];

                            if (String.IsNullOrEmpty(descuento))
                            {
                                descuento = null;
                            }

                            ///
                            Cat_Conceptos catConceptos = (Cat_Conceptos)_db.Cat_Conceptos.Where(c => c.NoIdentificacion == NoIdentificacion && c.SucursalId == sucursalId).FirstOrDefault();
                            if (catConceptos == null)
                            {
                                throw new Exception("No identificación no existe en catalogos conceptos");
                            }

                            concepto.ClavesProdServ = catConceptos.ClavesProdServ;
                            concepto.ClavesUnidad = catConceptos.ClavesUnidad;
                            concepto.Unidad = catConceptos.Unidad;
                            concepto.Descripcion = catConceptos.Descripcion;
                            concepto.NoIdentificacion = NoIdentificacion;
                            concepto.Cantidad = cantidad;
                            concepto.ValorUnitario = VUnitario;
                            //calcula importe
                            double importe = Convert.ToDouble(cantidad) * Convert.ToDouble(VUnitario);
                            concepto.Importe = importe;
                            concepto.Descuento = descuento;
                            concepto.ObjetoImpuestoId = catConceptos.ObjetoImpuesto;
                            
                            if(catConceptos.ImpuestoIdTras != null)
                            {
                                //concepto.Traslado = new TrasladoCP();
                                concepto.Traslado.TipoImpuesto = "Traslado";
                                //calcula base
                                decimal basePT = ((decimal)(catConceptos.ImpuestoT.Base * importe / 100));
                                concepto.Traslado.Base = basePT;
                                concepto.Traslado.Impuesto = convertImpuesto((int)catConceptos.ImpuestoT.Impuesto);
                                concepto.Traslado.TipoFactor = catConceptos.ImpuestoT.TipoFactor;
                                concepto.Traslado.TasaOCuota = catConceptos.ImpuestoT.TasaOCuota;
                                //calcula importe traslado
                                concepto.Traslado.Importe = (basePT * catConceptos.ImpuestoT.TasaOCuota);

                            }
                            if(catConceptos.ImpuestoIdRet != null)
                            {
                                //concepto.Retencion = new RetencionCP();
                                concepto.Retencion.TipoImpuesto = "Traslado";
                                //calcula base
                                decimal basePR = ((decimal)(catConceptos.ImpuestoR.Base * importe / 100));
                                concepto.Retencion.Base = basePR;
                                concepto.Retencion.Impuesto = convertImpuesto((int)catConceptos.ImpuestoR.Impuesto);
                                concepto.Retencion.TipoFactor = catConceptos.ImpuestoR.TipoFactor;
                                concepto.Retencion.TasaOCuota = catConceptos.ImpuestoR.TasaOCuota;
                                //calcula importe retencion
                                concepto.Retencion.Importe = (basePR * catConceptos.ImpuestoR.TasaOCuota);
                            }
                            conceptos.Add(concepto);

                        }
                        catch (Exception ex)
                        {
                            errores.Add(String.Format("No se pudo procesar el registro {0} el motivo reportado: {1} </br>", i, ex.Message));
                            continue;
                        }
                    }

                    
                }
                catch (Exception ex)
                {
                    errores.Add(ex.Message);
                }
                finally
                {
                    archivo.Close();
                    archivo.Dispose();
                }
            }

            if (errores.Count > 0)
            {
                throw new Exception(String.Join("|", errores));
            }

            

            return conceptos;
        }

        public String Exportar()
        {
            var path = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//Temp//LayoutConceptos_{0}.csv", DateTime.Now.ToString("ddMMyyyyHHmmss"));

            #region Encabezados

            var encabezados = new List<String>
            {
                "*No Identificación",
                "*Cantidad",
                "*Valor Unitario",
                "Descuento"
            };

            #endregion

            #region Registros

            #endregion

            using (var textWriter = File.CreateText(path))
            {
                using (var csv = new CsvWriter(textWriter))
                {
                    csv.WriteField(encabezados);
                }
                textWriter.Close();
            }

            return path;
        }

        public string convertImpuesto(int valueImp)
        {
            string impuesto = null;
            switch (valueImp)
            {
                case 1:
                    impuesto = "001";
                    break;
                case 2:
                    impuesto = "002";
                    break;
                case 3:
                    impuesto = "003";
                    break;
                default:
                    return null;
            }
            
            
            return impuesto;
        } 
    }
}
