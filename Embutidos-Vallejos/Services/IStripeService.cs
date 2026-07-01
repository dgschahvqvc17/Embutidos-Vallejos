using Embutidos_Vallejos.Models.DTOs;

namespace Embutidos_Vallejos.Services;

public interface IStripeService
{
    Task<string> CrearCheckoutSessionAsync(PedidoDto pedido, string successUrl, string cancelUrl);
    Task<bool> ValidarCheckoutSessionAsync(string sessionId);
}
