using CapaDatos;
using CapaNegocio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OpticaPrecisa.Models;

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

        // GET: Productos/Detalle/5
        public async Task<IActionResult> Detalle(int id)
        {
            var producto = await _productoService.ObtenerProductoPorIdAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Crear
        public async Task<IActionResult> Crear()
        {
            await CargarCategoriasAsync();
            return View(new ProductoViewModel());
        }

        // POST: Productos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(ProductoViewModel vm)
        {
            await ValidarReglasAsync(vm);

            if (!ModelState.IsValid)
            {
                await CargarCategoriasAsync(vm.IdCategoria);
                return View(vm);
            }

            var producto = new Producto
            {
                Nombre = vm.Nombre.Trim(),
                Descripcion = vm.Descripcion?.Trim(),
                IdCategoria = vm.IdCategoria,
                Precio = vm.Precio!.Value,
                Stock = vm.Stock!.Value
            };

            await _productoService.CrearProductoAsync(producto);

            TempData["Ok"] = "Producto registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Productos/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var producto = await _productoService.ObtenerProductoPorIdAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            var vm = new ProductoViewModel
            {
                IdProducto = producto.IdProducto,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                IdCategoria = producto.IdCategoria,
                Precio = producto.Precio,
                Stock = producto.Stock,
                Estado = producto.Estado ?? true
            };

            await CargarCategoriasAsync(producto.IdCategoria);
            return View(vm);
        }

        // POST: Productos/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, ProductoViewModel vm)
        {
            if (id != vm.IdProducto)
            {
                return NotFound();
            }

            await ValidarReglasAsync(vm);

            if (!ModelState.IsValid)
            {
                await CargarCategoriasAsync(vm.IdCategoria);
                return View(vm);
            }

            var producto = new Producto
            {
                IdProducto = vm.IdProducto,
                Nombre = vm.Nombre.Trim(),
                Descripcion = vm.Descripcion?.Trim(),
                IdCategoria = vm.IdCategoria,
                Precio = vm.Precio!.Value,
                Stock = vm.Stock!.Value,
                Estado = vm.Estado
            };

            await _productoService.ActualizarProductoAsync(producto);

            TempData["Ok"] = "Producto actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Productos/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            await _productoService.EliminarProductoAsync(id);

            TempData["Ok"] = "Producto dado de baja correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // Reglas que no se pueden expresar con atributos (nombre repetido, categoría válida)
        private async Task ValidarReglasAsync(ProductoViewModel vm)
        {
            if (!string.IsNullOrWhiteSpace(vm.Nombre) &&
                await _productoService.ExisteNombreAsync(vm.Nombre.Trim(), vm.IdProducto == 0 ? null : vm.IdProducto))
            {
                ModelState.AddModelError(nameof(vm.Nombre), "Ya existe un producto con ese nombre.");
            }

            if (vm.IdCategoria.HasValue && !await _productoService.ExisteCategoriaAsync(vm.IdCategoria.Value))
            {
                ModelState.AddModelError(nameof(vm.IdCategoria), "La categoría seleccionada no existe.");
            }
        }

        // Carga el listado de categorías para usarlo en el <select> de las vistas Crear/Editar
        private async Task CargarCategoriasAsync(int? idCategoriaSeleccionada = null)
        {
            var categorias = await _productoService.ObtenerCategoriasAsync();
            ViewBag.Categorias = new SelectList(categorias, "IdCategoria", "Nombre", idCategoriaSeleccionada);
        }
    }
}