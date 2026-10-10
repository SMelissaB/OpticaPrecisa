using CapaDatos;
using Microsoft.EntityFrameworkCore;

namespace CapaNegocio
{
    public class ProductoService
    {
        private readonly ApplicationDbContext _context;

        public ProductoService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Listar todos los productos, incluyendo el nombre de su categoría
        public async Task<List<Producto>> ObtenerProductosAsync()
        {
            return await _context.Producto
                .Include(p => p.IdCategoriaNavigation)
                .ToListAsync();
        }

        // Obtener un producto por su ID
        public async Task<Producto?> ObtenerProductoPorIdAsync(int id)
        {
            return await _context.Producto.FindAsync(id);
        }

        // Listado de categorías para usar en los combos (Crear/Editar)
        public async Task<List<Categoria>> ObtenerCategoriasAsync()
        {
            return await _context.Categoria
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        // Crear un nuevo producto
        public async Task CrearProductoAsync(Producto producto)
        {
            producto.Estado = true; // Por defecto activo

            _context.Producto.Add(producto);
            await _context.SaveChangesAsync();
        }

        // Actualizar los datos editables de un producto existente
        public async Task ActualizarProductoAsync(Producto producto)
        {
            var productoDb = await _context.Producto.FindAsync(producto.IdProducto);
            if (productoDb == null) return;

            productoDb.Nombre = producto.Nombre;
            productoDb.Descripcion = producto.Descripcion;
            productoDb.Precio = producto.Precio;
            productoDb.Stock = producto.Stock;
            productoDb.IdCategoria = producto.IdCategoria;
            productoDb.Estado = producto.Estado;

            _context.Producto.Update(productoDb);
            await _context.SaveChangesAsync();
        }

        // Dar de baja lógicamente a un producto
        public async Task EliminarProductoAsync(int id)
        {
            var producto = await _context.Producto.FindAsync(id);
            if (producto != null)
            {
                producto.Estado = false;

                await _context.SaveChangesAsync();
            }
        }
    }
}