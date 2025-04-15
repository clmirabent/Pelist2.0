using Films.Models;
using Films.DAL;
using Films.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Films.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult SignUP() 
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                UsuarioDAL dal = new UsuarioDAL();
                Usuario usuario = dal.GetUsuarioLogin(model.Username, model.Password);

                if (usuario != null)
                {
                    HttpContext.Session.SetString("Username", model.Username);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUP(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                UsuarioDAL dal = new UsuarioDAL();
                Usuario usuario = new Usuario();
                usuario.UserName = model.UserName;
                
                Usuario usuarioExistente = dal.GetUsuarioLogin(usuario.UserName, model.Password);

                if (usuarioExistente != null)
                {
                    ModelState.AddModelError("", "Usuario existente");
                    return View(model);
                }

                dal.CreateUsuario(usuario, model.Password);

                Usuario validarCreacion = dal.GetUsuarioLogin(model.UserName, model.Password);

                if (validarCreacion != null) 
                {
                    HttpContext.Session.SetString("Username", usuario.UserName);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "No se ha podido crear usuario");
            }
            return View(model);
        }
    }
}
