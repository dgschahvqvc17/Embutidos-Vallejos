namespace Embutidos_Vallejos.Services;

public interface IPayPalService
{
    Task<(string OrderId, string? ApprovalUrl)> CrearOrdenAsync(decimal monto, string? returnUrl = null, string? cancelUrl = null, string moneda = "USD");
    Task<string> CapturarOrdenAsync(string orderId);
}
