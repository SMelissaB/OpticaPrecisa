using CapaNegocio;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OpticaPrecisa.Controllers
{
    public class AccesoController : Controller
    {
        private readonly UsuarioService _usuarioService;

        // Inyectamos el servicio de negocio que creamos
        public AccesoController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Si ya está logueado, lo mandamos directo al inicio
            if (User.Identity!.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string nombreUsuario, string contrasena)
        {
            // Validamos usando la Capa de Negocio
            var usuario = await _usuarioService.ValidarUsuarioAsync(nombreUsuario, contrasena);

            if (usuario == null)
            {
                ViewData["Mensaje"] = "Usuario o contraseña incorrectos, o usuario inactivo.";
                return View();
            }

            // Creamos los "Claims" (la identidad del usuario para la sesión)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Role, usuario.Rol ?? "Vendedor") // Guardamos el rol (Administrador / Vendedor)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Registramos la sesión mediante la cookie de autenticación
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Salir()
        {
            // Cerramos la sesión
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Acceso");
        }
    }
}
