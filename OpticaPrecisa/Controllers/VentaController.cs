using CapaDatos;
using CapaNegocio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace OpticaPrecisa.Controllers
{
    public class VentaController : Controller
    {
        private readonly VentaService _ventaService;
        private readonly ApplicationDbContext _context;

        public VentaController(VentaService ventaService, ApplicationDbContext context)
        {
            _ventaService = ventaService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Clientes = await _context.Set<Cliente>().OrderBy(c => c.Nombre).ToListAsync();
            ViewBag.Vendedores = await _context.Set<Vendedor>().Where(v => v.Estado == true).OrderBy(v => v.Nombre).ToListAsync();
            ViewBag.Productos = await _context.Set<Producto>().Where(p => p.Estado == true && p.Stock > 0).OrderBy(p => p.Nombre).ToListAsync();

            return View();
        }

        public async Task<IActionResult> Historial()
        {
            var ventas = await _ventaService.ObtenerVentasAsync();
            return View(ventas);
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] Venta venta)
        {
            try
            {
                var ok = await _ventaService.RegistrarVentaAsync(venta);
                return Json(new { success = ok, message = "Venta registrada con éxito." });
            }
            catch (Exception ex)
            {
                var errorMsg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = errorMsg });
            }
        }

        public async Task<IActionResult> ReporteGeneral(DateTime? fechaInicio, DateTime? fechaFin)
        {
            var ventasQuery = _context.Set<Venta>()
                .Include(v => v.IdClienteNavigation)
                .Include(v => v.IdVendedorNavigation)
                .AsQueryable();

            if (fechaInicio.HasValue)
            {
                ventasQuery = ventasQuery.Where(v => v.Fecha >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                var finDia = fechaFin.Value.Date.AddDays(1).AddTicks(-1);
                ventasQuery = ventasQuery.Where(v => v.Fecha <= finDia);
            }

            var ventas = await ventasQuery.OrderByDescending(v => v.Fecha).ToListAsync();

            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");

            return View(ventas);
        }

        public async Task<IActionResult> Comprobante(int id)
        {
            var venta = await _ventaService.ObtenerPorIdAsync(id);
            if (venta == null)
            {
                return NotFound();
            }
            return View(venta);
        }
    }
}