using Embutidos_Vallejos.Models.DTOs;
using Embutidos_Vallejos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Embutidos_Vallejos.Controllers;

[Authorize(Roles = "Ventas,Administrador")]
public class VentasController : Controller
{
    private readonly IPedidoService _pedidoService;
    private readonly IProductoService _productoService;
    private readonly IVentaService _ventaService;
    private readonly IClienteService _clienteService;

    public VentasController(IPedidoService pedidoService, IProductoService productoService,
        IVentaService ventaService, IClienteService clienteService)
    {
        _pedidoService = pedidoService;
        _productoService = productoService;
        _ventaService = ventaService;
        _clienteService = clienteService;
    }

    private int GetEmpleadoId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index(string? estado, string? tab)
    {
        var pedidos = string.IsNullOrEmpty(estado)
            ? await _pedidoService.GetAllAsync()
            : await _pedidoService.GetByEstadoAsync(estado);

        var ventas = await _ventaService.GetAllAsync();

        ViewBag.EstadoActual = estado;
        ViewBag.TabActual = tab;
        ViewBag.Pedidos = pedidos;
        ViewBag.Ventas = ventas;
        return View();
    }

    public async Task<IActionResult> Details(int id)
    {
        var pedido = await _pedidoService.GetByIdAsync(id);
        if (pedido == null) return NotFound();
        return View(pedido);
    }

    [HttpPost]
    public async Task<IActionResult> ActualizarEstado(int id, string estado)
    {
        await _pedidoService.UpdateEstadoAsync(id, estado);
        TempData["Success"] = "Estado del pedido actualizado.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> NuevaVenta()
    {
        var productos = await _productoService.GetActivosAsync();
        var clientes = await _clienteService.GetAllAsync();
        ViewBag.Clientes = clientes;
        return View(productos);
    }

    [HttpPost]
    public async Task<IActionResult> NuevaVenta(VentaPosDto dto)
    {
        if (dto.Items == null || dto.Items.Count(i => i.Cantidad > 0) == 0)
        {
            TempData["Error"] = "Debe seleccionar al menos un producto.";
            return RedirectToAction(nameof(NuevaVenta));
        }

        var itemsValidos = dto.Items.Where(i => i.Cantidad > 0).ToList();

        if (dto.ClienteId == 0)
        {
            if (string.IsNullOrWhiteSpace(dto.NuevoClienteNombre) || string.IsNullOrWhiteSpace(dto.NuevoClienteApellido))
            {
                TempData["Error"] = "Debe seleccionar un cliente o crear uno nuevo.";
                return RedirectToAction(nameof(NuevaVenta));
            }

            var nuevoCliente = await _clienteService.RegisterAsync(new Models.RegisterViewModel
            {
                Nombre = dto.NuevoClienteNombre,
                Apellido = dto.NuevoClienteApellido,
                Telefono = dto.NuevoClienteTelefono ?? "+591",
                Email = dto.NuevoClienteEmail ?? $"venta_{Guid.NewGuid():N}@tmp.vallejos",
                Direccion = dto.NuevoClienteDireccion ?? "Venta presencial",
                Password = "tmp123"
            });

            dto.ClienteId = nuevoCliente.ClienteId;
        }

        var ventaDto = new VentaCreateDto
        {
            Total = itemsValidos.Sum(i => i.Cantidad * i.PrecioUnitario),
            MetodoPago = dto.MetodoPago ?? "Efectivo",
            ClienteId = dto.ClienteId,
            EmpleadoId = GetEmpleadoId(),
            Detalles = itemsValidos.Select(i => new VentaDetalleCreateDto
            {
                ProductoId = i.ProductoId,
                Cantidad = i.Cantidad,
                PrecioUnitario = i.PrecioUnitario
            }).ToList()
        };

        try
        {
            var venta = await _ventaService.CreateAsync(ventaDto);

            if (dto.MetodoPago == "QR")
                return RedirectToAction("QR", "Pago", new { ventaId = venta.VentaId, esVenta = true });

            TempData["Success"] = $"Venta #{venta.VentaId} registrada exitosamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error al registrar la venta: {ex.Message}";
            return RedirectToAction(nameof(NuevaVenta));
        }
    }

    [HttpGet]
    public async Task<IActionResult> BuscarClientes(string term)
    {
        var clientes = await _clienteService.SearchAsync(term ?? "");
        var result = clientes.Select(c => new
        {
            c.ClienteId,
            nombre = $"{c.Nombre} {c.Apellido}",
            c.Email,
            c.Telefono
        });
        return Json(result);
    }

    public async Task<IActionResult> Historial()
    {
        var ventas = await _ventaService.GetByEmpleadoAsync(GetEmpleadoId());
        return View(ventas);
    }

    public async Task<IActionResult> VentaDetalle(int id)
    {
        var venta = await _ventaService.GetByIdAsync(id);
        if (venta == null) return NotFound();
        return View(venta);
    }
}

public class VentaPosDto
{
    public int ClienteId { get; set; }
    public string? NuevoClienteNombre { get; set; }
    public string? NuevoClienteApellido { get; set; }
    public string? NuevoClienteTelefono { get; set; }
    public string? NuevoClienteEmail { get; set; }
    public string? NuevoClienteDireccion { get; set; }
    public string? MetodoPago { get; set; }
    public List<VentaItemDto> Items { get; set; } = new();
}

public class VentaItemDto
{
    public int ProductoId { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
}
