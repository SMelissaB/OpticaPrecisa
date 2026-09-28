using CapaDatos;
using CapaNegocio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OpticaPrecisa.Controllers
{
    public class VendedorController : Controller
    {
        private readonly VendedorService _vendedorService;
        public VendedorController(VendedorService vendedorService)
        {
            _vendedorService = vendedorService;
        }
        // GET: Vendedor
        public async Task<IActionResult> Index()
            {
                var lista = await _vendedorService.ObtenerVendedoresAsync();
                return View(lista);
            }

            

            // GET: vendedor/Editar/5
            public async Task<IActionResult> Editar(int id)
            {
                var vendedor = await _vendedorService.ObtenerVendedorPorIdAsync(id);
                if (vendedor == null)
                {
                    return NotFound();
                }
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
                return View(vendedor);
            }

            // POST: Vendedor/Eliminar/5
            [HttpPost]
            public async Task<IActionResult> Eliminar(int id)
            {
                await _vendedorService.EliminarVendedorAsync(id);
                return RedirectToAction(nameof(Index));
            }
        }
    }
