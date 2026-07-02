using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models.Entities;

public class Producto
{
    public int ProductoId { get; set; }

    [Required, MaxLength(150)]
    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    [Required]
    public decimal PrecioProduccion { get; set; }

    [Required]
    public decimal PrecioVenta { get; set; }

    public int Stock { get; set; }

    public int StockMinimo { get; set; }

    [MaxLength(255)]
    public string? Imagen { get; set; }

    [Required, MaxLength(30)]
    public string Estado { get; set; } = null!;

    public int CategoriaId { get; set; }

    public CategoriaProducto Categoria { get; set; } = null!;
    public ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();
    public ICollection<DetalleVenta> DetallesVenta { get; set; } = new List<DetalleVenta>();
}
