using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using EmisoraVozDelEste.Models;
using Newtonsoft.Json;
using System.Data.Entity;

namespace EmisoraVozDelEste.Controllers
{
    public class CotizacionesController : Controller
    {
        private VozDelEsteEntities1 db = new VozDelEsteEntities1();
        private readonly string apiKey = "60a82924e27eb538ce21c6a1568f779b";

        public async Task<ActionResult> CotizacionesOnline()
        {
            string url = $"http://api.currencylayer.com/live?access_key={apiKey}&currencies=UYU,EUR,ARS&source=USD&format=1";

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                var apiResult = JsonConvert.DeserializeObject<CurrencyApiResponse>(response);

                if (!apiResult.Success)
                {
                    return Content("Error al obtener cotizaciones.");
                }

                var cotizaciones = apiResult.Quotes.Select(q => new Cotizaciones
                {
                    Fecha = DateTime.Now,
                    TipoMoneda = q.Key,
                    Valor = q.Value
                }).ToList();

                foreach (var coti in cotizaciones)
                {
                    db.Cotizaciones.Add(coti);
                }
                db.SaveChanges();

                return View(cotizaciones);
            }
        }

        // GET: Cotizaciones
        public ActionResult Index()
        {
            var permisos = Session["Permisos"] as List<string>;
            if (permisos == null || !permisos.Contains("VerCotizacion"))
            {
                return RedirectToAction("AccesoDenegado", "Login");
            }
            return View(db.Cotizaciones.ToList());
        }

        // GET: Cotizaciones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Cotizaciones cotizaciones = db.Cotizaciones.Find(id);
            if (cotizaciones == null)
                return HttpNotFound();

            return View(cotizaciones);
        }

        // GET: Cotizaciones/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cotizaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Fecha,TipoMoneda,Valor")] Cotizaciones cotizaciones)
        {
            if (ModelState.IsValid)
            {
                db.Cotizaciones.Add(cotizaciones);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cotizaciones);
        }

        // GET: Cotizaciones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Cotizaciones cotizaciones = db.Cotizaciones.Find(id);
            if (cotizaciones == null)
                return HttpNotFound();

            return View(cotizaciones);
        }

        // POST: Cotizaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Fecha,TipoMoneda,Valor")] Cotizaciones cotizaciones)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cotizaciones).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cotizaciones);
        }

        // GET: Cotizaciones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Cotizaciones cotizaciones = db.Cotizaciones.Find(id);
            if (cotizaciones == null)
                return HttpNotFound();

            return View(cotizaciones);
        }

        // POST: Cotizaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cotizaciones cotizaciones = db.Cotizaciones.Find(id);
            db.Cotizaciones.Remove(cotizaciones);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }

        // Clase auxiliar interna
        private class CurrencyApiResponse
        {
            public bool Success { get; set; }
            public string Source { get; set; }
            public Dictionary<string, decimal> Quotes { get; set; }
        }
    }
}
