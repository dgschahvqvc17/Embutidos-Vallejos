namespace Embutidos_Vallejos.Models.DTOs;

public class ReporteVentasDto
{
    public int TotalVentas { get; set; }
    public decimal MontoTotal { get; set; }
    public decimal PromedioVenta { get; set; }
    public int TotalPedidos { get; set; }
    public decimal IngresoPedidos { get; set; }
    public List<VentaResumenDto> VentasRecientes { get; set; } = new();
    public List<PedidoResumenDto> PedidosRecientes { get; set; } = new();
}

public class VentaResumenDto
{
    public int VentaId { get; set; }
    public DateTime Fecha { get; set; }
    public string Cliente { get; set; } = null!;
    public decimal Total { get; set; }
    public string MetodoPago { get; set; } = null!;
}

public class PedidoResumenDto
{
    public int PedidoId { get; set; }
    public DateTime Fecha { get; set; }
    public string Cliente { get; set; } = null!;
    public decimal Total { get; set; }
    public string Estado { get; set; } = null!;
}

public class ReporteInventarioDto
{
    public int TotalProductos { get; set; }
    public int ProductosActivos { get; set; }
    public int ProductosStockBajo { get; set; }
    public int ProductosSinStock { get; set; }
    public decimal ValorInventario { get; set; }
    public List<ProductoDto> ProductosStockBajoLista { get; set; } = new();
}

public class DashboardDto
{
    public int TotalProductos { get; set; }
    public int TotalPedidos { get; set; }
    public int PedidosPendientes { get; set; }
    public int PedidosEntregados { get; set; }
    public int TotalClientes { get; set; }
    public int TotalEmpleados { get; set; }
    public int TotalRepartidores { get; set; }
    public decimal VentasHoy { get; set; }
    public decimal VentasMes { get; set; }
    public int ProductosStockBajo { get; set; }
}
