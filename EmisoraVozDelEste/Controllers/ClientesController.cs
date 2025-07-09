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
    public class ClientesController : Controller
    {
        private VozDelEsteEntities1 db = new VozDelEsteEntities1();

        // GET: Clientes
        public ActionResult Index()
        {
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("VerGestionClientes"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            return View(db.Clientes.ToList());
        }

        // GET: Clientes/Details/5
        public ActionResult Details(int? id)
        {
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("DetallesCliente"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes clientes = db.Clientes.Find(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            return View(clientes);
        }

        // GET: Clientes/Create
        public ActionResult Create()
        {
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("CrearCliente"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            return View();
        }

        // POST: Clientes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CI,Nombre,Apellido,Email,FechaNacimiento")] Clientes clientes)
        {
            if (ModelState.IsValid)
            {
                db.Clientes.Add(clientes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(clientes);
        }

        // GET: Clientes/Edit/5
        public ActionResult Edit(int? id)
        {
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("EditarCliente"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes clientes = db.Clientes.Find(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            return View(clientes);
        }

        // POST: Clientes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CI,Nombre,Apellido,Email,FechaNacimiento,UsuarioID")] Clientes clientes)
        {
            if (ModelState.IsValid)
            {
                
                bool usuarioExiste = db.Usuarios.Any(u => u.Id == clientes.UsuarioID);
                if (!usuarioExiste)
                {
                    ModelState.AddModelError("UsuarioID", "El usuario ingresado no existe.");
                    return View(clientes);
                }

                
                bool usuarioYaUsado = db.Clientes.Any(c => c.UsuarioID == clientes.UsuarioID && c.CI != clientes.CI);
                if (usuarioYaUsado)
                {
                    ModelState.AddModelError("UsuarioID", "Este usuario ya está asignado a otro cliente.");
                    return View(clientes);
                }

                // Todo está bien, actualizar
                db.Entry(clientes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(clientes);
        }

        // GET: Clientes/Delete/5
        public ActionResult Delete(int? id)
        {
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("EliminarCliente"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Clientes cliente = db.Clientes.Find(id);
            if (cliente == null)
                return HttpNotFound();

            return View(cliente);
        }

        // POST: Clientes/DeleteConfirmed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int CI) // ✅ Nombre del parámetro igual al HiddenField
        {
            var cliente = db.Clientes.Find(CI);
            if (cliente == null)
                return HttpNotFound();

            try
            {
                db.Clientes.Remove(cliente);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar el cliente. Asegúrese de que no tiene relaciones pendientes.");
                return View(cliente);
            }

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
