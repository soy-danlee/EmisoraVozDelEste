using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmisoraVozDelEste.Models;
using System.Globalization;


namespace EmisoraVozDelEste.Controllers
{
    public class ProgramasController : Controller
    {
        private VozDelEsteEntities1 db = new VozDelEsteEntities1();

        // GET: Programas

        public ActionResult Index()
        {
            //var permisos = Session["Permisos"] as List<string>;
            //if (permisos == null || !permisos.Contains("VerGestionProgramas"))
            //{
            //    return RedirectToAction("AccesoDenegado", "Login");
            //}
            var programas = db.Programas.ToList();

            var ahora = DateTime.Now;
            string diaHoy = ahora.ToString("dddd", new CultureInfo("es-ES"));
            TimeSpan horaActual = ahora.TimeOfDay;

            var programaActual = programas.FirstOrDefault(p =>
                p.Dia.ToLower().Contains(diaHoy.ToLower())
                && p.Hora.HasValue
                && horaActual >= p.Hora.Value
                && horaActual < p.Hora.Value.Add(TimeSpan.FromHours(1)) // ACA EL CODIGO SUPONE QUE EL PROGRAMA DURA 1 HORA
            );

            ViewBag.ProgramaActual = programaActual;
            return View(programas);
        }



        // GET: Programas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var programa = db.Programas
                             .Include(p => p.Comentarios.Select(c => c.Clientes))
                             .FirstOrDefault(p => p.Id == id);

            if (programa == null)
            {
                return HttpNotFound();
            }

            return View(programa);
        }

        // GET: Programas/Create
        public ActionResult Create()
        {
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("CrearPrograma"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }

            return View();
        }

        // POST: Programas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Programas programa)
        {
            

            if (ModelState.IsValid)
            {
                db.Programas.Add(programa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(programa);
        }

        // GET: Programas/Edit/5
        public ActionResult Edit(int? id)
        {
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("EditarPrograma"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Programas programas = db.Programas.Find(id);
            if (programas == null)
            {
                return HttpNotFound();
            }
            return View(programas);
        }

        // POST: Programas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Nombre,Imagen,Descripcion,Dia,Hora")] Programas programas)
        {

            if (ModelState.IsValid)
            {
                db.Entry(programas).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(programas);
        }

        // GET: Programas/Delete/5
        public ActionResult Delete(int? id)
        {
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("EliminarPrograma"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Programas programas = db.Programas.Find(id);
            if (programas == null)
            {
                return HttpNotFound();
            }
            return View(programas);
        }

        // POST: Programas/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            
            Programas programas = db.Programas.Find(id);
            db.Programas.Remove(programas);
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

        public ActionResult Sala(int id)
        {
            var programa = db.Programas.Find(id);
            if (programa == null)
            {
                return HttpNotFound();
            }

            return View(programa);
        }

        public Programas ObtenerProgramaActual()
        {
            var ahora = DateTime.Now;
            string diaHoy = ahora.ToString("dddd", new CultureInfo("es-ES")); // toma el dia actual
            TimeSpan horaActual = ahora.TimeOfDay;

            var programas = db.Programas.ToList();

            foreach (var p in programas)
            {
                if (p.Dia.Contains(diaHoy) && p.Hora.HasValue)
                {
                    var horaInicio = p.Hora.Value;
                    var horaFin = horaInicio.Add(TimeSpan.FromHours(1)); 

                    if (horaActual >= horaInicio && horaActual < horaFin)
                    {
                        return p;
                    }
                }
            }

            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarComentario(int programaId, string comentario)
        {
            if (string.IsNullOrWhiteSpace(comentario))
                return RedirectToAction("Details", new { id = programaId });

            if (Session["ClienteCI"] == null)
                return RedirectToAction("Login", "Login");

            int clienteCI = Convert.ToInt32(Session["ClienteCI"]);

            var nuevoComentario = new Comentarios
            {
                ProgramaId = programaId,
                ClienteCI = clienteCI,
                Comentario = comentario,
                Fecha = DateTime.Now
            };

            db.Comentarios.Add(nuevoComentario);
            db.SaveChanges();

            return RedirectToAction("Sala", new { id = programaId });
        }

        
        public ActionResult ProgramasCliente(string busqueda)
        {
            
            var programas = db.Programas.AsQueryable(); 

            if (!string.IsNullOrEmpty(busqueda))
            {
                programas = programas
                    .Where(p => p.Nombre.Contains(busqueda));
            }
            
            var lista = programas.ToList();
            
            //var programaActual = db.Programas.FirstOrDefault(p => p.IsCurrent);
            //ViewBag.ProgramaActual = programaActual;
            
            return View(lista);
        }
    }
}
