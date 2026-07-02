using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Embutidos_Vallejos.Services;

public class PayPalService : IPayPalService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public PayPalService(IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }

    public async Task<(string OrderId, string? ApprovalUrl)> CrearOrdenAsync(decimal monto, string? returnUrl = null, string? cancelUrl = null, string moneda = "USD")
    {
        _httpClient.DefaultRequestHeaders.Remove("Prefer");

        var accessToken = await GetAccessTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Prefer", "return=representation");

        object orderData;
        if (returnUrl != null)
        {
            orderData = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        amount = new
                        {
                            currency_code = moneda,
                            value = monto.ToString("F2", CultureInfo.InvariantCulture)
                        }
                    }
                },
                payment_source = new
                {
                    paypal = new
                    {
                        experience_context = new
                        {
                            return_url = returnUrl,
                            cancel_url = cancelUrl,
                            user_action = "PAY_NOW",
                            payment_method_preference = "IMMEDIATE_PAYMENT_REQUIRED",
                            brand_name = "Embutidos Vallejos",
                            locale = "en-US",
                            landing_page = "LOGIN"
                        }
                    }
                }
            };
        }
        else
        {
            orderData = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        amount = new
                        {
                            currency_code = moneda,
                            value = monto.ToString("F2", CultureInfo.InvariantCulture)
                        }
                    }
                }
            };
        }

        var json = JsonSerializer.Serialize(orderData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(
            $"{_config["PayPal:Url"]}/v2/checkout/orders", content);

        _httpClient.DefaultRequestHeaders.Remove("Prefer");

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"PayPal error ({response.StatusCode}): {errorBody}");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseBody);

        var orderId = result.GetProperty("id").GetString()!;

        string? approvalUrl = null;
        if (returnUrl != null)
        {
            var links = result.GetProperty("links");
            foreach (var link in links.EnumerateArray())
            {
                if (link.GetProperty("rel").GetString() == "payer-action")
                {
                    approvalUrl = link.GetProperty("href").GetString();
                    break;
                }
            }

            if (approvalUrl == null)
                throw new InvalidOperationException("No se encontró la URL de aprobación de PayPal");
        }

        return (orderId, approvalUrl);
    }

    public async Task<string> CapturarOrdenAsync(string orderId)
    {
        _httpClient.DefaultRequestHeaders.Remove("Prefer");

        var accessToken = await GetAccessTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.PostAsync(
            $"{_config["PayPal:Url"]}/v2/checkout/orders/{orderId}/capture",
            new StringContent("{}", Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"PayPal error al capturar ({response.StatusCode}): {errorBody}");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseBody);

        return result.GetProperty("status").GetString() ?? "UNKNOWN";
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var clientId = _config["PayPal:ClientId"];
        var secret = _config["PayPal:Secret"];

        var authBytes = Encoding.UTF8.GetBytes($"{clientId}:{secret}");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

        var tokenData = new List<KeyValuePair<string, string>>
        {
            new("grant_type", "client_credentials")
        };

        var response = await _httpClient.PostAsync(
            $"{_config["PayPal:Url"]}/v1/oauth2/token",
            new FormUrlEncodedContent(tokenData));

        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseBody);

        return result.GetProperty("access_token").GetString()!;
    }
}
