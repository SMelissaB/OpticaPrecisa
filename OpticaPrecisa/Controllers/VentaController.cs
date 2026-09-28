using CapaDatos;
using CapaEntidad;
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
    }
}