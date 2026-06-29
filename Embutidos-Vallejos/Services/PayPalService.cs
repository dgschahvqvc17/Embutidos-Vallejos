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

    public async Task<string> CrearOrdenAsync(decimal monto, string moneda = "USD")
    {
        var accessToken = await GetAccessTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var orderData = new
        {
            intent = "CAPTURE",
            purchase_units = new[]
            {
                new
                {
                    amount = new
                    {
                        currency_code = moneda,
                        value = monto.ToString("F2")
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(orderData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(
            $"{_config["PayPal:Url"]}/v2/checkout/orders", content);

        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseBody);

        return result.GetProperty("id").GetString()!;
    }

    public async Task<bool> CapturarOrdenAsync(string orderId)
    {
        var accessToken = await GetAccessTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.PostAsync(
            $"{_config["PayPal:Url"]}/v2/checkout/orders/{orderId}/capture", null);

        if (!response.IsSuccessStatusCode) return false;

        var responseBody = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(responseBody);

        return result.GetProperty("status").GetString() == "COMPLETED";
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
