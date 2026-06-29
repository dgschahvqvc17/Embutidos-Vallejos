using Embutidos_Vallejos.Models.DTOs;

namespace Embutidos_Vallejos.Services;

public interface ICategoriaProductoService
{
    Task<List<CategoriaDto>> GetAllAsync();
    Task<CategoriaDto?> GetByIdAsync(int id);
    Task<CategoriaDto> CreateAsync(CategoriaCreateDto dto);
    Task<CategoriaDto?> UpdateAsync(int id, CategoriaUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
