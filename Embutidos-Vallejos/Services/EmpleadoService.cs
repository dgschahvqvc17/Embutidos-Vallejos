using Embutidos_Vallejos.Data;
using Embutidos_Vallejos.Models.DTOs;
using Embutidos_Vallejos.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Embutidos_Vallejos.Services;

public class EmpleadoService : IEmpleadoService
{
    private readonly AppDbContext _db;

    public EmpleadoService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<EmpleadoDto>> GetAllAsync()
    {
        return await _db.Empleados
            .Include(e => e.Rol)
            .Select(e => new EmpleadoDto
            {
                EmpleadoId = e.EmpleadoId,
                Nombre = e.Nombre,
                Apellido = e.Apellido,
                Telefono = e.Telefono,
                Email = e.Email,
                FechaContratacion = e.FechaContratacion,
                Estado = e.Estado,
                NombreRol = e.Rol.NombreRol,
                PlacaVehiculo = e.Repartidor != null ? e.Repartidor.PlacaVehiculo : null
            })
            .ToListAsync();
    }

    public async Task<EmpleadoDto?> GetByIdAsync(int id)
    {
        return await _db.Empleados
            .Include(e => e.Rol)
            .Include(e => e.Repartidor)
            .Where(e => e.EmpleadoId == id)
            .Select(e => new EmpleadoDto
            {
                EmpleadoId = e.EmpleadoId,
                Nombre = e.Nombre,
                Apellido = e.Apellido,
                Telefono = e.Telefono,
                Email = e.Email,
                FechaContratacion = e.FechaContratacion,
                Estado = e.Estado,
                NombreRol = e.Rol.NombreRol,
                PlacaVehiculo = e.Repartidor != null ? e.Repartidor.PlacaVehiculo : null
            })
            .FirstOrDefaultAsync();
    }

    public async Task<EmpleadoDto> CreateAsync(EmpleadoCreateDto dto)
    {
        var empleado = new Empleado
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Telefono = dto.Telefono,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FechaContratacion = dto.FechaContratacion,
            Estado = dto.Estado,
            RolId = dto.RolId
        };

        _db.Empleados.Add(empleado);
        await _db.SaveChangesAsync();

        if (dto.RolId == 5)
        {
            var repartidor = new Repartidor
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Telefono = dto.Telefono,
                PlacaVehiculo = dto.PlacaVehiculo,
                Estado = dto.Estado == "Activo" ? "Disponible" : "Ocupado",
                EmpleadoId = empleado.EmpleadoId
            };
            _db.Repartidores.Add(repartidor);
            await _db.SaveChangesAsync();
        }

        return (await GetByIdAsync(empleado.EmpleadoId))!;
    }

    public async Task<EmpleadoDto?> UpdateAsync(int id, EmpleadoUpdateDto dto)
    {
        var empleado = await _db.Empleados
            .Include(e => e.Repartidor)
            .FirstOrDefaultAsync(e => e.EmpleadoId == id);
        if (empleado == null) return null;

        empleado.Nombre = dto.Nombre;
        empleado.Apellido = dto.Apellido;
        empleado.Telefono = dto.Telefono;
        empleado.Email = dto.Email;
        empleado.FechaContratacion = dto.FechaContratacion;
        empleado.Estado = dto.Estado;
        empleado.RolId = dto.RolId;

        if (dto.RolId == 5)
        {
            if (empleado.Repartidor != null)
            {
                empleado.Repartidor.Nombre = dto.Nombre;
                empleado.Repartidor.Apellido = dto.Apellido;
                empleado.Repartidor.Telefono = dto.Telefono;
                empleado.Repartidor.PlacaVehiculo = dto.PlacaVehiculo ?? empleado.Repartidor.PlacaVehiculo;
                empleado.Repartidor.Estado = dto.Estado == "Activo" ? "Disponible" : "Ocupado";
            }
            else
            {
                var repartidor = new Repartidor
                {
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Telefono = dto.Telefono,
                    PlacaVehiculo = dto.PlacaVehiculo,
                    Estado = dto.Estado == "Activo" ? "Disponible" : "Ocupado",
                    EmpleadoId = empleado.EmpleadoId
                };
                _db.Repartidores.Add(repartidor);
            }
        }
        else if (empleado.Repartidor != null)
        {
            _db.Repartidores.Remove(empleado.Repartidor);
        }

        await _db.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var empleado = await _db.Empleados
            .Include(e => e.Repartidor)
            .FirstOrDefaultAsync(e => e.EmpleadoId == id);
        if (empleado == null) return false;

        if (empleado.Repartidor != null)
            _db.Repartidores.Remove(empleado.Repartidor);

        _db.Empleados.Remove(empleado);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _db.Empleados.AnyAsync(e => e.Email == email);
    }

    public async Task<PerfilDto?> GetProfileAsync(int id)
    {
        var e = await _db.Empleados.Include(x => x.Rol).FirstOrDefaultAsync(x => x.EmpleadoId == id);
        if (e == null) return null;

        return new PerfilDto
        {
            Id = e.EmpleadoId,
            Nombre = e.Nombre,
            Apellido = e.Apellido,
            Telefono = e.Telefono,
            Email = e.Email,
            UserType = "Empleado",
            RolNombre = e.Rol.NombreRol
        };
    }

    public async Task<bool> UpdateProfileAsync(int id, PerfilUpdateDto dto)
    {
        var e = await _db.Empleados.FindAsync(id);
        if (e == null) return false;

        e.Nombre = dto.Nombre;
        e.Apellido = dto.Apellido;
        e.Telefono = dto.Telefono;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> VerifyPasswordAsync(int id, string password)
    {
        var e = await _db.Empleados.FindAsync(id);
        if (e == null) return false;
        return BCrypt.Net.BCrypt.Verify(password, e.Password);
    }

    public async Task<bool> CambiarPasswordAsync(int id, string newPassword)
    {
        var e = await _db.Empleados.FindAsync(id);
        if (e == null) return false;

        e.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _db.SaveChangesAsync();
        return true;
    }
}
