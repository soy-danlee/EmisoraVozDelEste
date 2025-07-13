using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmisoraVozDelEste.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Entity.Validation;

namespace EmisoraVozDelEste.Controllers
{
    public class NoticiasController : Controller
    {
        private VozDelEsteEntities1 db = new VozDelEsteEntities1();

        // GET: Noticias
        public ActionResult Index()
        {
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("VerGestionNoticias"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            return View(db.Noticias.ToList());
        }

        // GET: Noticias/Details/5
        public ActionResult Details(int id)
        {
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("DetallesNoticia"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            var noticia = db.Noticias.Find(id);
            if (noticia == null)
            {
                return HttpNotFound();
            }
            return View(noticia);  
        }

        // GET: Noticias/Create
        
        public ActionResult Create()
        {
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("CrearNoticia"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            return View();
        }

        // POST: Noticias/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Titulo,Contenido,FechaPublicacion,Imagen")] Noticias noticias)
        {
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("CrearNoticia"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    db.Noticias.Add(noticias);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        foreach (var ve in eve.ValidationErrors)
                        {
                            ModelState.AddModelError(ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                }
            }

            return View(noticias);
        }

        // GET: Noticias/Edit/5
        public ActionResult Edit(int? id)
        {
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("EditarNoticia"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Noticias noticias = db.Noticias.Find(id);
            if (noticias == null)
            {
                return HttpNotFound();
            }
            return View(noticias);
        }

        // POST: Noticias/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost ]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Titulo,Contenido,FechaPublicacion,Imagen")] Noticias noticias)
        {
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("EditarNoticia"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            if (ModelState.IsValid)
            {
                db.Entry(noticias).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(noticias);
        }

        // GET: Noticias/Delete/5
        public ActionResult Delete(int? id)
        {
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("EliminarNoticia"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Noticias noticias = db.Noticias.Find(id);
            if (noticias == null)
            {
                return HttpNotFound();
            }
            return View(noticias);
        }

        // POST: Noticias/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Noticias noticias = db.Noticias.Find(id);
            db.Noticias.Remove(noticias);
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

        public ActionResult NoticiasCliente(string busqueda)
        {
            var noticias = db.Noticias.AsQueryable(); // o List<Noticias>

            if (!string.IsNullOrEmpty(busqueda))
            {
                noticias = noticias
                    .Where(n => n.Titulo.Contains(busqueda));
            }

            var lista = noticias
                .OrderByDescending(n => n.FechaPublicacion)
                .ToList();

            return View(lista);
        }

        public ActionResult Detalle(int id)
        {
            var noticia = db.Noticias.Find(id);
            if (noticia == null)
            {
                return HttpNotFound();
            }
            return View(noticia);
        }
        public ActionResult NoticiasAmpliada(int id)
        {
            var noticia = db.Noticias.FirstOrDefault(n => n.Id == id);
            if (noticia == null)
                return HttpNotFound();     

            return View(noticia);      
        }
    }
}
