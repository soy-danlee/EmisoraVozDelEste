using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EmisoraVozDelEste.Models;
using EmisoraVozDelEste.Models.Api;
using System.Globalization;
using Newtonsoft.Json;

namespace EmisoraVozDelEste.Controllers
{
    public class ClimasController : Controller
    {
        private VozDelEsteEntities1 db = new VozDelEsteEntities1();

        public ActionResult ClimaOnlineVista()
        {
            string apiKey = "99a62615b949cb6d8a96f76f97c2ff69";
            string ciudad = "Maldonado,UY";

            // URLs de OpenWeather
            string urlActual = $"https://api.openweathermap.org/data/2.5/weather?q={ciudad}&appid={apiKey}&units=metric&lang=es";
            string urlPronostico = $"https://api.openweathermap.org/data/2.5/forecast?q={ciudad}&appid={apiKey}&units=metric&lang=es";

            using (var client = new WebClient())
            {
                // Clima actual
                string jsonActual = client.DownloadString(urlActual);
                var climaActual = JsonConvert.DeserializeObject<ClimaActual>(jsonActual);

                // Pronóstico 5 días
                string jsonPronostico = client.DownloadString(urlPronostico);
                var clima = ClimaOnline.FromJson(jsonPronostico);

                var hoy = DateTime.Now.Date;

                // Pronóstico del día actual (hoy), luego del momento actual
                var bloquesHoy = clima.List
                    .Where(x => x.DtTxt.Date == hoy && x.DtTxt > DateTime.Now)
                    .Take(3) // Mostramos solo los próximos 3 bloques
                    .ToList();

                var diarios = clima.List
                    .Where(x => x.DtTxt.Hour == 12)
                    .GroupBy(x => x.DtTxt.Date)
                    .Select(g => g.First())
                    .Take(5)
                    .ToList();

                var modelo = new ClimaViewModel
                {
                    Actual = climaActual,
                    Pronostico = diarios,
                    BloquesHoy = bloquesHoy,
                    Ciudad = clima.City.Name
                };

                return View("ClimaOnline", modelo);
            }
        }


        // GET: Climas
        public ActionResult Index()
        {
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("VerClima"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
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
