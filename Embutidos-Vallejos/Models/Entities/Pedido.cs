using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models.Entities;

public class Pedido
{
    public int PedidoId { get; set; }

    public DateTime FechaPedido { get; set; }

    [Required]
    public decimal Total { get; set; }

    [Required, MaxLength(30)]
    public string Estado { get; set; } = null!;

    public string? DireccionEntrega { get; set; }

    public int ClienteId { get; set; }
    public int? RepartidorId { get; set; }

    public Cliente Cliente { get; set; } = null!;
    public Repartidor? Repartidor { get; set; }
    public Pago? Pago { get; set; }
    public Entrega? Entrega { get; set; }
    public ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();
}
