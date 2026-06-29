using Embutidos_Vallejos.Models.DTOs;
using Embutidos_Vallejos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Embutidos_Vallejos.Controllers;

[Authorize(Roles = "Almacén,Administrador")]
public class AlmacenController : Controller
{
    private readonly IProductoService _productoService;
    private readonly ICategoriaProductoService _categoriaService;

    public AlmacenController(IProductoService productoService, ICategoriaProductoService categoriaService)
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
    public async Task<IActionResult> ActualizarStock(int id, int cantidad)
    {
        if (cantidad == 0)
        {
            TempData["Error"] = "La cantidad debe ser diferente de cero.";
            return RedirectToAction(nameof(Index));
        }

        var result = await _productoService.UpdateStockAsync(id, cantidad);

        if (result)
            TempData["Success"] = $"Stock actualizado ({(cantidad > 0 ? "+" : "")}{cantidad}).";
        else
            TempData["Error"] = "Error al actualizar el stock.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AjustarStock(int id, int stockFinal)
    {
        if (stockFinal < 0)
        {
            TempData["Error"] = "El stock no puede ser negativo.";
            return RedirectToAction(nameof(Index));
        }

        var producto = await _productoService.GetByIdAsync(id);
        if (producto == null) return NotFound();

        var diferencia = stockFinal - producto.Stock;

        if (diferencia == 0)
        {
            TempData["Error"] = "El stock es el mismo, no hay cambios.";
            return RedirectToAction(nameof(Index));
        }

        await _productoService.UpdateStockAsync(id, diferencia);
        TempData["Success"] = $"Stock de '{producto.Nombre}' ajustado a {stockFinal} ({(diferencia > 0 ? "+" : "")}{diferencia}).";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ActualizarStockMinimo(int id, int stockMinimo)
    {
        if (stockMinimo < 0)
        {
            TempData["Error"] = "El stock minimo no puede ser negativo.";
            return RedirectToAction(nameof(Index));
        }

        var producto = await _productoService.GetByIdAsync(id);
        if (producto == null) return NotFound();

        await _productoService.UpdateAsync(id, new ProductoUpdateDto
        {
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            PrecioProduccion = producto.PrecioProduccion,
            PrecioVenta = producto.PrecioVenta,
            Stock = producto.Stock,
            StockMinimo = stockMinimo,
            Imagen = producto.Imagen,
            Estado = producto.Estado,
            CategoriaId = producto.CategoriaId
        });

        TempData["Success"] = $"Stock minimo de '{producto.Nombre}' actualizado a {stockMinimo}.";
        return RedirectToAction(nameof(Index));
    }
}
