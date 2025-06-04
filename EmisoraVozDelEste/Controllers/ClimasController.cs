using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmisoraVozDelEste.Models;

namespace EmisoraVozDelEste.Controllers
{
    public class ClimasController : Controller
    {
        private VozDelEsteEntities1 db = new VozDelEsteEntities1();

        // GET: Climas
        public ActionResult Index()
        {
            return View(db.Clima.ToList());
        }

        // GET: Climas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clima clima = db.Clima.Find(id);
            if (clima == null)
            {
                return HttpNotFound();
            }
            return View(clima);
        }

        // GET: Climas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Climas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Fecha,Temperatura,Descripcion,Icono")] Clima clima)
        {
            if (ModelState.IsValid)
            {
                db.Clima.Add(clima);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(clima);
        }

        // GET: Climas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clima clima = db.Clima.Find(id);
            if (clima == null)
            {
                return HttpNotFound();
            }
            return View(clima);
        }

        // POST: Climas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Fecha,Temperatura,Descripcion,Icono")] Clima clima)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clima).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clima);
        }

        // GET: Climas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clima clima = db.Clima.Find(id);
            if (clima == null)
            {
                return HttpNotFound();
            }
            return View(clima);
        }

        // POST: Climas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Clima clima = db.Clima.Find(id);
            db.Clima.Remove(clima);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
