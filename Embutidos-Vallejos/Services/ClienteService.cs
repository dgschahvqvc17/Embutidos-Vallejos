using Embutidos_Vallejos.Data;
using Embutidos_Vallejos.Models;
using Embutidos_Vallejos.Models.DTOs;
using Embutidos_Vallejos.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Embutidos_Vallejos.Services;

public class ClienteService : IClienteService
{
    private readonly AppDbContext _db;

    public ClienteService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _db.Clientes.AnyAsync(c => c.Email == email);
    }

    public async Task<Cliente> RegisterAsync(RegisterViewModel model)
    {
        var cliente = new Cliente
        {
            Nombre = model.Nombre,
            Apellido = model.Apellido,
            Telefono = model.Telefono,
            Email = model.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Direccion = model.Direccion,
            FechaRegistro = DateTime.Now
        };

        _db.Clientes.Add(cliente);
        await _db.SaveChangesAsync();

        return cliente;
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        return await _db.Clientes.FindAsync(id);
    }

    public async Task<List<Cliente>> GetAllAsync()
    {
        return await _db.Clientes.ToListAsync();
    }

    public async Task<List<Cliente>> SearchAsync(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return await _db.Clientes.Take(10).ToListAsync();

        return await _db.Clientes
            .Where(c => c.Nombre.Contains(term) || c.Apellido.Contains(term) || c.Email.Contains(term) || c.Telefono!.Contains(term))
            .Take(20)
            .ToListAsync();
    }

    public async Task<PerfilDto?> GetProfileAsync(int id)
    {
        var c = await _db.Clientes.FindAsync(id);
        if (c == null) return null;

        return new PerfilDto
        {
            Id = c.ClienteId,
            Nombre = c.Nombre,
            Apellido = c.Apellido,
            Telefono = c.Telefono,
            Email = c.Email,
            Direccion = c.Direccion,
            UserType = "Cliente"
        };
    }

    public async Task<bool> UpdateProfileAsync(int id, PerfilUpdateDto dto)
    {
        var c = await _db.Clientes.FindAsync(id);
        if (c == null) return false;

        c.Nombre = dto.Nombre;
        c.Apellido = dto.Apellido;
        c.Telefono = dto.Telefono;
        c.Direccion = dto.Direccion;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> VerifyPasswordAsync(int id, string password)
    {
        var c = await _db.Clientes.FindAsync(id);
        if (c == null) return false;
        return BCrypt.Net.BCrypt.Verify(password, c.Password);
    }

    public async Task<bool> CambiarPasswordAsync(int id, string newPassword)
    {
        var c = await _db.Clientes.FindAsync(id);
        if (c == null) return false;

        c.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _db.SaveChangesAsync();
        return true;
    }
}
