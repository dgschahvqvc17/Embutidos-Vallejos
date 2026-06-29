using System.Diagnostics;
using Embutidos_Vallejos.Models;
using Embutidos_Vallejos.Services;
using Microsoft.AspNetCore.Mvc;

namespace Embutidos_Vallejos.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IReporteService _reporteService;

    public HomeController(ILogger<HomeController> logger, IReporteService reporteService)
    {
        _logger = logger;
        _reporteService = reporteService;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var dashboard = await _reporteService.GetDashboardAsync();
            return View(dashboard);
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
