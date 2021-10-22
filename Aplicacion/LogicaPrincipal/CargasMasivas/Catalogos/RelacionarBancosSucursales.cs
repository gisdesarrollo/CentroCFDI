using API.Enums;
using API.Relaciones;
using Aplicacion.Context;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;

namespace Aplicacion.LogicaPrincipal.CargasMasivas.Catalogos
{
    public class RelacionarBancosSucursales
    {

        #region Variables

        private readonly AplicacionContext _db = new AplicacionContext();

        #endregion

        public List<String> Importar(string path, int sucursalId)
        {
            var errores = new List<String>();
            using (StreamReader archivo = File.OpenText(path))
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
                    try
                    {
                        var bancoSucursal = new BancoSucursal();

                        var razonSocialSucursal = registros[i][0];
                        var nombreBanco = registros[i][1];
                        var nombre = registros[i][2];
                        var numeroCuenta = registros[i][3];

                        var sucursal = _db.Sucursales.FirstOrDefault(p => p.RazonSocial == razonSocialSucursal);
                        if (sucursal == null)
                        {
                            throw new Exception(String.Format("El sucursal {0} no fue encontrado en el registro {1}", razonSocialSucursal, i));
                        }
                        bancoSucursal.SucursalId = sucursal.Id;

                        var banco = _db.Bancos.FirstOrDefault(p => p.NombreCorto == nombreBanco);
                        if (banco == null)
                        {
                            throw new Exception(String.Format("El banco {0} no fue encontrado en el registro {1}", nombreBanco, i));
                        }
                        bancoSucursal.BancoId = banco.Id;

                        bancoSucursal.Nombre = nombre;
                        bancoSucursal.NumeroCuenta = numeroCuenta;

                        //Sustitucion de espacios en blanco por null
                        foreach (var propertyInfo in bancoSucursal.GetType().GetProperties())
                        {
                            if (propertyInfo.PropertyType == typeof(string))
                            {
                                if ((String)propertyInfo.GetValue(bancoSucursal, null) == string.Empty)
                                {
                                    propertyInfo.SetValue(bancoSucursal, null, null);
                                }
                            }
                        }

                        //Registro Existente
                        var registroAnterior = _db.BancosSucursales.FirstOrDefault(bc => bc.SucursalId == sucursal.Id && bc.BancoId == banco.Id && bc.NumeroCuenta == numeroCuenta);
                        if (registroAnterior != null)
                        {
                            bancoSucursal.Id = registroAnterior.Id;
                        }

                        //Actualizacion
                        _db.BancosSucursales.AddOrUpdate(bancoSucursal);

                        try
                        {
                            _db.SaveChanges();
                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            var erroresDb = new List<String>();
                            foreach (var validationErrors in dbEx.EntityValidationErrors)
                            {
                                foreach (var validationError in validationErrors.ValidationErrors)
                                {
                                    erroresDb.Add(String.Format("Propiedad: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage));
                                }
                            }
                            throw new Exception(string.Join(",", erroresDb.ToArray()));
                        }
                        catch (Exception ex)
                        {
                            if (ex.InnerException != null)
                            {
                                throw new Exception(ex.InnerException.InnerException.Message);
                            }
                            throw ex;
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
            }
            return errores;
        }

        public String Exportar(int sucursalId, bool prepopulado)
        {
            var path = String.Format("C:/Infodextra/Temp/BancosSucursales_S{0}_{1}.csv", sucursalId, DateTime.Now.ToString("ddMMyyyyHHmmss"));
            var bancosSucursales = _db.BancosSucursales.Where(t => t.Banco.Status == Status.Activo && t.Sucursal.Status == Status.Activo && t.SucursalId == sucursalId).ToList();

            #region Encabezados

            var encabezados = new List<String>
            {
                "Razon Social del Sucursal",
                "Nombre Corte del Banco",
                "Nombre de la Cuenta",
                "Numero de Cuenta",
            };

            #endregion

            #region Registros

            var registros = new List<List<String>>();
            if (prepopulado)
            {
                foreach (var bancoSucursal in bancosSucursales)
                {
                    var registroBancoSucursal = new List<String>();

                    registroBancoSucursal.Add(bancoSucursal.Sucursal.RazonSocial);
                    registroBancoSucursal.Add(bancoSucursal.Banco.NombreCorto);
                    registroBancoSucursal.Add(bancoSucursal.Nombre);
                    registroBancoSucursal.Add(bancoSucursal.NumeroCuenta);

                    registros.Add(registroBancoSucursal);
                }
            }

            #endregion

            using (var textWriter = File.CreateText(path))
            {
                using (var csv = new CsvWriter(textWriter))
                {
                    csv.WriteField(encabezados);
                    if (prepopulado)
                    {
                        foreach (var registro in registros)
                        {
                            csv.NextRecord();

                            for (int i = 0; i < registro.Count; i++)
                            {
                                if (registro[i] == null)
                                {
                                    registro[i] = string.Empty;
                                }
                            }

                            csv.WriteField(registro);
                        }
                    }
                }
                textWriter.Close();
            }

            return path;
        }
    }
}
