using EmisoraVozDelEste.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace EmisoraVozDelEste
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            CargarProgramasIniciales();
        }
        private void CargarProgramasIniciales()
        {
            using (var db = new VozDelEsteEntities1())
            {
                if (!db.Programas.Any())
                {
                    var programa = new Programas
                    {
                        Nombre = "Hola",
                        Imagen = "https://img.freepik.com/foto-gratis/balon-futbol-linea-blanca-estadio_1150-5285.jpg?semt=ais_items_boosted&w=740",
                        Descripcion = "Descripcion aca",
                        Dia = "Lunes a viernes",
                        Hora = new TimeSpan(20, 0, 0)
                    };
                    db.Programas.Add(programa);
                    db.SaveChanges();
                }
            }
        }
    }
}
