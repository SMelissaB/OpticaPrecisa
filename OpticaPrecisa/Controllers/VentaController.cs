using CapaDatos;
using CapaNegocio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

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

        public async Task<IActionResult> ReporteGeneralPdf(DateTime? fechaInicio, DateTime? fechaFin)
        {
            var ventasQuery = _context.Set<Venta>()
                .Include(v => v.IdClienteNavigation)
                .Include(v => v.IdVendedorNavigation)
                .AsQueryable();

            if (fechaInicio.HasValue)
                ventasQuery = ventasQuery.Where(v => v.Fecha >= fechaInicio.Value);

            if (fechaFin.HasValue)
            {
                var finDia = fechaFin.Value.Date.AddDays(1).AddTicks(-1);
                ventasQuery = ventasQuery.Where(v => v.Fecha <= finDia);
            }

            var ventas = await ventasQuery.OrderByDescending(v => v.Fecha).ToListAsync();

            // Generar PDF usando QuestPDF
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Lato"));

                    page.Header().ShowOnce().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("ÓPTICA PRECISA").Bold().FontSize(18).FontColor(Colors.Blue.Medium);
                            col.Item().Text("RUC: 20601234567 | Chiclayo, Perú").FontSize(9).FontColor(Colors.Grey.Medium);
                        });
                        row.ConstantItem(150).AlignRight().Column(col =>
                        {
                            col.Item().Text("REPORTE DE VENTAS").Bold().FontSize(11);
                            col.Item().Text($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8).FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Spacing(10);

                        if (fechaInicio.HasValue || fechaFin.HasValue)
                        {
                            col.Item().Text($"Filtro aplicado - Desde: {fechaInicio?.ToString("dd/MM/yyyy") ?? "Inicio"} | Hasta: {fechaFin?.ToString("dd/MM/yyyy") ?? "Actual"}").Italic().FontSize(9);
                        }

                        col.Item().Table(table =>
                        {
                            // Definición correcta para QuestPDF moderno
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(50); // N° Venta
                                columns.RelativeColumn(2);  // Fecha
                                columns.RelativeColumn(3);  // Cliente
                                columns.RelativeColumn(3);  // Vendedor
                                columns.RelativeColumn(2);  // Total
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Darken3).Padding(5).Text("N°").FontColor(Colors.White).Bold();
                                header.Cell().Background(Colors.Grey.Darken3).Padding(5).Text("Fecha").FontColor(Colors.White).Bold();
                                header.Cell().Background(Colors.Grey.Darken3).Padding(5).Text("Cliente").FontColor(Colors.White).Bold();
                                header.Cell().Background(Colors.Grey.Darken3).Padding(5).Text("Vendedor").FontColor(Colors.White).Bold();
                                header.Cell().Background(Colors.Grey.Darken3).Padding(5).AlignRight().Text("Total (S/)").FontColor(Colors.White).Bold();
                            });

                            foreach (var v in ventas)
                            {
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(v.IdVenta.ToString("D6"));
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(v.Fecha?.ToString("dd/MM/yyyy HH:mm") ?? "-");
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(v.IdClienteNavigation?.Nombre ?? "N/A");
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(v.IdVendedorNavigation?.Nombre ?? "N/A");
                                table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"S/ {v.Total:0.00}");
                            }
                        });

                        decimal totalGeneral = ventas.Sum(x => x.Total);
                        col.Item().AlignRight().PaddingTop(10).Text($"TOTAL RECAUDADO: S/ {totalGeneral:0.00}").Bold().FontSize(12).FontColor(Colors.Green.Darken2);
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                    });
                });
            });

            byte[] pdfBytes = documento.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Reporte_Ventas_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
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