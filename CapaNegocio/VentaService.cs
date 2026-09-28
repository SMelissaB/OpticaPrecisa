using CapaDatos;
using CapaEntidad;

namespace CapaNegocio
{
    public class VentaService
    {
        private readonly VentaDatos _ventaDatos;

        public VentaService(VentaDatos ventaDatos)
        {
            _ventaDatos = ventaDatos;
        }

        public async Task<List<Venta>> ObtenerVentasAsync() => await _ventaDatos.ObtenerVentasAsync();

        public async Task<Venta?> ObtenerPorIdAsync(int id) => await _ventaDatos.ObtenerPorIdAsync(id);

        public async Task<bool> RegistrarVentaAsync(Venta venta)
        {
            if (venta.IdCliente == null || venta.IdCliente <= 0)
                throw new ArgumentException("Debe seleccionar un cliente.");

            if (venta.IdVendedor == null || venta.IdVendedor <= 0)
                throw new ArgumentException("Debe asociar un vendedor.");

            if (venta.DetalleVenta == null || !venta.DetalleVenta.Any())
                throw new ArgumentException("Debe agregar al menos un producto a la venta.");

            venta.Total = venta.DetalleVenta.Sum(d => d.Subtotal);

            return await _ventaDatos.RegistrarVentaAsync(venta);
        }
    }
}