using Embutidos_Vallejos.Data;
using Embutidos_Vallejos.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Embutidos_Vallejos.Services;

public static class DbInitializer
{
    public static async Task InitializeAsync(AppDbContext db)
    {
        if (!await db.Roles.AnyAsync())
        {
            var roles = new List<Rol>
            {
                new() { NombreRol = "Administrador" },
                new() { NombreRol = "Producción" },
                new() { NombreRol = "Ventas" },
                new() { NombreRol = "Almacén" },
                new() { NombreRol = "Repartidor" }
            };

            db.Roles.AddRange(roles);
            await db.SaveChangesAsync();
        }

        if (!await db.Empleados.AnyAsync())
        {
            var admin = new Empleado
            {
                Nombre = "Admin",
                Apellido = "Vallejos",
                Email = "admin@vallejos.com",
                Password = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Telefono = "+59170000000",
                FechaContratacion = DateTime.Today,
                Estado = "Activo",
                RolId = 1
            };

            db.Empleados.Add(admin);
            await db.SaveChangesAsync();
        }

        if (!await db.CategoriasProducto.AnyAsync())
        {
            var categorias = new List<CategoriaProducto>
            {
                new() { Nombre = "Chorizos", Descripcion = "Chorizos artesanales" },
                new() { Nombre = "Salchichas", Descripcion = "Salchichas de primera calidad" },
                new() { Nombre = "Jamones", Descripcion = "Jamones cocidos y ahumados" },
                new() { Nombre = "Embutidos Secos", Descripcion = "Embutidos curados y secos" }
            };

            db.CategoriasProducto.AddRange(categorias);
            await db.SaveChangesAsync();
        }
    }
}
