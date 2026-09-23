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

        // Valida las credenciales del usuario para el inicio de sesión.
        public async Task<Usuario?> ValidarUsuarioAsync(string nombreUsuario, string contrasena)
        {
            // Busca el usuario que coincida, esté activo y trae los datos de vendedor si los tuviera
            var usuario = await _context.Usuario
                .Include(u => u.Vendedor)
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Contrasena == contrasena && u.Estado == true);

            return usuario;
        }

        // Listar todos los usuarios
        public async Task<List<Usuario>> ObtenerUsuariosAsync()
        {
            return await _context.Usuario.ToListAsync();
        }

        // Obtener un usuario por su ID
        public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int id)
        {
            return await _context.Usuario.FindAsync(id);
        }

        // Crear un nuevo usuario
        public async Task CrearUsuarioAsync(Usuario usuario)
        {
            usuario.Estado = true; // Por defecto activo
            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();
        }

        // Actualizar usuario existente
        public async Task ActualizarUsuarioAsync(Usuario usuario)
        {
            _context.Usuario.Update(usuario);
            await _context.SaveChangesAsync();
        }

        // Eliminar o dar de baja lógicamente a un usuario
        public async Task EliminarUsuarioAsync(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario != null)
            {
                usuario.Estado = false;

                await _context.SaveChangesAsync();
            }
        }
    }
}
