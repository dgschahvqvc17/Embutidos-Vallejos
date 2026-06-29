using Embutidos_Vallejos.Models.DTOs;
using Embutidos_Vallejos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Embutidos_Vallejos.Controllers;

[Authorize]
public class CarritoController : Controller
{
    private readonly IProductoService _productoService;
    private readonly IPedidoService _pedidoService;

    public CarritoController(IProductoService productoService, IPedidoService pedidoService)
    {
        _productoService = productoService;
        _pedidoService = pedidoService;
    }

    public IActionResult Index()
    {
        var items = GetCarritoItems();
        return View(items);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(int productoId, int cantidad = 1)
    {
        var producto = await _productoService.GetByIdAsync(productoId);
        if (producto == null || producto.Estado != "Activo")
            return Json(new { success = false, message = "Producto no disponible" });

        if (producto.Stock < cantidad)
            return Json(new { success = false, message = "Stock insuficiente" });

        var items = GetCarritoItems();
        var existente = items.FirstOrDefault(i => i.ProductoId == productoId);

        if (existente != null)
        {
            existente.Cantidad += cantidad;
        }
        else
        {
            items.Add(new CarritoItemDto
            {
                ProductoId = producto.ProductoId,
                NombreProducto = producto.Nombre,
                ImagenProducto = producto.Imagen,
                Cantidad = cantidad,
                PrecioUnitario = producto.PrecioVenta
            });
        }

        SetCarritoItems(items);

        return Json(new { success = true, message = "Producto agregado al carrito", count = items.Sum(i => i.Cantidad) });
    }

    [HttpPost]
    public IActionResult ActualizarCantidad(int productoId, int cantidad)
    {
        var items = GetCarritoItems();
        var item = items.FirstOrDefault(i => i.ProductoId == productoId);
        if (item == null) return Json(new { success = false });

        if (cantidad <= 0)
            items.Remove(item);
        else
            item.Cantidad = cantidad;

        SetCarritoItems(items);

        return Json(new
        {
            success = true,
            total = items.Sum(i => i.Cantidad * i.PrecioUnitario),
            count = items.Sum(i => i.Cantidad),
            subtotal = item != null && cantidad > 0 ? item.Cantidad * item.PrecioUnitario : 0
        });
    }

    [HttpPost]
    public IActionResult Eliminar(int productoId)
    {
        var items = GetCarritoItems();
        items.RemoveAll(i => i.ProductoId == productoId);
        SetCarritoItems(items);

        return Json(new
        {
            success = true,
            total = items.Sum(i => i.Cantidad * i.PrecioUnitario),
            count = items.Sum(i => i.Cantidad)
        });
    }

    [HttpPost]
    public async Task<IActionResult> AgregarDelPedido(int pedidoId)
    {
        var pedido = await _pedidoService.GetByIdAsync(pedidoId);
        if (pedido == null)
            return Json(new { success = false, message = "Pedido no encontrado" });

        var items = GetCarritoItems();

        foreach (var detalle in pedido.Detalles)
        {
            var producto = await _productoService.GetByIdAsync(detalle.ProductoId);
            if (producto == null || producto.Estado != "Activo" || producto.Stock <= 0)
                continue;

            var existente = items.FirstOrDefault(i => i.ProductoId == detalle.ProductoId);
            var qty = Math.Min(detalle.Cantidad, producto.Stock);

            if (existente != null)
            {
                existente.Cantidad += qty;
            }
            else
            {
                items.Add(new CarritoItemDto
                {
                    ProductoId = producto.ProductoId,
                    NombreProducto = producto.Nombre,
                    ImagenProducto = producto.Imagen,
                    Cantidad = qty,
                    PrecioUnitario = producto.PrecioVenta
                });
            }
        }

        SetCarritoItems(items);

        return Json(new { success = true, message = "Productos agregados al carrito", count = items.Sum(i => i.Cantidad) });
    }

    public IActionResult ObtenerConteo()
    {
        var items = GetCarritoItems();
        return Json(new { count = items.Sum(i => i.Cantidad), total = items.Sum(i => i.Cantidad * i.PrecioUnitario) });
    }

    private List<CarritoItemDto> GetCarritoItems()
    {
        var json = HttpContext.Session.GetString("Carrito");
        if (string.IsNullOrEmpty(json))
            return new List<CarritoItemDto>();

        return JsonSerializer.Deserialize<List<CarritoItemDto>>(json) ?? new List<CarritoItemDto>();
    }

    private void SetCarritoItems(List<CarritoItemDto> items)
    {
        HttpContext.Session.SetString("Carrito", JsonSerializer.Serialize(items));
    }
}
