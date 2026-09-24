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
        private readonly CorreoService _correoService;

        // Inyectamos el servicio de negocio que creamos
        public AccesoController(UsuarioService usuarioService, CorreoService correoService)
        {
            _usuarioService = usuarioService;
            _correoService = correoService;
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

        [HttpGet]
        public IActionResult OlvideContrasena()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarOlvideContrasena(string correo)
        {
            var usuario = await _usuarioService.ObtenerPorCorreoAsync(correo);
            if (usuario == null)
            {
                ViewData["Error"] = "El correo ingresado no se encuentra registrado en el sistema.";
                return View("OlvideContrasena");
            }

            string nuevaContrasena = Guid.NewGuid().ToString().Substring(0, 8);
            bool actualizado = await _usuarioService.ActualizarContrasenaAsync(usuario.IdUsuario, nuevaContrasena);

            if (!actualizado)
            {
                ViewData["Error"] = "Ocurrió un error al actualizar la contraseña. Inténtalo de nuevo.";
                return View("OlvideContrasena");
            }

            string asunto = "Recuperación de Contraseña - OpticaPrecisa";
            string cuerpo = $"Hola {usuario.NombreUsuario},\n\nHas solicitado restablecer tu contraseña. Tu nueva contraseña temporal es: {nuevaContrasena}\n\nTe recomendamos iniciar sesión y cambiarla.";

            bool correoEnviado = await _correoService.EnviarCorreoAsync(correo, asunto, cuerpo);

            if (correoEnviado)
            {
                ViewData["Mensaje"] = "Se ha enviado una nueva contraseña temporal a tu correo electrónico.";
            }
            else
            {
                ViewData["Error"] = "La contraseña se actualizó, pero no se pudo enviar el correo electrónico.";
            }

            return View("OlvideContrasena");
        }

    }
}
