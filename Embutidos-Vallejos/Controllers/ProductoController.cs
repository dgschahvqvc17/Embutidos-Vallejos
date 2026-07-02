using Embutidos_Vallejos.Services;
using Microsoft.AspNetCore.Mvc;

namespace Embutidos_Vallejos.Controllers;

public class ProductoController : Controller
{
    private readonly IProductoService _productoService;
    private readonly ICategoriaProductoService _categoriaService;

    public ProductoController(IProductoService productoService, ICategoriaProductoService categoriaService)
    {
        _productoService = productoService;
        _categoriaService = categoriaService;
    }

    public async Task<IActionResult> Index(int? categoriaId, bool catalogo = false)
    {
        var productos = categoriaId.HasValue
            ? await _productoService.GetByCategoriaAsync(categoriaId.Value)
            : await _productoService.GetActivosAsync();

        ViewBag.Categorias = await _categoriaService.GetAllAsync();
        ViewBag.CategoriaActual = categoriaId;
        ViewBag.Catalogo = catalogo;

        return View(productos);
    }

    public async Task<IActionResult> Details(int id)
    {
        var producto = await _productoService.GetByIdAsync(id);
        if (producto == null) return NotFound();

        return View(producto);
    }
}
