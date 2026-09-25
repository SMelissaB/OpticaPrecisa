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
        public UsuariosController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
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
            if (nuevaPassword != confirmarPassword)
            {
                TempData["Error"] = "Las nuevas contraseñas no coinciden.";
                return RedirectToAction(nameof(Editar), new { id = idUsuario });
            }

            // El servicio valida, hashea, guarda y envía el correo automáticamente
            bool cambiado = await _usuarioService.CambiarContrasenaAsync(idUsuario, passwordActual, nuevaPassword);

            if (!cambiado)
            {
                TempData["Error"] = "La contraseña actual ingresada es incorrecta.";
                return RedirectToAction(nameof(Editar), new { id = idUsuario });
            }

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
