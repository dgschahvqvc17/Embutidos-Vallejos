using Embutidos_Vallejos.Models.DTOs;
using Embutidos_Vallejos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Embutidos_Vallejos.Controllers;

[Authorize(Roles = "Repartidor,Administrador")]
public class RepartidorController : Controller
{
    private readonly IEntregaService _entregaService;
    private readonly IPedidoService _pedidoService;
    private readonly IRepartidorService _repartidorService;

    public RepartidorController(
        IEntregaService entregaService,
        IPedidoService pedidoService,
        IRepartidorService repartidorService)
    {
        _entregaService = entregaService;
        _pedidoService = pedidoService;
        _repartidorService = repartidorService;
    }

    private int GetEmpleadoId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index()
    {
        var empleadoId = GetEmpleadoId();
        var repartidor = await _repartidorService.GetByEmpleadoIdAsync(empleadoId);

        var dashboard = new RepartidorDashboardDto
        {
            PedidosDisponibles = await _pedidoService.GetDisponiblesParaRepartoAsync()
        };

        if (repartidor != null)
        {
            dashboard.NombreRepartidor = $"{repartidor.Nombre} {repartidor.Apellido}";
            dashboard.PlacaVehiculo = repartidor.PlacaVehiculo;
            dashboard.EntregasPendientes = await _entregaService.GetPendientesByRepartidorAsync(repartidor.RepartidorId);
            dashboard.EntregasCompletadas = (await _entregaService.GetByRepartidorAsync(repartidor.RepartidorId))
                .Where(e => e.EstadoEntrega == "Entregado")
                .OrderByDescending(e => e.FechaEntrega)
                .ToList();
        }
        else
        {
            dashboard.NombreRepartidor = "Administrador";
            dashboard.EntregasPendientes = await _entregaService.GetPendientesAsync();
            dashboard.EntregasCompletadas = await _entregaService.GetCompletadasAsync();
        }

        return View(dashboard);
    }

    [HttpPost]
    public async Task<IActionResult> IniciarEntrega(int id)
    {
        var result = await _entregaService.IniciarEntregaAsync(id);
        if (result)
            TempData["Success"] = "Entrega iniciada. ¡Buen viaje!";
        else
            TempData["Error"] = "Error al iniciar la entrega.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> CompletarEntrega(int id, string? observaciones)
    {
        var result = await _entregaService.CompletarEntregaAsync(id, observaciones);
        if (result)
            TempData["Success"] = "Entrega completada exitosamente.";
        else
            TempData["Error"] = "Error al completar la entrega.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> TomarPedido(int pedidoId)
    {
        var empleadoId = GetEmpleadoId();
        var repartidor = await _repartidorService.GetByEmpleadoIdAsync(empleadoId);

        if (repartidor == null)
        {
            TempData["Error"] = "No se encontro el repartidor asociado a tu cuenta.";
            return RedirectToAction(nameof(Index));
        }

        var pedido = await _pedidoService.GetByIdAsync(pedidoId);
        if (pedido == null || pedido.RepartidorId != null)
        {
            TempData["Error"] = "El pedido no esta disponible para recoger.";
            return RedirectToAction(nameof(Index));
        }

        var asignado = await _pedidoService.AsignarRepartidorAsync(pedidoId, repartidor.RepartidorId);
        if (!asignado)
        {
            TempData["Error"] = "Error al asignar el pedido.";
            return RedirectToAction(nameof(Index));
        }

        await _entregaService.CreateAsync(pedidoId, repartidor.RepartidorId);
        TempData["Success"] = $"Pedido #{pedidoId} asignado exitosamente. ¡A entregar!";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> PedidosRealizados()
    {
        var empleadoId = GetEmpleadoId();
        var repartidor = await _repartidorService.GetByEmpleadoIdAsync(empleadoId);

        if (repartidor == null)
            return NotFound();

        var entregas = await _entregaService.GetByRepartidorAsync(repartidor.RepartidorId);
        var completadas = entregas
            .Where(e => e.EstadoEntrega == "Entregado")
            .OrderByDescending(e => e.FechaEntrega)
            .ToList();

        return View(completadas);
    }

    public async Task<IActionResult> PedidosDisponibles()
    {
        var pedidos = await _pedidoService.GetDisponiblesParaRepartoAsync();
        return View(pedidos);
    }
}
