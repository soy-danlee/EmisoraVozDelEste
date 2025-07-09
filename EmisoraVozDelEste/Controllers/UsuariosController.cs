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
            var permisos = Session["Permisos"] as List<string>;

            if (permisos == null || !permisos.Contains("VerGestionUsuarios"))
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

            var usuarios = db.Usuarios.Include(u => u.Roles);
            return View(usuarios.ToList());
        }

        // GET: Usuarios/Details/5
        public ActionResult Details(int? id)
        {
            var permisos = Session["Permisos"] as List<string>;

            if (permisos == null || !permisos.Contains("VerDetallesUsuario"))
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }

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
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("CrearUsuario"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre");
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre,Email,Contraseña,RolId")] Usuarios usuario)
        {
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("CrearUsuario"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }

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
        
        public ActionResult Edit(int? id)
        {
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("EditarUsuario"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var usuario = db.Usuarios.Find(id);
            if (usuario == null)
                return HttpNotFound();

            usuario.Contraseña = null;
            ViewBag.RolId = new SelectList(db.Roles, "Id", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
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
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("EliminarUsuario"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Usuarios usuario = db.Usuarios.Find(id);
            if (usuario == null)
                return HttpNotFound();

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var usuario = db.Usuarios.Find(id);
            if (usuario == null)
                return HttpNotFound();

            // Verificar si el usuario tiene clientes asociados
            bool tieneClientes = db.Clientes.Any(c => c.UsuarioID == id);

            if (tieneClientes)
            {
                ModelState.AddModelError("", "No se puede eliminar el usuario porque tiene clientes asignados.");
                return View("Delete", usuario); // Mostrar la vista de borrado con el mensaje de error
            }

            // Si no tiene clientes, eliminar usuario
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
