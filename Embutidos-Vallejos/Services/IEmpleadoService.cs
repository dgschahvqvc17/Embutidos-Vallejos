using Embutidos_Vallejos.Models.DTOs;
using Embutidos_Vallejos.Models.Entities;

namespace Embutidos_Vallejos.Services;

public interface IEmpleadoService
{
    Task<List<EmpleadoDto>> GetAllAsync();
    Task<List<EmpleadoDto>> GetByRolIdAsync(int rolId);
    Task<List<Rol>> GetAllRolesAsync();
    Task<EmpleadoDto?> GetByIdAsync(int id);
    Task<EmpleadoDto> CreateAsync(EmpleadoCreateDto dto);
    Task<EmpleadoDto?> UpdateAsync(int id, EmpleadoUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> EmailExistsAsync(string email);
    Task<PerfilDto?> GetProfileAsync(int id);
    Task<bool> UpdateProfileAsync(int id, PerfilUpdateDto dto);
    Task<bool> VerifyPasswordAsync(int id, string password);
    Task<bool> CambiarPasswordAsync(int id, string newPassword);
}
