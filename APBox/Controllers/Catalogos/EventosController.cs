using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APBox.Controllers.Catalogos
{
    public class EventosController : Controller
    {
        // GET: Eventos
        public ActionResult Index()
        {
            return View();
        }

        // GET: Eventos/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Eventos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Eventos/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Eventos/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Eventos/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Eventos/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Eventos/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
