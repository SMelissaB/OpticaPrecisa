using CapaDatos;
using CapaNegocio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OpticaPrecisa.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly UsuarioService _usuarioService;
        private readonly CorreoService _correoService;
        public UsuariosController(UsuarioService usuarioService, CorreoService correoService)
        {
            _usuarioService = usuarioService;
            _correoService = correoService;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var lista = await _usuarioService.ObtenerUsuariosAsync();
            return View(lista);
        }

        // GET: Usuarios/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: Usuarios/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                await _usuarioService.CrearUsuarioAsync(usuario);
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuarios/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var usuario = await _usuarioService.ObtenerUsuarioPorIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // POST: Usuarios/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _usuarioService.ActualizarUsuarioAsync(usuario);
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPassword(int idUsuario, string passwordActual, string nuevaPassword, string confirmarPassword)
        {
            var usuario = await _usuarioService.ObtenerUsuarioPorIdAsync(idUsuario);
            if (usuario == null)
            {
                return NotFound();
            }

            // Validar que la contraseña actual sea correcta
            if (usuario.Contrasena != passwordActual)
            {
                TempData["Error"] = "La contraseña actual ingresada es incorrecta.";
                return RedirectToAction(nameof(Editar), new { id = idUsuario });
            }

            // Validar que las nuevas contraseñas coincidan
            if (nuevaPassword != confirmarPassword)
            {
                TempData["Error"] = "Las nuevas contraseñas no coinciden.";
                return RedirectToAction(nameof(Editar), new { id = idUsuario });
            }

            // Actualizar contraseña
            usuario.Contrasena = nuevaPassword;
            await _usuarioService.ActualizarUsuarioAsync(usuario);

            // Enviar notificación por correo
            string asunto = "Seguridad: Modificación de Contraseña";
            string mensaje = $"Hola {usuario.NombreUsuario},\n\nTe informamos que la contraseña de tu cuenta ha sido modificada exitosamente.\n\nSi no realizaste esta acción, comunícate con el administrador de inmediato.";
            await _correoService.EnviarCorreoAsync(usuario.Correo, asunto, mensaje);

            TempData["Exito"] = "La contraseña se ha actualizado correctamente y se ha enviado un correo de notificación.";
            return RedirectToAction(nameof(Editar), new { id = idUsuario });
        }

        // POST: Usuarios/Eliminar/5
        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            await _usuarioService.EliminarUsuarioAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
