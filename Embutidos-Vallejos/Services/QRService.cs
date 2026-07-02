using QRCoder;

namespace Embutidos_Vallejos.Services;

public class QRService : IQRService
{
    public string GenerarQRCode(string texto)
    {
        using var qrGenerator = new QRCodeGenerator();
        var qrData = qrGenerator.CreateQrCode(texto, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrData);
        var qrBytes = qrCode.GetGraphic(20);

        return Convert.ToBase64String(qrBytes);
    }

    public string GenerarQRPago(int pedidoId, decimal monto, string referencia)
    {
        var data = $@"{{
    ""pedido_id"": {pedidoId},
    ""monto"": {monto.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)},
    ""referencia"": ""{referencia}"",
    ""fecha"": ""{DateTime.Now:yyyy-MM-dd HH:mm:ss}"",
    ""concepto"": ""Pago Embutidos Vallejos - Pedido #{pedidoId}""
}}";

        return GenerarQRCode(data);
    }
}
