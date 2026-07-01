using Embutidos_Vallejos.Models.DTOs;
using Embutidos_Vallejos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Embutidos_Vallejos.Controllers;

[Authorize]
public class PedidoController : Controller
{
    private readonly IPedidoService _pedidoService;
    private readonly IPagoService _pagoService;
    private readonly IProductoService _productoService;

    public PedidoController(IPedidoService pedidoService, IPagoService pagoService, IProductoService productoService)
    {
        _pedidoService = pedidoService;
        _pagoService = pagoService;
        _productoService = productoService;
    }

    public async Task<IActionResult> Index()
    {
        var clienteId = GetClienteId();
        if (clienteId == null)
            return RedirectToAction("Login", "Account");

        var pedidos = await _pedidoService.GetByClienteAsync(clienteId.Value);
        return View(pedidos);
    }

    public async Task<IActionResult> Details(int id)
    {
        var pedido = await _pedidoService.GetByIdAsync(id);
        if (pedido == null) return NotFound();

        var clienteId = GetClienteId();
        if (pedido.ClienteId != clienteId && !User.IsInRole("Administrador"))
            return Forbid();

        return View(pedido);
    }

    public IActionResult Checkout()
    {
        var json = HttpContext.Session.GetString("Carrito");
        if (string.IsNullOrEmpty(json))
            return RedirectToAction("Index", "Carrito");

        var items = JsonSerializer.Deserialize<List<CarritoItemDto>>(json) ?? new();
        if (!items.Any())
            return RedirectToAction("Index", "Carrito");

        ViewBag.Total = items.Sum(i => i.Cantidad * i.PrecioUnitario);
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RealizarPedido(string tipoPago, string? direccionEntrega, string? direccionGuardada)
    {
        var clienteId = GetClienteId();
        if (clienteId == null)
            return Json(new { success = false, redirect = "/Account/Login" });

        var json = HttpContext.Session.GetString("Carrito");
        if (string.IsNullOrEmpty(json))
            return Json(new { success = false, message = "Carrito vacio" });

        var items = JsonSerializer.Deserialize<List<CarritoItemDto>>(json) ?? new();
        if (!items.Any())
            return Json(new { success = false, message = "Carrito vacio" });

        try
        {
            var dto = new PedidoCreateDto
            {
                DireccionEntrega = direccionEntrega ?? direccionGuardada,
                TipoPago = tipoPago,
                Items = items
            };

            var pedido = await _pedidoService.CreateAsync(clienteId.Value, dto);

            HttpContext.Session.Remove("Carrito");

            if (tipoPago == "PayPal")
            {
                return Json(new { success = true, redirect = Url.Action("PayPal", "Pago", new { pedidoId = pedido.PedidoId }) });
            }

            if (tipoPago == "QR")
            {
                return Json(new { success = true, redirect = Url.Action("QR", "Pago", new { pedidoId = pedido.PedidoId }) });
            }

            if (tipoPago == "Stripe")
            {
                return Json(new { success = true, redirect = Url.Action("Stripe", "Pago", new { pedidoId = pedido.PedidoId }) });
            }

            return Json(new { success = true, redirect = Url.Action("Details", new { id = pedido.PedidoId }) });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Cancelar(int id)
    {
        var clienteId = GetClienteId();
        if (clienteId == null) return Forbid();

        var pedido = await _pedidoService.GetByIdAsync(id);
        if (pedido == null || pedido.ClienteId != clienteId)
            return Forbid();

        var result = await _pedidoService.CancelarAsync(id);
        if (!result)
        {
            TempData["Error"] = "No se puede cancelar el pedido.";
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["Success"] = "Pedido cancelado exitosamente.";
        return RedirectToAction(nameof(Index));
    }

    private int? GetClienteId()
    {
        var userType = User.FindFirstValue("UserType");
        if (userType != "Cliente") return null;

        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idStr, out var id) ? id : null;
    }
}
