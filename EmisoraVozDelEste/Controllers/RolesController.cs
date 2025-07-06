using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmisoraVozDelEste.Models;

namespace EmisoraVozDelEste.Controllers
{
    public class RolesController : Controller
    {
        private VozDelEsteEntities1 db = new VozDelEsteEntities1();

        // GET: Roles
        public ActionResult Index()
        {
            var permisos = Session["Permisos"] as List<string>;

            if (permisos == null || !permisos.Contains("VerGestionRoles"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            return View(db.Roles.ToList());
        }

        // GET: Roles/Details/5
        public ActionResult Details(int? id)
        {
            var permisos = Session["Permisos"] as List<string>;

            if (permisos == null || !permisos.Contains("DetallesRoles"))
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var role = db.Roles.Include(r => r.Permisos).FirstOrDefault(r => r.Id == id);
            if (role == null)
                return HttpNotFound();

            return View(role);
        }

        // GET: Roles/Create
        public ActionResult Create()
        {
            var permisos = Session["Permisos"] as List<string>;

            if (permisos == null || !permisos.Contains("CrearRoles"))
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Nombre")] Roles role)
        {
            if (ModelState.IsValid)
            {
                db.Roles.Add(role);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(role);
        }

        // GET: Roles/Edit/5
        public ActionResult Edit(int? id)
        {

            // Verificar si el usuario tiene el permiso "Editar"
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("EditarRoles"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var role = db.Roles.Include(r => r.Permisos).FirstOrDefault(r => r.Id == id);
            if (role == null)
                return HttpNotFound();

            var allPermissions = db.Permisos.ToList();
            var rolePermissionIds = role.Permisos.Select(p => p.Id).ToList();

            var permissionCheckboxes = allPermissions.Select(p => new PermissionCheckbox
            {
                PermissionID = p.Id,
                PermissionName = p.Nombre,
                IsAssigned = rolePermissionIds.Contains(p.Id)
            }).ToList();

            var viewModel = new EditRoleViewModel
            {
                Id = role.Id,
                Nombre = role.Nombre,
                Permisos = permissionCheckboxes
            };

            return View(viewModel);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditRoleViewModel model)
        {
            // ✅ Validar si tiene permiso "Editar"
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("EditarRoles"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }

            if (ModelState.IsValid)
            {
                var role = db.Roles.Include(r => r.Permisos).FirstOrDefault(r => r.Id == model.Id);
                if (role == null)
                    return HttpNotFound();

                role.Nombre = model.Nombre;

                // Limpiar permisos actuales
                role.Permisos.Clear();

                // Agregar nuevos permisos seleccionados
                var selectedPermissionIds = model.Permisos
                    .Where(p => p.IsAssigned)
                    .Select(p => p.PermissionID)
                    .ToList();

                var selectedPermissions = db.Permisos
                    .Where(p => selectedPermissionIds.Contains(p.Id))
                    .ToList();

                foreach (var permiso in selectedPermissions)
                {
                    role.Permisos.Add(permiso);
                }

                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        var entityName = eve.Entry.Entity.GetType().Name;
                        foreach (var ve in eve.ValidationErrors)
                        {
                            var error = $"Entidad: {entityName}, Propiedad: {ve.PropertyName}, Error: {ve.ErrorMessage}";
                            System.Diagnostics.Debug.WriteLine(error);
                            ModelState.AddModelError(ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    // ❗️Pasamos a reconstruir los permisos correctamente más abajo
                }
            }

            // ⚠️ Siempre reconstruir la lista de permisos aunque haya errores
            var permisosAsignados = model.Permisos
    ?.Where(p => p.IsAssigned)
    .Select(p => p.PermissionID)
    .ToList() ?? new List<int>();

            model.Permisos = db.Permisos
                .Select(p => new PermissionCheckbox
                {
                    PermissionID = p.Id,
                    PermissionName = p.Nombre,
                    IsAssigned = permisosAsignados.Contains(p.Id)
                })
                .ToList();

            return View(model);
        }

        // GET: Roles/Delete/5
        public ActionResult Delete(int? id)
        {
            var permisos = Session["Permisos"] as List<string>;

            if (permisos == null || !permisos.Contains("EliminarRoles"))
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var role = db.Roles.Find(id);
            if (role == null)
                return HttpNotFound();

            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var role = db.Roles.Find(id);
            if (role != null)
            {
                db.Roles.Remove(role);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }

    // ViewModel para la edición de Roles con Permisos
    public class EditRoleViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<PermissionCheckbox> Permisos { get; set; }
    }

    // Clase para manejar el checkbox de permisos en la vista
    public class PermissionCheckbox
    {
        public int PermissionID { get; set; }
        public string PermissionName { get; set; }
        public bool IsAssigned { get; set; }
    }
}
