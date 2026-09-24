using System;
using System.Collections.Generic;

namespace CapaDatos;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Contrasena { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Rol { get; set; } = null!;

    public bool? Estado { get; set; }

    public virtual Vendedor? Vendedor { get; set; }
}
