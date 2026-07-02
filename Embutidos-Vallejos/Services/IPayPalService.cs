namespace Embutidos_Vallejos.Services;

public interface IPayPalService
{
    Task<string> CrearOrdenAsync(decimal monto, string moneda = "USD");
    Task<bool> CapturarOrdenAsync(string orderId);
}
