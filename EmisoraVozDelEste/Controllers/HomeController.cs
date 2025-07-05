using EmisoraVozDelEste.Models.Api;
using EmisoraVozDelEste.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EmisoraVozDelEste.Controllers
{
    public class HomeController : Controller
    {
        private VozDelEsteEntities1 db = new VozDelEsteEntities1();

        public async Task<ActionResult> Index()
        {
            var modelo = new HomePageViewModel();

            // --- Clima ---
            try
            {
                string ciudad = "Maldonado,UY";
                string apiKeyClima = "99a62615b949cb6d8a96f76f97c2ff69";
                string urlPronostico = $"https://api.openweathermap.org/data/2.5/forecast?q={ciudad}&appid={apiKeyClima}&units=metric&lang=es";

                using (var client = new WebClient())
                {
                    var jsonPronostico = client.DownloadString(urlPronostico);
                    var clima = ClimaOnline.FromJson(jsonPronostico);
                    var hoy = DateTime.Now.Date;

                    var primerPronosticoHoy = clima.List
                        .Where(x => x.DtTxt.Date == hoy)
                        .OrderBy(x => x.DtTxt)
                        .FirstOrDefault();

                    if (primerPronosticoHoy != null)
                    {
                        modelo.ClimaSidebar = new Clima
                        {
                            Fecha = primerPronosticoHoy.DtTxt,
                            Temperatura = (decimal)primerPronosticoHoy.Main.Temp,
                            Descripcion = primerPronosticoHoy.Weather[0].Description,
                            Icono = primerPronosticoHoy.Weather[0].Icon
                        };
                    }
                }
            }
            catch
            {
                modelo.ClimaSidebar = null;
            }

            // --- Cotizaciones ---
            try
            {
                var cotizacionesRecientes = db.Cotizaciones
                    .Where(c => DbFunctions.TruncateTime(c.Fecha) == DateTime.Today)
                    .ToList();

                modelo.Cotizaciones = cotizacionesRecientes;
            }
            catch
            {
                modelo.Cotizaciones = new List<Cotizaciones>();
            }

            // --- Últimas Noticias ---
            modelo.UltimasNoticias = db.Noticias
                .OrderByDescending(n => n.FechaPublicacion)
                .Take(3)
                .ToList();

            // --- Próximos Programas de Hoy ---
            var diaActual = ObtenerDiaSemanaEnEspañol();
            var horaActual = DateTime.Now.TimeOfDay;

            modelo.ProximosProgramas = db.Programas
                .Where(p => p.Dia == diaActual && p.Hora > horaActual)
                .OrderBy(p => p.Hora)
                .Take(3)
                .ToList();

            return View(modelo);
        }

        private string ObtenerDiaSemanaEnEspañol()
        {
            var dias = new Dictionary<DayOfWeek, string>
            {
                { DayOfWeek.Monday, "Lunes" },
                { DayOfWeek.Tuesday, "Martes" },
                { DayOfWeek.Wednesday, "Miércoles" },
                { DayOfWeek.Thursday, "Jueves" },
                { DayOfWeek.Friday, "Viernes" },
                { DayOfWeek.Saturday, "Sábado" },
                { DayOfWeek.Sunday, "Domingo" },
            };

            return dias[DateTime.Now.DayOfWeek];
        }

        public ActionResult About() => View();
        public ActionResult Contact() => View();
    }
}