using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapaEntidad;
using Microsoft.EntityFrameworkCore;

namespace CapaDatos
{
    public class VentaDatos
    {
        private readonly ApplicationDbContext _context;

        public VentaDatos(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Venta>> ObtenerVentasAsync()
        {
            return await _context.Set<Venta>()
                .Include(v => v.IdClienteNavigation)
                .Include(v => v.IdVendedorNavigation)
                .Include(v => v.DetalleVenta)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();
        }

        public async Task<Venta?> ObtenerPorIdAsync(int idVenta)
        {
            return await _context.Set<Venta>()
                .Include(v => v.IdClienteNavigation)
                .Include(v => v.IdVendedorNavigation)
                .Include(v => v.DetalleVenta)
                .FirstOrDefaultAsync(v => v.IdVenta == idVenta);
        }

        public async Task<bool> RegistrarVentaAsync(Venta venta)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Validar y descontar stock de cada producto
                foreach (var item in venta.DetalleVenta)
                {
                    var producto = await _context.Set<Producto>().FindAsync(item.IdProducto);
                    if (producto == null)
                        throw new Exception($"El producto con ID {item.IdProducto} no existe.");

                    if (producto.Stock < item.Cantidad)
                        throw new Exception($"Stock insuficiente para '{producto.Nombre}'. Disponible: {producto.Stock}");

                    producto.Stock -= item.Cantidad;
                }

                // 2. Preparar el objeto Venta limpio
                var nuevaVenta = new Venta
                {
                    Fecha = DateTime.Now,
                    IdCliente = venta.IdCliente,
                    IdVendedor = venta.IdVendedor,
                    Total = venta.Total
                };

                // Asignar los detalles únicamente con sus datos numéricos
                foreach (var item in venta.DetalleVenta)
                {
                    nuevaVenta.DetalleVenta.Add(new DetalleVenta
                    {
                        IdProducto = item.IdProducto,
                        Cantidad = item.Cantidad,
                        Subtotal = item.Subtotal
                    });
                }

                // 3. Guardar la venta en la base de datos
                await _context.Set<Venta>().AddAsync(nuevaVenta);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}