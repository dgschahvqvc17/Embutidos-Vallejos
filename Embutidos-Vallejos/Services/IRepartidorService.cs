using Embutidos_Vallejos.Models.DTOs;

namespace Embutidos_Vallejos.Services;

public interface IRepartidorService
{
    Task<List<RepartidorDto>> GetAllAsync();
    Task<RepartidorDto?> GetByIdAsync(int id);
    Task<RepartidorDto?> GetByEmpleadoIdAsync(int empleadoId);
    Task<List<RepartidorDto>> GetDisponiblesAsync();
    Task<RepartidorDto> CreateAsync(RepartidorCreateDto dto);
    Task<RepartidorDto?> UpdateAsync(int id, RepartidorUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdateEstadoAsync(int id, string estado);
}
