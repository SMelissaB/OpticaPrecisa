using CapaDatos;
using CapaNegocio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OpticaPrecisa.Controllers
{
    [Authorize]
    public class ProductosController : Controller
    {
        private readonly ProductoService _productoService;

        public ProductosController(ProductoService productoService)
        {
            _productoService = productoService;
        }

        // GET: Productos
        public async Task<IActionResult> Index()
        {
            var lista = await _productoService.ObtenerProductosAsync();
            return View(lista);
        }

        // GET: Productos/Crear
        public async Task<IActionResult> Crear()
        {
            await CargarCategoriasAsync();
            return View();
        }

        // POST: Productos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Producto producto)
        {
            if (ModelState.IsValid)
            {
                await _productoService.CrearProductoAsync(producto);
                return RedirectToAction(nameof(Index));
            }

            await CargarCategoriasAsync(producto.IdCategoria);
            return View(producto);
        }

        // GET: Productos/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var producto = await _productoService.ObtenerProductoPorIdAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            await CargarCategoriasAsync(producto.IdCategoria);
            return View(producto);
        }

        // POST: Productos/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Producto producto)
        {
            if (id != producto.IdProducto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _productoService.ActualizarProductoAsync(producto);
                return RedirectToAction(nameof(Index));
            }

            await CargarCategoriasAsync(producto.IdCategoria);
            return View(producto);
        }

        // POST: Productos/Eliminar/5
        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            await _productoService.EliminarProductoAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Carga el listado de categorías para usarlo en el <select> de las vistas Crear/Editar
        private async Task CargarCategoriasAsync(int? idCategoriaSeleccionada = null)
        {
            var categorias = await _productoService.ObtenerCategoriasAsync();
            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "Nombre", idCategoriaSeleccionada);
        }
    }
}