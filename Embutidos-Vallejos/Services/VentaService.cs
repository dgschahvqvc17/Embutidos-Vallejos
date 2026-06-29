using Embutidos_Vallejos.Data;
using Embutidos_Vallejos.Models.DTOs;
using Embutidos_Vallejos.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Embutidos_Vallejos.Services;

public class VentaService : IVentaService
{
    private readonly AppDbContext _db;

    public VentaService(AppDbContext db) => _db = db;

    public async Task<List<VentaDto>> GetAllAsync()
    {
        return await _db.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Empleado)
            .Include(v => v.DetallesVenta).ThenInclude(d => d.Producto)
            .OrderByDescending(v => v.FechaVenta)
            .Select(v => MapToDto(v))
            .ToListAsync();
    }

    public async Task<VentaDto?> GetByIdAsync(int id)
    {
        return await _db.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Empleado)
            .Include(v => v.DetallesVenta).ThenInclude(d => d.Producto)
            .Where(v => v.VentaId == id)
            .Select(v => MapToDto(v))
            .FirstOrDefaultAsync();
    }

    public async Task<VentaDto> CreateAsync(VentaCreateDto dto)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var venta = new Venta
            {
                FechaVenta = DateTime.Now,
                Total = dto.Total,
                MetodoPago = dto.MetodoPago,
                Estado = "Completada",
                ClienteId = dto.ClienteId,
                EmpleadoId = dto.EmpleadoId
            };

            _db.Ventas.Add(venta);
            await _db.SaveChangesAsync();

            foreach (var detalleDto in dto.Detalles)
            {
                var producto = await _db.Productos.FindAsync(detalleDto.ProductoId);
                if (producto != null)
                    producto.Stock -= detalleDto.Cantidad;

                _db.DetallesVenta.Add(new DetalleVenta
                {
                    Cantidad = detalleDto.Cantidad,
                    PrecioUnitario = detalleDto.PrecioUnitario,
                    Subtotal = detalleDto.Cantidad * detalleDto.PrecioUnitario,
                    VentaId = venta.VentaId,
                    ProductoId = detalleDto.ProductoId
                });
            }

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return (await GetByIdAsync(venta.VentaId))!;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<VentaDto>> GetByEmpleadoAsync(int empleadoId)
    {
        return await _db.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Empleado)
            .Include(v => v.DetallesVenta).ThenInclude(d => d.Producto)
            .Where(v => v.EmpleadoId == empleadoId)
            .OrderByDescending(v => v.FechaVenta)
            .Select(v => MapToDto(v))
            .ToListAsync();
    }

    public async Task<List<VentaDto>> GetByClienteAsync(int clienteId)
    {
        return await _db.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Empleado)
            .Include(v => v.DetallesVenta).ThenInclude(d => d.Producto)
            .Where(v => v.ClienteId == clienteId)
            .OrderByDescending(v => v.FechaVenta)
            .Select(v => MapToDto(v))
            .ToListAsync();
    }

    public async Task<List<VentaDto>> GetVentasDelDiaAsync()
    {
        var hoy = DateTime.Today;
        return await _db.Ventas
            .Include(v => v.Cliente)
            .Include(v => v.Empleado)
            .Include(v => v.DetallesVenta).ThenInclude(d => d.Producto)
            .Where(v => v.FechaVenta >= hoy)
            .OrderByDescending(v => v.FechaVenta)
            .Select(v => MapToDto(v))
            .ToListAsync();
    }

    private static VentaDto MapToDto(Venta v) => new()
    {
        VentaId = v.VentaId,
        FechaVenta = v.FechaVenta,
        Total = v.Total,
        MetodoPago = v.MetodoPago,
        Estado = v.Estado,
        ClienteId = v.ClienteId,
        NombreCliente = $"{v.Cliente.Nombre} {v.Cliente.Apellido}",
        EmpleadoId = v.EmpleadoId,
        NombreEmpleado = $"{v.Empleado.Nombre} {v.Empleado.Apellido}",
        Detalles = v.DetallesVenta.Select(d => new DetalleVentaDto
        {
            DetalleVentaId = d.DetalleVentaId,
            Cantidad = d.Cantidad,
            PrecioUnitario = d.PrecioUnitario,
            Subtotal = d.Subtotal,
            ProductoId = d.ProductoId,
            NombreProducto = d.Producto.Nombre,
            ImagenProducto = d.Producto.Imagen
        }).ToList()
    };
}
