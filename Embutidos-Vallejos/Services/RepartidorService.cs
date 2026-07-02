using Embutidos_Vallejos.Data;
using Embutidos_Vallejos.Models.DTOs;
using Embutidos_Vallejos.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Embutidos_Vallejos.Services;

public class RepartidorService : IRepartidorService
{
    private readonly AppDbContext _db;

    public RepartidorService(AppDbContext db) => _db = db;

    public async Task<List<RepartidorDto>> GetAllAsync()
    {
        return await _db.Repartidores
            .Select(r => new RepartidorDto
            {
                RepartidorId = r.RepartidorId,
                Nombre = r.Nombre,
                Apellido = r.Apellido,
                Telefono = r.Telefono,
                PlacaVehiculo = r.PlacaVehiculo,
                Estado = r.Estado,
                Email = r.Empleado != null ? r.Empleado.Email : null,
                PedidosAsignados = r.Pedidos.Count(p => p.Estado != "Entregado" && p.Estado != "Cancelado")
            })
            .ToListAsync();
    }

    public async Task<RepartidorDto?> GetByIdAsync(int id)
    {
        return await _db.Repartidores
            .Where(r => r.RepartidorId == id)
            .Select(r => new RepartidorDto
            {
                RepartidorId = r.RepartidorId,
                Nombre = r.Nombre,
                Apellido = r.Apellido,
                Telefono = r.Telefono,
                PlacaVehiculo = r.PlacaVehiculo,
                Estado = r.Estado,
                Email = r.Empleado != null ? r.Empleado.Email : null,
                PedidosAsignados = r.Pedidos.Count(p => p.Estado != "Entregado" && p.Estado != "Cancelado")
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<RepartidorDto>> GetDisponiblesAsync()
    {
        return await _db.Repartidores
            .Where(r => r.Estado == "Disponible")
            .Select(r => new RepartidorDto
            {
                RepartidorId = r.RepartidorId,
                Nombre = r.Nombre,
                Apellido = r.Apellido,
                Telefono = r.Telefono,
                PlacaVehiculo = r.PlacaVehiculo,
                Estado = r.Estado
            })
            .ToListAsync();
    }

    public async Task<RepartidorDto> CreateAsync(RepartidorCreateDto dto)
    {
        var empleado = new Empleado
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Telefono = dto.Telefono,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FechaContratacion = DateTime.Today,
            Estado = "Activo",
            RolId = 5
        };

        _db.Empleados.Add(empleado);
        await _db.SaveChangesAsync();

        var repartidor = new Repartidor
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Telefono = dto.Telefono,
            PlacaVehiculo = dto.PlacaVehiculo,
            Estado = dto.Estado,
            EmpleadoId = empleado.EmpleadoId
        };

        _db.Repartidores.Add(repartidor);
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(repartidor.RepartidorId))!;
    }

    public async Task<RepartidorDto?> UpdateAsync(int id, RepartidorUpdateDto dto)
    {
        var repartidor = await _db.Repartidores
            .Include(r => r.Empleado)
            .FirstOrDefaultAsync(r => r.RepartidorId == id);
        if (repartidor == null) return null;

        repartidor.Nombre = dto.Nombre;
        repartidor.Apellido = dto.Apellido;
        repartidor.Telefono = dto.Telefono;
        repartidor.PlacaVehiculo = dto.PlacaVehiculo;
        repartidor.Estado = dto.Estado;

        if (repartidor.Empleado != null)
        {
            repartidor.Empleado.Nombre = dto.Nombre;
            repartidor.Empleado.Apellido = dto.Apellido;
            repartidor.Empleado.Telefono = dto.Telefono;
            repartidor.Empleado.Email = dto.Email;
            repartidor.Empleado.Estado = dto.Estado == "Disponible" ? "Activo" : "Inactivo";
        }

        await _db.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var repartidor = await _db.Repartidores
            .Include(r => r.Pedidos)
            .Include(r => r.Entregas)
            .Include(r => r.Empleado)
            .FirstOrDefaultAsync(r => r.RepartidorId == id);

        if (repartidor == null) return false;
        if (repartidor.Pedidos.Any(p => p.Estado != "Entregado" && p.Estado != "Cancelado"))
            return false;

        if (repartidor.Empleado != null)
            _db.Empleados.Remove(repartidor.Empleado);

        _db.Repartidores.Remove(repartidor);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateEstadoAsync(int id, string estado)
    {
        var repartidor = await _db.Repartidores.FindAsync(id);
        if (repartidor == null) return false;

        repartidor.Estado = estado;
        await _db.SaveChangesAsync();
        return true;
    }
}
