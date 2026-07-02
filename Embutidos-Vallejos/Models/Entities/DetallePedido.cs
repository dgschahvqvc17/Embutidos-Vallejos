using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models.Entities;

public class DetallePedido
{
    public int DetalleId { get; set; }

    public int Cantidad { get; set; }

    [Required]
    public decimal PrecioUnitario { get; set; }

    [Required]
    public decimal Subtotal { get; set; }

    public int PedidoId { get; set; }
    public int ProductoId { get; set; }

    public Pedido Pedido { get; set; } = null!;
    public Producto Producto { get; set; } = null!;
}
