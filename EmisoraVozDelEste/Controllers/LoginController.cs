using EmisoraVozDelEste.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmisoraVozDelEste.Controllers
{
    public class LoginController : Controller
    {
        private VozDelEsteEntities1 db = new VozDelEsteEntities1();

        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            using (var db = new VozDelEsteEntities1())
            {
                var usuario = db.Usuarios.Include("Roles").FirstOrDefault(u => u.Nombre == username && u.Contraseña == password);

                if (usuario != null)
                {
                    Session["UsuarioNombre"] = usuario.Nombre;
                    Session["Rol"] = usuario.Roles.Nombre;

                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Message = "Usuario o contraseña incorrectos.";
                return View();
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult AccesoDenegado()
        {
            return View();
        }
    }
}