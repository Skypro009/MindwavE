using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MindwavE.Services;

public class GeminiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

    public GeminiService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<string> GetCompletionAsync(string prompt)
    {
        var url = $"{BaseUrl}?key={Constants.GeminiApiKey}";
        
        // Inject system instruction locally by prepending to prompt
        var fullPrompt = "You are MindBot. " + prompt;

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = fullPrompt }
                    }
                }
            }
        };

        var response = await _httpClient.PostAsJsonAsync(url, requestBody);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(json);
            return geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? "No response";
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Gemini API Error: {response.StatusCode} - {errorContent}");
        }
    }
}

// Response Models
public class GeminiResponse
{
    [JsonPropertyName("candidates")]
    public List<Candidate> Candidates { get; set; }
}

public class Candidate
{
    [JsonPropertyName("content")]
    public Content Content { get; set; }
}

public class Content
{
    [JsonPropertyName("parts")]
    public List<Part> Parts { get; set; }
}

public class Part
{
    [JsonPropertyName("text")]
    public string Text { get; set; }
}
