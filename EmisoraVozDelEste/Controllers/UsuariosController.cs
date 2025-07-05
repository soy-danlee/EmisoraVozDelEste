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
    public class UsuariosController : Controller
    {
        private VozDelEsteEntities1 db = new VozDelEsteEntities1();

        // GET: Usuarios
        public ActionResult Index()
        {
            var usuarios = db.Usuarios.Include(u => u.Roles);
            return View(usuarios.ToList());
        }

        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Usuarios usuario = db.Usuarios.Find(id);
            if (usuario == null)
                return HttpNotFound();

            return View(usuario);
        }

        // GET: Usuarios/Create
        public ActionResult Create()
        {
            ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre");
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Email,Contraseña,RolId")] Usuarios usuario)
        {
            if (ModelState.IsValid)
            {
                // Hashear contraseña antes de guardar
                usuario.Contraseña = BCrypt.Net.BCrypt.HashPassword(usuario.Contraseña);

                db.Usuarios.Add(usuario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // GET: Usuarios/Edit/5
        [HttpPost, ActionName("Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Usuarios usuario = db.Usuarios.Find(id);
            if (usuario == null)
                return HttpNotFound();

            // No mostrar la contraseña hasheada en la vista, dejar vacío para cambiar si se quiere
            usuario.Contraseña = null;

            ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Email,Contraseña,RolId")] Usuarios usuario)
        {
            if (ModelState.IsValid)
            {
                var usuarioOriginal = db.Usuarios.AsNoTracking().FirstOrDefault(u => u.Id == usuario.Id);

                if (usuarioOriginal == null)
                    return HttpNotFound();

                if (!string.IsNullOrWhiteSpace(usuario.Contraseña))
                {
                    // Si se ingresó nueva contraseña, hashearla
                    usuario.Contraseña = BCrypt.Net.BCrypt.HashPassword(usuario.Contraseña);
                }
                else
                {
                    // Si no se cambió, mantener la antigua
                    usuario.Contraseña = usuarioOriginal.Contraseña;
                }

                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // GET: Usuarios/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Usuarios usuario = db.Usuarios.Find(id);
            if (usuario == null)
                return HttpNotFound();

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuarios usuario = db.Usuarios.Find(id);
            db.Usuarios.Remove(usuario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
