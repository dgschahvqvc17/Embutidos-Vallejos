using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models.Entities;

public class Entrega
{
    public int EntregaId { get; set; }

    public DateTime? FechaSalida { get; set; }
    public DateTime? FechaEntrega { get; set; }

    [Required, MaxLength(30)]
    public string EstadoEntrega { get; set; } = null!;

    public string? Observaciones { get; set; }

    public int PedidoId { get; set; }
    public int RepartidorId { get; set; }

    public Pedido Pedido { get; set; } = null!;
    public Repartidor Repartidor { get; set; } = null!;
}
