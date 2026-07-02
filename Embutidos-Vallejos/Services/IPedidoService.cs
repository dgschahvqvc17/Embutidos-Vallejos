using Embutidos_Vallejos.Models.DTOs;

namespace Embutidos_Vallejos.Services;

public interface IPedidoService
{
    Task<List<PedidoDto>> GetAllAsync();
    Task<List<PedidoDto>> GetByClienteAsync(int clienteId);
    Task<List<PedidoDto>> GetByEstadoAsync(string estado);
    Task<PedidoDto?> GetByIdAsync(int id);
    Task<PedidoDto> CreateAsync(int clienteId, PedidoCreateDto dto);
    Task<bool> UpdateEstadoAsync(int id, string estado);
    Task<bool> AsignarRepartidorAsync(int pedidoId, int repartidorId);
    Task<bool> CancelarAsync(int id);
    Task<List<PedidoDto>> GetDisponiblesParaRepartoAsync();
}
