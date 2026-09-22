using System;
using System.Collections.Generic;

namespace CapaDatos;

public partial class Venta
{
    public int IdVenta { get; set; }

    public DateTime? Fecha { get; set; }

    public int? IdCliente { get; set; }

    public int? IdVendedor { get; set; }

    public decimal Total { get; set; }

    public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();

    public virtual Cliente? IdClienteNavigation { get; set; }

    public virtual Vendedor? IdVendedorNavigation { get; set; }
}
