using Stripe;
using Stripe.Checkout;
using Embutidos_Vallejos.Models.DTOs;
using Microsoft.Extensions.Configuration;

namespace Embutidos_Vallejos.Services;

public class StripeService : IStripeService
{
    private readonly IConfiguration _config;

    public StripeService(IConfiguration config)
    {
        _config = config;
        StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
    }

    public async Task<string> CrearCheckoutSessionAsync(PedidoDto pedido, string successUrl, string cancelUrl)
    {
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(pedido.Total * 100), // Stripe requiere el monto en centavos
                        Currency = "bob", // Bolivianos (BOB)
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = $"Pedido #{pedido.PedidoId} - Embutidos Vallejos",
                            Description = "Embutidos de alta calidad y sabor tradicional boliviano"
                        },
                    },
                    Quantity = 1,
                },
            },
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl,
            Metadata = new Dictionary<string, string>
            {
                { "PedidoId", pedido.PedidoId.ToString() }
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);
        return session.Url;
    }

    public async Task<bool> ValidarCheckoutSessionAsync(string sessionId)
    {
        try
        {
            var service = new SessionService();
            var session = await service.GetAsync(sessionId);
            return session != null && session.PaymentStatus == "paid";
        }
        catch
        {
            return false;
        }
    }
}
