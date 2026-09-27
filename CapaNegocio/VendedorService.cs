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


        // He  copiado todos losmetods de usuario, pero tu vendeodr no los necesita todos, solo necesita 
        // listasrm guardar, editar, eliminar, y buscar por id
        
        
        // Listar todos los usuarios -> ESTE SI SE NECESITA y todo lo de usaurio se lo pasamos como vendedor
        public async Task<List<Vendedor>> ObtenerVendedoresAsync()
        {
            return await _context.Vendedor.ToListAsync();
        } // quedo ok  ESTOS COMENTARIO LUEGO LOS BORRAS

        // Obtener un vendeor por su ID -> SI SE NECESITA
        public async Task<Vendedor?> ObtenerVendedorPorIdAsync(int id)
        {
            return await _context.Vendedor.FindAsync(id);
        } // ok


        // este si 
        public async Task ActualizarVendedorAsync(Vendedor vendedor)
        {
            // 1. Buscamos el usuario actual directamente desde la base de datos
            var vendedorDb = await _context.Vendedor.FindAsync(vendedor.IdVendedor);
            if (vendedorDb == null) return;

            // 2. Actualizamos únicamente los campos que permitimos editar en el formulario
            vendedorDb.Correo = vendedor.Correo; 
            vendedorDb.Estado = vendedor.Estado;
            vendedorDb.Dni = vendedor.Dni;
            // ...
            // .. asi continuas
            // aqui tendrias que poner todos los atribtos de vendedor




            // 3. Guardamos los cambios
            _context.Vendedor.Update(vendedorDb);
            await _context.SaveChangesAsync();
        } //ok

        // Eliminar o dar de baja lógicamente a un usuario  --> ests si
        public async Task EliminarVendedorAsync(int id)
        {
            var vendedor = await _context.Vendedor.FindAsync(id);
            if (vendedor != null)
            {
                vendedor.Estado = false;

                await _context.SaveChangesAsync();
            }
        } // ok


    }
}
