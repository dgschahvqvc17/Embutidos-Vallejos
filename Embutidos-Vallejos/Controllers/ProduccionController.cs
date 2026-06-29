using Embutidos_Vallejos.Models.DTOs;
using Embutidos_Vallejos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Embutidos_Vallejos.Controllers;

[Authorize(Roles = "Producción,Administrador")]
public class ProduccionController : Controller
{
    private readonly IProductoService _productoService;
    private readonly ICategoriaProductoService _categoriaService;

    public ProduccionController(IProductoService productoService, ICategoriaProductoService categoriaService)
    {
        _productoService = productoService;
        _categoriaService = categoriaService;
    }

    public async Task<IActionResult> Index(int? categoriaId, string? search)
    {
        var productos = await _productoService.GetAllAsync();

        if (categoriaId.HasValue && categoriaId.Value > 0)
            productos = productos.Where(p => p.CategoriaId == categoriaId.Value).ToList();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.ToLower();
            productos = productos.Where(p => p.Nombre.ToLower().Contains(term) || (p.Descripcion ?? "").ToLower().Contains(term)).ToList();
        }

        ViewBag.Categorias = await _categoriaService.GetAllAsync();
        ViewBag.CategoriaId = categoriaId;
        ViewBag.Search = search;
        return View(productos);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleEstado(int id)
    {
        var producto = await _productoService.GetByIdAsync(id);
        if (producto == null) return NotFound();

        var nuevoEstado = producto.Estado == "Activo" ? "Inactivo" : "Activo";

        await _productoService.UpdateAsync(id, new ProductoUpdateDto
        {
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            PrecioProduccion = producto.PrecioProduccion,
            PrecioVenta = producto.PrecioVenta,
            Stock = producto.Stock,
            StockMinimo = producto.StockMinimo,
            Imagen = producto.Imagen,
            Estado = nuevoEstado,
            CategoriaId = producto.CategoriaId
        });

        TempData["Success"] = $"Producto {(nuevoEstado == "Activo" ? "activado" : "desactivado")}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ActualizarPrecios(int id, decimal precioProduccion, decimal precioVenta)
    {
        if (precioProduccion <= 0 || precioVenta <= 0)
        {
            TempData["Error"] = "Los precios deben ser mayores a cero.";
            return RedirectToAction(nameof(Index));
        }

        var producto = await _productoService.GetByIdAsync(id);
        if (producto == null) return NotFound();

        await _productoService.UpdateAsync(id, new ProductoUpdateDto
        {
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            PrecioProduccion = precioProduccion,
            PrecioVenta = precioVenta,
            Stock = producto.Stock,
            StockMinimo = producto.StockMinimo,
            Imagen = producto.Imagen,
            Estado = producto.Estado,
            CategoriaId = producto.CategoriaId
        });

        TempData["Success"] = $"Precios de '{producto.Nombre}' actualizados.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> RegistrarProduccion(int id, int cantidadProducida)
    {
        if (cantidadProducida <= 0)
        {
            TempData["Error"] = "La cantidad producida debe ser mayor a cero.";
            return RedirectToAction(nameof(Index));
        }

        var producto = await _productoService.GetByIdAsync(id);
        if (producto == null) return NotFound();

        await _productoService.UpdateStockAsync(id, cantidadProducida);

        if (producto.Estado != "Activo")
        {
            await _productoService.UpdateAsync(id, new ProductoUpdateDto
            {
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                PrecioProduccion = producto.PrecioProduccion,
                PrecioVenta = producto.PrecioVenta,
                Stock = producto.Stock + cantidadProducida,
                StockMinimo = producto.StockMinimo,
                Imagen = producto.Imagen,
                Estado = "Activo",
                CategoriaId = producto.CategoriaId
            });
        }

        TempData["Success"] = $"Produccion registrada: +{cantidadProducida} unidades de '{producto.Nombre}'.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ActualizarDescripcion(int id, string descripcion)
    {
        var producto = await _productoService.GetByIdAsync(id);
        if (producto == null) return NotFound();

        await _productoService.UpdateAsync(id, new ProductoUpdateDto
        {
            Nombre = producto.Nombre,
            Descripcion = descripcion,
            PrecioProduccion = producto.PrecioProduccion,
            PrecioVenta = producto.PrecioVenta,
            Stock = producto.Stock,
            StockMinimo = producto.StockMinimo,
            Imagen = producto.Imagen,
            Estado = producto.Estado,
            CategoriaId = producto.CategoriaId
        });

        TempData["Success"] = $"Descripcion de '{producto.Nombre}' actualizada.";
        return RedirectToAction(nameof(Index));
    }
}
