using CapaDatos;
using CapaNegocio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OpticaPrecisa.Controllers
{
    public class VendedorController : Controller
    {
        private readonly VendedorService _vendedorService;
        private readonly UsuarioService _usuarioService;
        public VendedorController(VendedorService vendedorService, UsuarioService usuarioService)
        {
            _vendedorService = vendedorService;
            _usuarioService = usuarioService;
        }
        // GET: Vendedor
        public async Task<IActionResult> Index()
            {
                var lista = await _vendedorService.ObtenerVendedorAsync();
                return View(lista);
            }

        // GET: Vendedor/Crear
        public async Task<IActionResult> Crear()
        {
            await CargarUsuariosVendedoresAsync();
            return View();
        }

        // POST: Vendedor/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Vendedor vendedor)
        {
            if (ModelState.IsValid)
            {
                await _vendedorService.CrearVendedorAsync(vendedor);
                return RedirectToAction(nameof(Index));
            }

            await CargarUsuariosVendedoresAsync(vendedor.IdUsuario);
            return View(vendedor);
        }

        // GET: vendedor/Editar/5
        public async Task<IActionResult> Editar(int id)
            {
                var vendedor = await _vendedorService.ObtenerVendedorPorIdAsync(id);
                if (vendedor == null)
                {
                    return NotFound();
                }
                await CargarUsuariosVendedoresAsync(vendedor.IdUsuario);
                return View(vendedor);
            }

            // POST: Vendedor/Editar/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Editar(int id, Vendedor vendedor)
            {
                if (id != vendedor.IdVendedor)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    await _vendedorService.ActualizarVendedorAsync(vendedor);
                    return RedirectToAction(nameof(Index));
                }
                await CargarUsuariosVendedoresAsync(vendedor.IdUsuario);
                return View(vendedor);
            }

            // POST: Vendedor/Eliminar/5
            [HttpPost]
            public async Task<IActionResult> Eliminar(int id)
            {
                await _vendedorService.EliminarVendedorAsync(id);
                return RedirectToAction(nameof(Index));
            }

            private async Task CargarUsuariosVendedoresAsync(int? idUsuarioSeleccionado = null)
            {
                var usuariosVendedores = await _usuarioService.ObtenerUsuariosVendedoresAsync(idUsuarioSeleccionado);

                
                ViewBag.ListaUsuarios = new SelectList(usuariosVendedores, "IdUsuario", "NombreUsuario", idUsuarioSeleccionado);
            }
        }
    }
