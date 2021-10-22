using API.Models.Cargas;
using Aplicacion.LogicaPrincipal.CargasMasivas.Catalogos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Web.Mvc;

namespace APBox.Controllers.Operaciones
{
    public class CargasMasivasController : Controller
    {

        #region Variables

        private readonly RelacionarBancosClientes _relacionarBancosClientes = new RelacionarBancosClientes();
        private readonly RelacionarBancosSucursales _relacionarBancosSucursales = new RelacionarBancosSucursales();

        #endregion

        // GET: CargasMasivas
        public ActionResult RelacionClientesBancos()
        {
            var cargasMasivasModel = new CargasMasivasModel
            {
                Prepopulado = true,
                SucursalId = ObtenerSucursal()
            };

            return View(cargasMasivasModel);
        }

        [HttpPost]
        public ActionResult RelacionClientesBancos(CargasMasivasModel cargasMasivasModel, string actionName)
        {
            if (ModelState.IsValid)
            {
                var errores = new List<String>();
                try
                {
                    switch (actionName)
                    {
                        case "Cargar":
                            var path = SubeArchivo();
                            errores = _relacionarBancosClientes.Importar(path, ObtenerSucursal());
                            if (errores.Count > 0)
                            {
                                foreach (var error in errores)
                                {
                                    ModelState.AddModelError("", error);
                                }
                            }
                            break;
                        case "Descargar":
                            var pathArchivo = _relacionarBancosClientes.Exportar(cargasMasivasModel.SucursalId, cargasMasivasModel.Prepopulado);
                            return File(pathArchivo, MediaTypeNames.Application.Octet, Path.GetFileName(pathArchivo));
                    }
                    if (errores.Count == 0)
                    {
                        ModelState.AddModelError("", "Comando realizado con éxito");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(cargasMasivasModel);
        }

        public ActionResult RelacionSucursalesBancos()
        {
            var cargasMasivasModel = new CargasMasivasModel
            {
                Prepopulado = true,
                SucursalId = ObtenerSucursal()
            };

            return View(cargasMasivasModel);
        }

        [HttpPost]
        public ActionResult RelacionSucursalesBancos(CargasMasivasModel cargasMasivasModel, string actionName)
        {
            if (ModelState.IsValid)
            {
                var errores = new List<String>();
                try
                {
                    switch (actionName)
                    {
                        case "Cargar":
                            var path = SubeArchivo();
                            errores = _relacionarBancosSucursales.Importar(path, ObtenerSucursal());
                            if (errores.Count > 0)
                            {
                                foreach (var error in errores)
                                {
                                    ModelState.AddModelError("", error);
                                }
                            }
                            break;
                        case "Descargar":
                            var pathArchivo = _relacionarBancosSucursales.Exportar(cargasMasivasModel.SucursalId, cargasMasivasModel.Prepopulado);
                            return File(pathArchivo, MediaTypeNames.Application.Octet, Path.GetFileName(pathArchivo));
                    }
                    if (errores.Count == 0)
                    {
                        ModelState.AddModelError("", "Comando realizado con éxito");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(cargasMasivasModel);
        }

        #region Popula Forma

        private int ObtenerSucursal()
        {
            return Convert.ToInt32(Session["SucursalId"]);
        }

        #endregion

        #region Archivos

        private String SubeArchivo()
        {
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Archivos/CargasMasivas/"), fileName);
                        file.SaveAs(path);
                        return path;
                    }
                    catch (Exception)
                    {
                    }

                }
            }
            return null;
        }

        #endregion

    }
}