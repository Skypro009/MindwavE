using System;
using System.Threading.Tasks;

namespace MindwavE.Services
{
    public interface IAiService
    {
        Task<string> GetResponseAsync(string userMessage);
    }

    public class AiService : IAiService
    {
        // TODO: Replace with real API key and endpoint
        private const string ApiKey = "YOUR_API_KEY_HERE";
        
        public async Task<string> GetResponseAsync(string userMessage)
        {
            // Simulate network delay
            await Task.Delay(1000);

            // Simple mock logic for now, enabling API structure
            if (string.IsNullOrWhiteSpace(userMessage))
                return "Molim te, reci mi nešto.";

            return $"Razumem da kažeš: '{userMessage}'. Ja sam MindwavE AI, tu sam da te saslušam. (Ovo je demo odgovor dok ne ubacimo pravi API ključ).";
        }
    }
}
