using EmisoraVozDelEste.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmisoraVozDelEste.Controllers
{
    public class AutorizacionController : Controller
    {


        private VozDelEsteEntities1 db = new VozDelEsteEntities1();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Solo si está logueado:
            if (User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name;

                // Trae User con Role y Permissions:
                var user = db.Usuarios
                             .Include("Roles.Permisos")
                             .FirstOrDefault(u => u.Nombre == userName);

                if (user != null)
                {
                    Session["Rol"] = user.Roles.Nombre;
                    Session["Permisos"] = user.Roles.Permisos.Select(p => p.Nombre).ToList();
                }
            }
            else
            {
                Session["Rol"] = null;
                Session["Permisos"] = null;
            }
        }


    }
}