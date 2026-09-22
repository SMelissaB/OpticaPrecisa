using CapaDatos;
using Microsoft.EntityFrameworkCore;

namespace CapaNegocio
{
    public class UsuarioService
    {
        private readonly ApplicationDbContext _context;

        public UsuarioService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Valida las credenciales del usuario para el inicio de sesión.
        /// </summary>
        public async Task<Usuario?> ValidarUsuarioAsync(string nombreUsuario, string contrasena)
        {
            // Busca el usuario que coincida, esté activo y trae los datos de vendedor si los tuviera
            var usuario = await _context.Usuario
                .Include(u => u.Vendedor)
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Contrasena == contrasena && u.Estado == true);

            return usuario;
        }
    }
}
