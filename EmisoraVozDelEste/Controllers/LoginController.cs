﻿using EmisoraVozDelEste.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

namespace EmisoraVozDelEste.Controllers
{
    public class LoginController : Controller
    {
        private VozDelEsteEntities1 db = new VozDelEsteEntities1();

        // GET: Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Debe ingresar usuario y contraseña.";
                return View();
            }

            var user = db.Usuarios
                         .Include("Roles.Permisos")
                         .FirstOrDefault(u => u.Nombre.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Contraseña))
            {
                FormsAuthentication.SetAuthCookie(user.Nombre, false);

                Session["UsuarioNombre"] = user.Nombre;
                Session["Rol"] = user.Roles?.Nombre ?? "Sin rol";
                Session["Permisos"] = user.Roles?.Permisos?.Select(p => p.Nombre).ToList() ?? new List<string>();

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Usuario o contraseña incorrectos.";
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login");
        }


        [Authorize]
        public ActionResult RefreshPermissions()
        {
            var userName = User.Identity.Name;

            var user = db.Usuarios
                         .Include("Roles.Permisos")
                         .FirstOrDefault(u => u.Nombre.Equals(userName, StringComparison.OrdinalIgnoreCase));

            if (user != null)
            {
                Session["Rol"] = user.Roles?.Nombre ?? "Sin rol";
                Session["Permisos"] = user.Roles?.Permisos?.Select(p => p.Nombre).ToList() ?? new List<string>();
            }

            return RedirectToAction("Index", "Home");
        }
        public ActionResult AccesoDenegado()
        {
            return View();
        }
    }
}