using Embutidos_Vallejos.Models.DTOs;
using Embutidos_Vallejos.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Embutidos_Vallejos.Controllers;

[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly IEmpleadoService _empleadoService;
    private readonly IProductoService _productoService;
    private readonly ICategoriaProductoService _categoriaService;
    private readonly IRepartidorService _repartidorService;
    private readonly IPedidoService _pedidoService;
    private readonly IReporteService _reporteService;
    private readonly IEntregaService _entregaService;
    private readonly IReporteExportacionService _reporteExportacionService;

    public AdminController(
        IEmpleadoService empleadoService,
        IProductoService productoService,
        ICategoriaProductoService categoriaService,
        IRepartidorService repartidorService,
        IPedidoService pedidoService,
        IReporteService reporteService,
        IEntregaService entregaService,
        IReporteExportacionService reporteExportacionService)
    {
        _empleadoService = empleadoService;
        _productoService = productoService;
        _categoriaService = categoriaService;
        _repartidorService = repartidorService;
        _pedidoService = pedidoService;
        _reporteService = reporteService;
        _entregaService = entregaService;
        _reporteExportacionService = reporteExportacionService;
    }

    public async Task<IActionResult> Index()
    {
        var dashboard = await _reporteService.GetDashboardAsync();
        return View(dashboard);
    }

    // ─── Empleados ───
    public async Task<IActionResult> Empleados(int? rolId)
    {
        List<EmpleadoDto> empleados;
        if (rolId.HasValue)
            empleados = await _empleadoService.GetByRolIdAsync(rolId.Value);
        else
            empleados = await _empleadoService.GetAllAsync();

        ViewBag.RolFiltro = rolId;
        ViewBag.Roles = await _empleadoService.GetAllRolesAsync();
        return View(empleados);
    }

    public IActionResult CrearEmpleado()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CrearEmpleado(EmpleadoCreateDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        if (await _empleadoService.EmailExistsAsync(dto.Email))
        {
            ModelState.AddModelError("Email", "Este correo ya esta registrado.");
            return View(dto);
        }

        await _empleadoService.CreateAsync(dto);
        TempData["Success"] = "Empleado creado exitosamente.";
        return RedirectToAction(nameof(Empleados));
    }

    public async Task<IActionResult> EditarEmpleado(int id)
    {
        var empleado = await _empleadoService.GetByIdAsync(id);
        if (empleado == null) return NotFound();

        var dto = new EmpleadoUpdateDto
        {
            Nombre = empleado.Nombre,
            Apellido = empleado.Apellido,
            Telefono = empleado.Telefono,
            Email = empleado.Email,
            FechaContratacion = empleado.FechaContratacion,
            Estado = empleado.Estado,
            RolId = empleado.NombreRol switch
            {
                "Administrador" => 1,
                "Producción" => 2,
                "Ventas" => 3,
                "Almacén" => 4,
                "Repartidor" => 5,
                _ => 0
            },
            PlacaVehiculo = empleado.PlacaVehiculo
        };

        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> EditarEmpleado(int id, EmpleadoUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var result = await _empleadoService.UpdateAsync(id, dto);
        if (result == null) return NotFound();

        TempData["Success"] = "Empleado actualizado exitosamente.";
        return RedirectToAction(nameof(Empleados));
    }

    [HttpPost]
    public async Task<IActionResult> EliminarEmpleado(int id)
    {
        var result = await _empleadoService.DeleteAsync(id);
        if (!result) return NotFound();

        TempData["Success"] = "Empleado eliminado exitosamente.";
        return RedirectToAction(nameof(Empleados));
    }

    // ─── Productos ───
    public async Task<IActionResult> Productos()
    {
        var productos = await _productoService.GetAllAsync();
        return View(productos);
    }

    public async Task<IActionResult> CrearProducto()
    {
        ViewBag.Categorias = await _categoriaService.GetAllAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CrearProducto(ProductoCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categorias = await _categoriaService.GetAllAsync();
            return View(dto);
        }

        await _productoService.CreateAsync(dto);
        TempData["Success"] = "Producto creado exitosamente.";
        return RedirectToAction(nameof(Productos));
    }

    public async Task<IActionResult> EditarProducto(int id)
    {
        var producto = await _productoService.GetByIdAsync(id);
        if (producto == null) return NotFound();

        ViewBag.Categorias = await _categoriaService.GetAllAsync();

        var dto = new ProductoUpdateDto
        {
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            PrecioProduccion = producto.PrecioProduccion,
            PrecioVenta = producto.PrecioVenta,
            Stock = producto.Stock,
            StockMinimo = producto.StockMinimo,
            Imagen = producto.Imagen,
            Estado = producto.Estado,
            CategoriaId = producto.CategoriaId
        };

        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> EditarProducto(int id, ProductoUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categorias = await _categoriaService.GetAllAsync();
            return View(dto);
        }

        var result = await _productoService.UpdateAsync(id, dto);
        if (result == null) return NotFound();

        TempData["Success"] = "Producto actualizado exitosamente.";
        return RedirectToAction(nameof(Productos));
    }

    [HttpPost]
    public async Task<IActionResult> EliminarProducto(int id)
    {
        var result = await _productoService.DeleteAsync(id);
        if (!result) return NotFound();

        TempData["Success"] = "Producto eliminado exitosamente.";
        return RedirectToAction(nameof(Productos));
    }

    // ─── Categorias ───
    public async Task<IActionResult> Categorias()
    {
        var categorias = await _categoriaService.GetAllAsync();
        return View(categorias);
    }

    [HttpPost]
    public async Task<IActionResult> CrearCategoria(CategoriaCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Datos invalidos.";
            return RedirectToAction(nameof(Categorias));
        }

        await _categoriaService.CreateAsync(dto);
        TempData["Success"] = "Categoria creada exitosamente.";
        return RedirectToAction(nameof(Categorias));
    }

    [HttpPost]
    public async Task<IActionResult> EditarCategoria(int id, CategoriaUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Datos invalidos.";
            return RedirectToAction(nameof(Categorias));
        }

        var result = await _categoriaService.UpdateAsync(id, dto);
        if (result == null) return NotFound();

        TempData["Success"] = "Categoria actualizada exitosamente.";
        return RedirectToAction(nameof(Categorias));
    }

    [HttpPost]
    public async Task<IActionResult> EliminarCategoria(int id)
    {
        var result = await _categoriaService.DeleteAsync(id);
        if (!result)
        {
            TempData["Error"] = "No se puede eliminar la categoria. Tiene productos asociados o no existe.";
            return RedirectToAction(nameof(Categorias));
        }

        TempData["Success"] = "Categoria eliminada exitosamente.";
        return RedirectToAction(nameof(Categorias));
    }

    // ─── Repartidores ───
    public async Task<IActionResult> Repartidores()
    {
        var repartidores = await _repartidorService.GetAllAsync();
        return View(repartidores);
    }

    [HttpPost]
    public async Task<IActionResult> CrearRepartidor(RepartidorCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Datos invalidos.";
            return RedirectToAction(nameof(Repartidores));
        }

        if (await _empleadoService.EmailExistsAsync(dto.Email))
        {
            TempData["Error"] = "El email ya esta registrado.";
            return RedirectToAction(nameof(Repartidores));
        }

        try
        {
            await _repartidorService.CreateAsync(dto);
            TempData["Success"] = "Repartidor creado exitosamente.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error al crear repartidor: {ex.Message}";
        }
        return RedirectToAction(nameof(Repartidores));
    }

    [HttpPost]
    public async Task<IActionResult> EditarRepartidor(int id, RepartidorUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Datos invalidos.";
            return RedirectToAction(nameof(Repartidores));
        }

        try
        {
            var result = await _repartidorService.UpdateAsync(id, dto);
            if (result == null) return NotFound();
            TempData["Success"] = "Repartidor actualizado exitosamente.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error al actualizar: {ex.Message}";
        }
        return RedirectToAction(nameof(Repartidores));
    }

    [HttpPost]
    public async Task<IActionResult> EliminarRepartidor(int id)
    {
        var result = await _repartidorService.DeleteAsync(id);
        if (!result)
        {
            TempData["Error"] = "No se puede eliminar el repartidor. Tiene entregas pendientes.";
            return RedirectToAction(nameof(Repartidores));
        }

        TempData["Success"] = "Repartidor eliminado exitosamente.";
        return RedirectToAction(nameof(Repartidores));
    }

    // ─── Pedidos ───
    public async Task<IActionResult> Pedidos(string? estado = null)
    {
        List<PedidoDto> pedidos;
        if (string.IsNullOrEmpty(estado) || estado == "Todos")
            pedidos = await _pedidoService.GetAllAsync();
        else
            pedidos = await _pedidoService.GetByEstadoAsync(estado);

        ViewBag.EstadoFiltro = estado ?? "Todos";
        return View(pedidos);
    }

    public async Task<IActionResult> DetallePedido(int id)
    {
        var pedido = await _pedidoService.GetByIdAsync(id);
        if (pedido == null) return NotFound();

        ViewBag.Repartidores = await _repartidorService.GetDisponiblesAsync();
        return View(pedido);
    }

    [HttpPost]
    public async Task<IActionResult> AsignarRepartidor(int pedidoId, int repartidorId)
    {
        var result = await _pedidoService.AsignarRepartidorAsync(pedidoId, repartidorId);
        if (!result)
        {
            TempData["Error"] = "Error al asignar repartidor.";
            return RedirectToAction(nameof(DetallePedido), new { id = pedidoId });
        }

        await _entregaService.CreateAsync(pedidoId, repartidorId);

        TempData["Success"] = "Repartidor asignado exitosamente.";
        return RedirectToAction(nameof(DetallePedido), new { id = pedidoId });
    }

    [HttpPost]
    public async Task<IActionResult> ActualizarEstadoPedido(int id, string estado)
    {
        var result = await _pedidoService.UpdateEstadoAsync(id, estado);
        if (!result)
        {
            TempData["Error"] = "Error al actualizar el estado.";
            return RedirectToAction(nameof(Pedidos));
        }

        TempData["Success"] = $"Estado actualizado a: {estado}";
        return RedirectToAction(nameof(DetallePedido), new { id });
    }

    [HttpPost]
    public async Task<IActionResult> CancelarPedido(int id)
    {
        var result = await _pedidoService.CancelarAsync(id);
        if (!result)
        {
            TempData["Error"] = "No se puede cancelar el pedido.";
            return RedirectToAction(nameof(Pedidos));
        }

        TempData["Success"] = "Pedido cancelado exitosamente.";
        return RedirectToAction(nameof(Pedidos));
    }

    // ─── Reportes ───
    public async Task<IActionResult> Reportes()
    {
        var reporteVentas = await _reporteService.GetReporteVentasAsync();
        var reporteInventario = await _reporteService.GetReporteInventarioAsync();

        ViewBag.ReporteVentas = reporteVentas;
        ViewBag.ReporteInventario = reporteInventario;

        return View();
    }

    public async Task<IActionResult> ReporteVentas(DateTime? desde, DateTime? hasta)
    {
        var reporte = await _reporteService.GetReporteVentasAsync(desde, hasta);
        return Json(reporte);
    }

    public async Task<IActionResult> ExportarVentasPdf()
    {
        var reporte = await _reporteService.GetReporteVentasAsync();
        var pdf = await _reporteExportacionService.ExportarVentasPdfAsync(reporte);
        return File(pdf, "application/pdf", $"reporte-ventas-{DateTime.Now:yyyyMMdd}.pdf");
    }

    public async Task<IActionResult> ExportarInventarioPdf()
    {
        var reporte = await _reporteService.GetReporteInventarioAsync();
        var pdf = await _reporteExportacionService.ExportarInventarioPdfAsync(reporte);
        return File(pdf, "application/pdf", $"reporte-inventario-{DateTime.Now:yyyyMMdd}.pdf");
    }

    public async Task<IActionResult> ExportarVentasExcel()
    {
        var reporte = await _reporteService.GetReporteVentasAsync();
        var excel = await _reporteExportacionService.ExportarVentasExcelAsync(reporte);
        return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"reporte-ventas-{DateTime.Now:yyyyMMdd}.xlsx");
    }

    public async Task<IActionResult> ExportarInventarioExcel()
    {
        var reporte = await _reporteService.GetReporteInventarioAsync();
        var excel = await _reporteExportacionService.ExportarInventarioExcelAsync(reporte);
        return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"reporte-inventario-{DateTime.Now:yyyyMMdd}.xlsx");
    }

    public async Task<IActionResult> ExportarReporteCompletoPdf()
    {
        var ventas = await _reporteService.GetReporteVentasAsync();
        var inventario = await _reporteService.GetReporteInventarioAsync();
        var pdf = await _reporteExportacionService.ExportarReporteCompletoPdfAsync(ventas, inventario);
        return File(pdf, "application/pdf", $"reporte-general-{DateTime.Now:yyyyMMdd}.pdf");
    }

    public async Task<IActionResult> ExportarReporteCompletoExcel()
    {
        var ventas = await _reporteService.GetReporteVentasAsync();
        var inventario = await _reporteService.GetReporteInventarioAsync();
        var excel = await _reporteExportacionService.ExportarReporteCompletoExcelAsync(ventas, inventario);
        return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"reporte-general-{DateTime.Now:yyyyMMdd}.xlsx");
    }
}
