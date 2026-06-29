using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models.DTOs;

public class PagoDto
{
    public int PagoId { get; set; }
    public decimal Monto { get; set; }
    public string TipoPago { get; set; } = null!;
    public DateTime FechaPago { get; set; }
    public string EstadoPago { get; set; } = null!;
    public string? CodigoTransaccion { get; set; }
    public string? ReferenciaPago { get; set; }
    public int PedidoId { get; set; }
}

public class PagoCreateDto
{
    [Required]
    public decimal Monto { get; set; }

    [Required(ErrorMessage = "El tipo de pago es requerido")]
    [MaxLength(50)]
    public string TipoPago { get; set; } = null!;

    [MaxLength(100)]
    public string? CodigoTransaccion { get; set; }

    [MaxLength(100)]
    public string? ReferenciaPago { get; set; }

    [Required]
    public int PedidoId { get; set; }
}

public class PagoReceiptDto
{
    public int PedidoId { get; set; }
    public DateTime FechaPedido { get; set; }
    public DateTime FechaPago { get; set; }
    public decimal Monto { get; set; }
    public string TipoPago { get; set; } = null!;
    public string EstadoPago { get; set; } = null!;
    public string? CodigoTransaccion { get; set; }
    public string? ReferenciaPago { get; set; }
    public string NombreCliente { get; set; } = null!;
    public string? DireccionEntrega { get; set; }
    public List<DetallePedidoDto> Detalles { get; set; } = new();
    public string? CodigoQR { get; set; }
}
