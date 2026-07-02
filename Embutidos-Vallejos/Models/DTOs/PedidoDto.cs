using System.ComponentModel.DataAnnotations;

namespace Embutidos_Vallejos.Models.DTOs;

public class PedidoDto
{
    public int PedidoId { get; set; }
    public DateTime FechaPedido { get; set; }
    public decimal Total { get; set; }
    public string Estado { get; set; } = null!;
    public string? DireccionEntrega { get; set; }
    public int ClienteId { get; set; }
    public string? NombreCliente { get; set; }
    public int? RepartidorId { get; set; }
    public string? NombreRepartidor { get; set; }
    public List<DetallePedidoDto> Detalles { get; set; } = new();
    public PagoDto? Pago { get; set; }
    public EntregaDto? Entrega { get; set; }
}

public class DetallePedidoDto
{
    public int DetalleId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = null!;
    public string? ImagenProducto { get; set; }
}

public class PedidoCreateDto
{
    public string? DireccionEntrega { get; set; }
    public string TipoPago { get; set; } = null!;
    public List<CarritoItemDto> Items { get; set; } = new();
}

public class CarritoItemDto
{
    public int ProductoId { get; set; }
    public string NombreProducto { get; set; } = string.Empty;
    public string? ImagenProducto { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
