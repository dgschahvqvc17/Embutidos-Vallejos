using Embutidos_Vallejos.Models.DTOs;

namespace Embutidos_Vallejos.Services;

public interface IProductoService
{
    Task<List<ProductoDto>> GetAllAsync();
    Task<List<ProductoDto>> GetActivosAsync();
    Task<List<ProductoDto>> GetByCategoriaAsync(int categoriaId);
    Task<ProductoDto?> GetByIdAsync(int id);
    Task<ProductoDto> CreateAsync(ProductoCreateDto dto);
    Task<ProductoDto?> UpdateAsync(int id, ProductoUpdateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdateStockAsync(int id, int cantidad);
    Task<List<ProductoDto>> GetStockBajoAsync();
}
