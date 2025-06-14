using EmisoraVozDelEste.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmisoraVozDelEste.Controllers
{
    public class PermisosController : Controller
    {
        // GET: Permisos
        public ActionResult Index()
        {
            using (var db = new VozDelEsteEntities1())
            {
                var permisos = db.Permisos.Include(p => p.Roles).ToList();
                return View(permisos);
            }
        }
        
        [HttpPost]
        public ActionResult GuardarPermisos(List<int> PermisosSeleccionados)
        {
            using (var db = new VozDelEsteEntities1())
            {
                var permisos = db.Permisos.ToList();

                foreach (var permiso in permisos)
                {
                    permiso.PuedeVerDropdown = PermisosSeleccionados != null && PermisosSeleccionados.Contains(permiso.Id);
                }

                db.SaveChanges();

                return RedirectToAction("Index");
            }
        }

        // GET: Permisos/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Permisos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Permisos/Create
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

        // GET: Permisos/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Permisos/Edit/5
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

        // GET: Permisos/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Permisos/Delete/5
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
