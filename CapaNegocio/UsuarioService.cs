using CapaDatos;
using Microsoft.EntityFrameworkCore;

namespace CapaNegocio
{
    public class UsuarioService
    {
        private readonly ApplicationDbContext _context;
        private readonly CorreoService _correoService;

        public UsuarioService(ApplicationDbContext context, CorreoService correoService)
        {
            _context = context;
            _correoService = correoService;
        }

        // Valida las credenciales del usuario para el inicio de sesión.
        public async Task<Usuario?> ValidarUsuarioAsync(string nombreUsuario, string contrasena)
        {
            // Buscamos al usuario únicamente por su nombre y si está activo
            var usuario = await _context.Usuario
                .Include(u => u.Vendedor)
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Estado == true);

            if (usuario == null)
            {
                return null;
            }

            // Verificamos si la contraseña ingresada coincide con el hash almacenado
            bool passwordValida = BCrypt.Net.BCrypt.Verify(contrasena, usuario.Contrasena);
            if (!passwordValida)
            {
                return null;
            }

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

        // Crear un nuevo usuario hasheando su contraseña
        public async Task CrearUsuarioAsync(Usuario usuario)
        {
            usuario.Estado = true; // Por defecto activo

            // Encriptamos la contraseña antes de guardarla en la base de datos
            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);

            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarUsuarioAsync(Usuario usuario)
        {
            // 1. Buscamos el usuario actual directamente desde la base de datos
            var usuarioDb = await _context.Usuario.FindAsync(usuario.IdUsuario);
            if (usuarioDb == null) return;

            // 2. Actualizamos únicamente los campos que permitimos editar en el formulario
            usuarioDb.Correo = usuario.Correo;
            usuarioDb.Rol = usuario.Rol;
            usuarioDb.Estado = usuario.Estado;

            if (!string.IsNullOrEmpty(usuario.Contrasena) && usuario.Contrasena != usuarioDb.Contrasena)
            {
                usuarioDb.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);
            }

            // 3. Guardamos los cambios
            _context.Usuario.Update(usuarioDb);
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

        // Busca un usuario activo por su correo electrónico para la recuperación de contraseña.
        public async Task<Usuario?> ObtenerPorCorreoAsync(string correo)
        {
            return await _context.Usuario
                .FirstOrDefaultAsync(u => u.Correo == correo && u.Estado == true);
        }

        // Actualiza la contraseña del usuario en la base de datos.
        public async Task<bool> ActualizarContrasenaAsync(int idUsuario, string nuevaContrasenaPlana)
        {
            var usuario = await _context.Usuario.FindAsync(idUsuario);
            if (usuario == null)
            {
                return false;
            }

            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(nuevaContrasenaPlana);
            _context.Usuario.Update(usuario);

            int resultado = await _context.SaveChangesAsync();
            return resultado > 0;
        }

        public async Task<bool> CambiarContrasenaAsync(int idUsuario, string passwordActual, string nuevaPassword)
        {
            var usuario = await _context.Usuario.FindAsync(idUsuario);
            if (usuario == null) return false;

            // Verificar si la contraseña actual coincide con el hash en BD
            bool esValida = BCrypt.Net.BCrypt.Verify(passwordActual, usuario.Contrasena);
            if (!esValida) return false;

            // Hashear y guardar la nueva contraseña
            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(nuevaPassword);
            _context.Usuario.Update(usuario);
            await _context.SaveChangesAsync();

            // Enviar notificación por correo desde el servicio
            string asunto = "Seguridad: Modificación de Contraseña";
            string mensaje = $"Hola {usuario.NombreUsuario},\n\nTe informamos que la contraseña de tu cuenta ha sido modificada exitosamente.\n\nSi no realizaste esta acción, comunícate con el administrador de inmediato.";

            await _correoService.EnviarCorreoAsync(usuario.Correo, asunto, mensaje);

            return true;
        }

    }
}
