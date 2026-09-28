using CapaDatos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class VendedorService
    {
        private readonly ApplicationDbContext _context;

        public VendedorService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        // Listar todos los vendedores
        public async Task<List<Vendedor>> ObtenerVendedoresAsync()
        {
            return await _context.Vendedor.ToListAsync();
        } 

        // Obtener un vendeor por su ID 
        public async Task<Vendedor?> ObtenerVendedorPorIdAsync(int id)
        {
            return await _context.Vendedor.FindAsync(id);
        } 


        // Para actualizar el vendedor
        public async Task ActualizarVendedorAsync(Vendedor vendedor)
        {
            // 1. Buscamos el vendedor actual directamente desde la base de datos
            var vendedorDb = await _context.Vendedor.FindAsync(vendedor.IdVendedor);
            if (vendedorDb == null) return;

            // 2. Actualizamos únicamente los campos que permitimos editar en el formulario
            //vendedorDb.IdVendedor = vendedor.IdVendedor;
            //vendedorDb.IdUsuario = vendedor.IdUsuario;
            vendedorDb.Nombre = vendedor.Nombre;
            vendedorDb.Dni = vendedor.Dni;
            vendedorDb.Telefono = vendedor.Telefono;
            vendedorDb.Correo = vendedor.Correo;
            vendedorDb.FechaIngreso = vendedor.FechaIngreso;
            vendedorDb.Estado = vendedor.Estado;           
            
            // 3. Guardamos los cambios
            _context.Vendedor.Update(vendedorDb);
            await _context.SaveChangesAsync();
        } 

        // Eliminar o dar de baja lógicamente a un vendedor
        public async Task EliminarVendedorAsync(int id)
        {
            var vendedor = await _context.Vendedor.FindAsync(id);
            if (vendedor != null)
            {
                vendedor.Estado = false;

                await _context.SaveChangesAsync();
            }
        }


    }
}
