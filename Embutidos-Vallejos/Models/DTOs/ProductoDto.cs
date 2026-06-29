using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models.DTOs;

public class ProductoDto
{
    public int ProductoId { get; set; }
    public string Nombre { get; set; } = null!;
    public string? Descripcion { get; set; }
    public decimal PrecioProduccion { get; set; }
    public decimal PrecioVenta { get; set; }
    public int Stock { get; set; }
    public int StockMinimo { get; set; }
    public string? Imagen { get; set; }
    public string Estado { get; set; } = null!;
    public int CategoriaId { get; set; }
    public string NombreCategoria { get; set; } = null!;
    public bool StockBajo => Stock <= StockMinimo;
}

public class ProductoCreateDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(150)]
    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El precio de produccion es requerido")]
    [Range(0.01, 999999.99, ErrorMessage = "Precio invalido")]
    public decimal PrecioProduccion { get; set; }

    [Required(ErrorMessage = "El precio de venta es requerido")]
    [Range(0.01, 999999.99, ErrorMessage = "Precio invalido")]
    public decimal PrecioVenta { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int StockMinimo { get; set; }

    public string? Imagen { get; set; }

    [Required(ErrorMessage = "El estado es requerido")]
    [MaxLength(30)]
    public string Estado { get; set; } = "Activo";

    [Required]
    public int CategoriaId { get; set; }
}

public class ProductoUpdateDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [MaxLength(150)]
    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El precio de produccion es requerido")]
    [Range(0.01, 999999.99)]
    public decimal PrecioProduccion { get; set; }

    [Required(ErrorMessage = "El precio de venta es requerido")]
    [Range(0.01, 999999.99)]
    public decimal PrecioVenta { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int StockMinimo { get; set; }

    public string? Imagen { get; set; }

    [Required(ErrorMessage = "El estado es requerido")]
    [MaxLength(30)]
    public string Estado { get; set; } = null!;

    [Required]
    public int CategoriaId { get; set; }
}
