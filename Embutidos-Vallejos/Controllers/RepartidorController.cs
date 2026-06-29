using Embutidos_Vallejos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Embutidos_Vallejos.Controllers;

[Authorize(Roles = "Repartidor,Administrador")]
public class RepartidorController : Controller
{
    private readonly IEntregaService _entregaService;
    private readonly IPedidoService _pedidoService;

    public RepartidorController(IEntregaService entregaService, IPedidoService pedidoService)
    {
        _entregaService = entregaService;
        _pedidoService = pedidoService;
    }

    public async Task<IActionResult> Index()
    {
        var entregas = await _entregaService.GetPendientesAsync();
        ViewBag.Completadas = await _entregaService.GetByRepartidorAsync(0); // no filter, show all
        return View(entregas);
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
}
