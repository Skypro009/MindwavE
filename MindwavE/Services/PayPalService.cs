using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MindwavE.Services
{
    public class PayPalService
    {
        private readonly HttpClient _httpClient;
        private string _accessToken;
        private DateTime _tokenExpiry;

        public PayPalService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(Constants.PayPalBaseUrl);
        }

        private async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiry)
                return _accessToken;

            var authBytes = Encoding.ASCII.GetBytes($"{Constants.PayPalClientId}:{Constants.PayPalSecret}");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authBytes));

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var response = await _httpClient.PostAsync("/v1/oauth2/token", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonNode.Parse(json);
            
            _accessToken = doc["access_token"].ToString();
            var expiresIn = doc["expires_in"].GetValue<int>();
            _tokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn - 60);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            return _accessToken;
        }

        public async Task<(string orderId, string approveUrl)> CreateOrderAsync(decimal amount)
        {
            await GetAccessTokenAsync();

            var orderRequest = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        amount = new
                        {
                            currency_code = "USD",
                            value = amount.ToString("0.00")
                        }
                    }
                },
                application_context = new
                {
                    return_url = "mindwave://paypal/success",
                    cancel_url = "mindwave://paypal/cancel"
                }
            };

            var jsonContent = JsonSerializer.Serialize(orderRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/v2/checkout/orders", content);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var doc = JsonNode.Parse(jsonResponse);

            var orderId = doc["id"].ToString();
            var links = doc["links"].AsArray();
            var approveUrl = links.First(x => x["rel"].ToString() == "approve")["href"].ToString();

            return (orderId, approveUrl);
        }

        public async Task<bool> CaptureOrderAsync(string orderId)
        {
            await GetAccessTokenAsync();

            var content = new StringContent("", Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"/v2/checkout/orders/{orderId}/capture", content);

            return response.IsSuccessStatusCode;
        }
    }
}
