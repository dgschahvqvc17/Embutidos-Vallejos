using Embutidos_Vallejos.Models.DTOs;

namespace Embutidos_Vallejos.Services;

public interface IEntregaService
{
    Task<EntregaDto?> GetByPedidoIdAsync(int pedidoId);
    Task<List<EntregaDto>> GetByRepartidorAsync(int repartidorId);
    Task<List<EntregaDto>> GetPendientesByRepartidorAsync(int repartidorId);
    Task<List<EntregaDto>> GetCompletadasAsync();
    Task<EntregaDto> CreateAsync(int pedidoId, int repartidorId);
    Task<bool> IniciarEntregaAsync(int entregaId);
    Task<bool> CompletarEntregaAsync(int entregaId, string? observaciones);
    Task<List<EntregaDto>> GetPendientesAsync();
}
