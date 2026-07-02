using Embutidos_Vallejos.Data;
using Embutidos_Vallejos.Models.DTOs;
using Embutidos_Vallejos.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Embutidos_Vallejos.Services;

public class CategoriaProductoService : ICategoriaProductoService
{
    private readonly AppDbContext _db;

    public CategoriaProductoService(AppDbContext db) => _db = db;

    public async Task<List<CategoriaDto>> GetAllAsync()
    {
        return await _db.CategoriasProducto
            .Select(c => new CategoriaDto
            {
                CategoriaId = c.CategoriaId,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                CantidadProductos = c.Productos.Count
            })
            .ToListAsync();
    }

    public async Task<CategoriaDto?> GetByIdAsync(int id)
    {
        return await _db.CategoriasProducto
            .Where(c => c.CategoriaId == id)
            .Select(c => new CategoriaDto
            {
                CategoriaId = c.CategoriaId,
                Nombre = c.Nombre,
                Descripcion = c.Descripcion,
                CantidadProductos = c.Productos.Count
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CategoriaDto> CreateAsync(CategoriaCreateDto dto)
    {
        var categoria = new CategoriaProducto
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion
        };

        _db.CategoriasProducto.Add(categoria);
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(categoria.CategoriaId))!;
    }

    public async Task<CategoriaDto?> UpdateAsync(int id, CategoriaUpdateDto dto)
    {
        var categoria = await _db.CategoriasProducto.FindAsync(id);
        if (categoria == null) return null;

        categoria.Nombre = dto.Nombre;
        categoria.Descripcion = dto.Descripcion;

        await _db.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var categoria = await _db.CategoriasProducto
            .Include(c => c.Productos)
            .FirstOrDefaultAsync(c => c.CategoriaId == id);

        if (categoria == null) return false;
        if (categoria.Productos.Any()) return false;

        _db.CategoriasProducto.Remove(categoria);
        await _db.SaveChangesAsync();
        return true;
    }
}
