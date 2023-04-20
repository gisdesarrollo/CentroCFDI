using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace APBox.Controllers.Operaciones
{
    public class AddendaController : Controller
    {
        // GET: Addenda
        public ActionResult Index()
        {
            return View();
        }

        // GET: Addenda/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Addenda/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Addenda/Create
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

        // GET: Addenda/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Addenda/Edit/5
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

        // GET: Addenda/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Addenda/Delete/5
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
