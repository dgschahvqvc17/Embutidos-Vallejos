using Embutidos_Vallejos.Data;
using Embutidos_Vallejos.Models.DTOs;
using Embutidos_Vallejos.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Embutidos_Vallejos.Services;

public class ProductoService : IProductoService
{
    private readonly AppDbContext _db;

    public ProductoService(AppDbContext db) => _db = db;

    public async Task<List<ProductoDto>> GetAllAsync()
    {
        return await _db.Productos
            .Include(p => p.Categoria)
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    public async Task<List<ProductoDto>> GetActivosAsync()
    {
        return await _db.Productos
            .Include(p => p.Categoria)
            .Where(p => p.Estado == "Activo")
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    public async Task<List<ProductoDto>> GetByCategoriaAsync(int categoriaId)
    {
        return await _db.Productos
            .Include(p => p.Categoria)
            .Where(p => p.CategoriaId == categoriaId && p.Estado == "Activo")
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    public async Task<ProductoDto?> GetByIdAsync(int id)
    {
        return await _db.Productos
            .Include(p => p.Categoria)
            .Where(p => p.ProductoId == id)
            .Select(p => MapToDto(p))
            .FirstOrDefaultAsync();
    }

    public async Task<ProductoDto> CreateAsync(ProductoCreateDto dto)
    {
        var producto = new Producto
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            PrecioProduccion = dto.PrecioProduccion,
            PrecioVenta = dto.PrecioVenta,
            Stock = dto.Stock,
            StockMinimo = dto.StockMinimo,
            Imagen = dto.Imagen,
            Estado = dto.Estado,
            CategoriaId = dto.CategoriaId
        };

        _db.Productos.Add(producto);
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(producto.ProductoId))!;
    }

    public async Task<ProductoDto?> UpdateAsync(int id, ProductoUpdateDto dto)
    {
        var producto = await _db.Productos.FindAsync(id);
        if (producto == null) return null;

        producto.Nombre = dto.Nombre;
        producto.Descripcion = dto.Descripcion;
        producto.PrecioProduccion = dto.PrecioProduccion;
        producto.PrecioVenta = dto.PrecioVenta;
        producto.Stock = dto.Stock;
        producto.StockMinimo = dto.StockMinimo;
        producto.Imagen = dto.Imagen;
        producto.Estado = dto.Estado;
        producto.CategoriaId = dto.CategoriaId;

        await _db.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var producto = await _db.Productos.FindAsync(id);
        if (producto == null) return false;

        _db.Productos.Remove(producto);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateStockAsync(int id, int cantidad)
    {
        var producto = await _db.Productos.FindAsync(id);
        if (producto == null) return false;

        producto.Stock += cantidad;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<ProductoDto>> GetStockBajoAsync()
    {
        return await _db.Productos
            .Include(p => p.Categoria)
            .Where(p => p.Stock <= p.StockMinimo && p.Estado == "Activo")
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    private static ProductoDto MapToDto(Producto p) => new()
    {
        ProductoId = p.ProductoId,
        Nombre = p.Nombre,
        Descripcion = p.Descripcion,
        PrecioProduccion = p.PrecioProduccion,
        PrecioVenta = p.PrecioVenta,
        Stock = p.Stock,
        StockMinimo = p.StockMinimo,
        Imagen = p.Imagen,
        Estado = p.Estado,
        CategoriaId = p.CategoriaId,
        NombreCategoria = p.Categoria.Nombre
    };
}
