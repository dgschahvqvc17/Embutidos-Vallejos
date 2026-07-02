using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models.Entities;

public class DetalleVenta
{
    public int DetalleVentaId { get; set; }

    public int Cantidad { get; set; }

    [Required]
    public decimal PrecioUnitario { get; set; }

    [Required]
    public decimal Subtotal { get; set; }

    public int VentaId { get; set; }
    public int ProductoId { get; set; }

    public Venta Venta { get; set; } = null!;
    public Producto Producto { get; set; } = null!;
}
