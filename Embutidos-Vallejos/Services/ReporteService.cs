using Embutidos_Vallejos.Data;
using Embutidos_Vallejos.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Embutidos_Vallejos.Services;

public class ReporteService : IReporteService
{
    private readonly AppDbContext _db;

    public ReporteService(AppDbContext db) => _db = db;

    public async Task<DashboardDto> GetDashboardAsync()
    {
        var hoy = DateTime.Today;
        var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);

        var dashboard = new DashboardDto
        {
            TotalProductos = await _db.Productos.CountAsync(),
            TotalPedidos = await _db.Pedidos.CountAsync(),
            PedidosPendientes = await _db.Pedidos.CountAsync(p => p.Estado == "Pendiente"),
            PedidosEntregados = await _db.Pedidos.CountAsync(p => p.Estado == "Entregado"),
            TotalClientes = await _db.Clientes.CountAsync(),
            TotalEmpleados = await _db.Empleados.CountAsync(),
            TotalRepartidores = await _db.Repartidores.CountAsync(),
            VentasHoy = await _db.Ventas.Where(v => v.FechaVenta >= hoy).SumAsync(v => (decimal?)v.Total) ?? 0,
            VentasMes = await _db.Ventas.Where(v => v.FechaVenta >= inicioMes).SumAsync(v => (decimal?)v.Total) ?? 0,
            ProductosStockBajo = await _db.Productos.CountAsync(p => p.Stock <= p.StockMinimo && p.Estado == "Activo")
        };

        return dashboard;
    }

    public async Task<ReporteVentasDto> GetReporteVentasAsync(DateTime? desde = null, DateTime? hasta = null)
    {
        desde ??= DateTime.Today.AddDays(-30);
        hasta ??= DateTime.Now;

        var ventasQuery = _db.Ventas.Where(v => v.FechaVenta >= desde && v.FechaVenta <= hasta);
        var pedidosQuery = _db.Pedidos.Where(p => p.FechaPedido >= desde && p.FechaPedido <= hasta);

        var reporte = new ReporteVentasDto
        {
            TotalVentas = await ventasQuery.CountAsync(),
            MontoTotal = await ventasQuery.SumAsync(v => (decimal?)v.Total) ?? 0,
            TotalPedidos = await pedidosQuery.CountAsync(),
            IngresoPedidos = await pedidosQuery.SumAsync(p => (decimal?)p.Total) ?? 0,
            PromedioVenta = await ventasQuery.AverageAsync(v => (decimal?)v.Total) ?? 0,
            VentasRecientes = await ventasQuery
                .Include(v => v.Cliente)
                .OrderByDescending(v => v.FechaVenta)
                .Take(10)
                .Select(v => new VentaResumenDto
                {
                    VentaId = v.VentaId,
                    Fecha = v.FechaVenta,
                    Cliente = v.Cliente.Nombre + " " + v.Cliente.Apellido,
                    Total = v.Total,
                    MetodoPago = v.MetodoPago
                })
                .ToListAsync(),
            PedidosRecientes = await pedidosQuery
                .Include(p => p.Cliente)
                .OrderByDescending(p => p.FechaPedido)
                .Take(10)
                .Select(p => new PedidoResumenDto
                {
                    PedidoId = p.PedidoId,
                    Fecha = p.FechaPedido,
                    Cliente = p.Cliente.Nombre + " " + p.Cliente.Apellido,
                    Total = p.Total,
                    Estado = p.Estado
                })
                .ToListAsync()
        };

        return reporte;
    }

    public async Task<ReporteInventarioDto> GetReporteInventarioAsync()
    {
        var productos = await _db.Productos.ToListAsync();

        var reporte = new ReporteInventarioDto
        {
            TotalProductos = productos.Count,
            ProductosActivos = productos.Count(p => p.Estado == "Activo"),
            ProductosStockBajo = productos.Count(p => p.Stock <= p.StockMinimo && p.Estado == "Activo"),
            ProductosSinStock = productos.Count(p => p.Stock == 0 && p.Estado == "Activo"),
            ValorInventario = productos.Where(p => p.Estado == "Activo").Sum(p => p.PrecioProduccion * p.Stock)
        };

        reporte.ProductosStockBajoLista = await _db.Productos
            .Include(p => p.Categoria)
            .Where(p => p.Stock <= p.StockMinimo && p.Estado == "Activo")
            .Select(p => new ProductoDto
            {
                ProductoId = p.ProductoId,
                Nombre = p.Nombre,
                Stock = p.Stock,
                StockMinimo = p.StockMinimo,
                PrecioVenta = p.PrecioVenta,
                NombreCategoria = p.Categoria.Nombre,
                Estado = p.Estado
            })
            .ToListAsync();

        return reporte;
    }
}
