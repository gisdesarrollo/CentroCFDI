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
    public class RelacionarBancosClientes
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
                var sucursal = _db.Sucursales.Find(sucursalId);

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
                        var bancoCliente = new BancoCliente();

                        var rfcCliente = registros[i][0];
                        var nombreBanco = registros[i][1];
                        var nombre = registros[i][2];
                        var numeroCuenta = registros[i][3];

                        var cliente = _db.Clientes.FirstOrDefault(p => p.Rfc == rfcCliente && p.SucursalId == sucursalId);
                        if (cliente == null)
                        {
                            throw new Exception(String.Format("El cliente con RFC {0} no fue encontrado en el registro {1}", rfcCliente, i));
                        }
                        bancoCliente.ClienteId = cliente.Id;

                        var banco = _db.Bancos.FirstOrDefault(p => p.NombreCorto == nombreBanco);
                        if (banco == null)
                        {
                            throw new Exception(String.Format("El banco {0} no fue encontrado en el registro {1}", nombreBanco, i));
                        }
                        bancoCliente.BancoId = banco.Id;

                        bancoCliente.Nombre = nombre;
                        bancoCliente.NumeroCuenta = numeroCuenta;

                        //Sustitucion de espacios en blanco por null
                        foreach (var propertyInfo in bancoCliente.GetType().GetProperties())
                        {
                            if (propertyInfo.PropertyType == typeof(string))
                            {
                                if ((String)propertyInfo.GetValue(bancoCliente, null) == string.Empty)
                                {
                                    propertyInfo.SetValue(bancoCliente, null, null);
                                }
                            }
                        }

                        //Registro Existente
                        var registroAnterior = _db.BancosClientes.FirstOrDefault(bc => bc.ClienteId == cliente.Id && bc.BancoId == banco.Id && bc.NumeroCuenta == numeroCuenta);
                        if(registroAnterior != null)
                        {
                            bancoCliente.Id = registroAnterior.Id;
                        }

                        //Actualizacion
                        _db.BancosClientes.AddOrUpdate(bancoCliente);

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
            var path = String.Format("C:/Infodextra/Temp/BancosClientes_S{0}_{1}.csv", sucursalId, DateTime.Now.ToString("ddMMyyyyHHmmss"));
            var bancosClientes = _db.BancosClientes.Where(t => t.Banco.Status == Status.Activo && t.Cliente.Status == Status.Activo && t.Cliente.SucursalId == sucursalId).ToList();

            #region Encabezados

            var encabezados = new List<String>
            {
                "RFC del Cliente",
                "Nombre Corto del Banco",
                "Nombre de la Cuenta",
                "Numero de Cuenta",
            };

            #endregion

            #region Registros

            var registros = new List<List<String>>();
            if (prepopulado)
            {
                foreach (var bancoCliente in bancosClientes)
                {
                    var registroBancoCliente = new List<String>();

                    registroBancoCliente.Add(bancoCliente.Cliente.Rfc);
                    registroBancoCliente.Add(bancoCliente.Banco.NombreCorto);
                    registroBancoCliente.Add(bancoCliente.Nombre);
                    registroBancoCliente.Add(bancoCliente.NumeroCuenta);

                    registros.Add(registroBancoCliente);
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
