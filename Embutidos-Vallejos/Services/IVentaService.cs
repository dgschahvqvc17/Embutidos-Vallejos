using Embutidos_Vallejos.Models.DTOs;

namespace Embutidos_Vallejos.Services;

public interface IVentaService
{
    Task<List<VentaDto>> GetAllAsync();
    Task<VentaDto?> GetByIdAsync(int id);
    Task<VentaDto> CreateAsync(VentaCreateDto dto);
    Task<List<VentaDto>> GetByEmpleadoAsync(int empleadoId);
    Task<List<VentaDto>> GetByClienteAsync(int clienteId);
    Task<List<VentaDto>> GetVentasDelDiaAsync();
}
