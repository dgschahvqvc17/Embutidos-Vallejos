using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models.Entities;

public class Pago
{
    public int PagoId { get; set; }

    [Required]
    public decimal Monto { get; set; }

    [Required, MaxLength(50)]
    public string TipoPago { get; set; } = null!;

    public DateTime FechaPago { get; set; }

    [Required, MaxLength(30)]
    public string EstadoPago { get; set; } = null!;

    [MaxLength(100)]
    public string? CodigoTransaccion { get; set; }

    [MaxLength(100)]
    public string? ReferenciaPago { get; set; }

    public int PedidoId { get; set; }

    public Pedido Pedido { get; set; } = null!;
}
