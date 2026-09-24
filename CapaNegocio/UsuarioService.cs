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

        /// Valida las credenciales del usuario para el inicio de sesión.
        public async Task<Usuario?> ValidarUsuarioAsync(string nombreUsuario, string contrasena)
        {
            // Busca el usuario que coincida, esté activo y trae los datos de vendedor si los tuviera
            var usuario = await _context.Usuario
                .Include(u => u.Vendedor)
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Contrasena == contrasena && u.Estado == true);

            return usuario;
        }


        // Busca un usuario activo por su correo electrónico para la recuperación de contraseña.
        public async Task<Usuario?> ObtenerPorCorreoAsync(string correo)
        {
            return await _context.Usuario
                .FirstOrDefaultAsync(u => u.Correo == correo && u.Estado == true);
        }

        // Actualiza la contraseña del usuario en la base de datos.
        public async Task<bool> ActualizarContrasenaAsync(int idUsuario, string nuevaContrasena)
        {
            var usuario = await _context.Usuario.FindAsync(idUsuario);
            if (usuario == null)
            {
                return false;
            }

            usuario.Contrasena = nuevaContrasena;
            _context.Usuario.Update(usuario);

            int resultado = await _context.SaveChangesAsync();
            return resultado > 0;
        }

    }
}
