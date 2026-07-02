namespace Embutidos_Vallejos.Models.DTOs;

public class RepartidorDashboardDto
{
    public List<EntregaDto> EntregasPendientes { get; set; } = new();
    public List<EntregaDto> EntregasCompletadas { get; set; } = new();
    public List<PedidoDto> PedidosDisponibles { get; set; } = new();
    public string NombreRepartidor { get; set; } = string.Empty;
    public string? PlacaVehiculo { get; set; }
    public int TotalPendientes => EntregasPendientes.Count;
    public int TotalCompletadas => EntregasCompletadas.Count;
    public int TotalDisponibles => PedidosDisponibles.Count;
}
