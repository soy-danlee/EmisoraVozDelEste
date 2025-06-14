using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmisoraVozDelEste.Controllers
{
    public class PanelController : Controller
    {
        // GET: Panel
        public ActionResult Index()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public ActionResult GuardarPermisos(List<string> Permisos)
        {
            // Aquí puedes guardar los permisos en la base de datos, en Session o en otro lado
            Session["PermisosAsignados"] = Permisos;

            TempData["Mensaje"] = "Permisos guardados correctamente.";
            return RedirectToAction("Index");
        }
    }
}