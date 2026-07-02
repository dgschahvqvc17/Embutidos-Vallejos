namespace Embutidos_Vallejos.Services;

public interface IQRService
{
    string GenerarQRCode(string texto);
    string GenerarQRPago(int pedidoId, decimal monto, string referencia);
}
