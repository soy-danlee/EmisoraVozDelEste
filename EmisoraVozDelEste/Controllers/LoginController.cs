using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmisoraVozDelEste.Controllers
{
    public class LoginController : Controller
    {
        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            // Aquí pondrás la lógica de autenticación
            if (username == "admin" && password == "1234")
            {
                ViewBag.Message = "Inicio de sesión exitoso.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Message = "Usuario o contraseña incorrectos.";
            return View();
        }
    }
}