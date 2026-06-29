using Embutidos_Vallejos.Models;
using Embutidos_Vallejos.Models.DTOs;
using Embutidos_Vallejos.Models.Entities;

namespace Embutidos_Vallejos.Services;

public interface IClienteService
{
    Task<bool> EmailExistsAsync(string email);
    Task<Cliente> RegisterAsync(RegisterViewModel model);
    Task<Cliente?> GetByIdAsync(int id);
    Task<List<Cliente>> GetAllAsync();
    Task<PerfilDto?> GetProfileAsync(int id);
    Task<bool> UpdateProfileAsync(int id, PerfilUpdateDto dto);
    Task<bool> VerifyPasswordAsync(int id, string password);
    Task<bool> CambiarPasswordAsync(int id, string newPassword);
    Task<List<Cliente>> SearchAsync(string term);
}
