using Embutidos_Vallejos.Models.DTOs;

namespace Embutidos_Vallejos.Services;

public interface IReporteService
{
    Task<DashboardDto> GetDashboardAsync();
    Task<ReporteVentasDto> GetReporteVentasAsync(DateTime? desde = null, DateTime? hasta = null);
    Task<ReporteInventarioDto> GetReporteInventarioAsync();
}
