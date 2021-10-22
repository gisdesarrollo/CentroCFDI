using APBox.Context;
using API.Models.Control;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace APBox.Controllers.Control
{
    [APBox.Control.SessionExpire]
    public class ControlController : Controller
    {

        #region Variables

        private readonly APBoxContext _db = new APBoxContext();

        #endregion

        //[Authorize(Roles = "CONFIGURACIONES")]
        public ActionResult Configuraciones()
        {
            var configuracionesModel = new ConfiguracionesModel
            {
                Configuraciones = _db.Configuraciones.ToList()
            };
            return View(configuracionesModel);
        }

        [HttpPost]
        //[Authorize(Roles = "CONFIGURACIONES")]
        public ActionResult Configuraciones(ConfiguracionesModel configuracionesModel)
        {
            if (ModelState.IsValid)
            {
                configuracionesModel.Configuraciones.ForEach(p => _db.Entry(p).State = EntityState.Modified);
                _db.SaveChanges();

                ModelState.AddModelError("", "Comando realizado con éxito");
                return View(configuracionesModel);
            }
            return View(configuracionesModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}