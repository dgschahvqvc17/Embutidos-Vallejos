using Embutidos_Vallejos.Models.DTOs;

namespace Embutidos_Vallejos.Services;

public interface IReporteExportacionService
{
    Task<byte[]> ExportarVentasPdfAsync(ReporteVentasDto reporte);
    Task<byte[]> ExportarInventarioPdfAsync(ReporteInventarioDto reporte);
    Task<byte[]> ExportarVentasExcelAsync(ReporteVentasDto reporte);
    Task<byte[]> ExportarInventarioExcelAsync(ReporteInventarioDto reporte);
    Task<byte[]> ExportarReporteCompletoPdfAsync(ReporteVentasDto ventas, ReporteInventarioDto inventario);
    Task<byte[]> ExportarReporteCompletoExcelAsync(ReporteVentasDto ventas, ReporteInventarioDto inventario);
}
