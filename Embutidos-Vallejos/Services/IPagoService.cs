using Embutidos_Vallejos.Models.DTOs;

namespace Embutidos_Vallejos.Services;

public interface IPagoService
{
    Task<PagoDto?> GetByPedidoIdAsync(int pedidoId);
    Task<List<PagoDto>> GetAllAsync();
    Task<PagoDto> CreateAsync(PagoCreateDto dto);
    Task<bool> ConfirmarPagoAsync(int pagoId, string codigoTransaccion);
    Task<PagoReceiptDto?> GetReceiptAsync(int pedidoId);
}
