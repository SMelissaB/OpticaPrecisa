using System.ComponentModel.DataAnnotations;

namespace OpticaPrecisa.Models
{
    public class ProductoViewModel
    {
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        [Display(Name = "Nombre del producto")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecciona una categoría.")]
        [Display(Name = "Categoría")]
        public int? IdCategoria { get; set; }

        [StringLength(200, ErrorMessage = "La descripción no puede superar los 200 caracteres.")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, 99999999.99, ErrorMessage = "El precio debe ser mayor que 0.")]
        public decimal? Precio { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, 100000, ErrorMessage = "El stock debe estar entre 0 y 100 000.")]
        public int? Stock { get; set; }

        public bool Estado { get; set; } = true;
    }
}