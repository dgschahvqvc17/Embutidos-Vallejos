namespace Embutidos_Vallejos.Models.DTOs;

public class EntregaDto
{
    public int EntregaId { get; set; }
    public DateTime? FechaSalida { get; set; }
    public DateTime? FechaEntrega { get; set; }
    public string EstadoEntrega { get; set; } = null!;
    public string? Observaciones { get; set; }
    public int PedidoId { get; set; }
    public int RepartidorId { get; set; }
    public string? NombreRepartidor { get; set; }
    public string? PlacaVehiculo { get; set; }
}

public class EntregaUpdateDto
{
    public string? Observaciones { get; set; }
}
