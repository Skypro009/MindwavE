using MindwavE.Models;

namespace MindwavE.Services
{
    public class ChatService
    {
        private readonly Supabase.Client _supabaseClient;

        public ChatService(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        public async Task<List<ChatMessage>> GetMessagesAsync()
        {
            var response = await _supabaseClient.From<ChatMessage>()
                .Order(x => x.CreatedAt, Supabase.Postgrest.Constants.Ordering.Ascending)
                .Get();
            return response.Models;
        }

        public async Task SaveMessageAsync(string content, string role)
        {
            var user = _supabaseClient.Auth.CurrentUser;
            if (user == null) return;

            var message = new ChatMessage
            {
                UserId = user.Id,
                Content = content,
                Role = role,
                CreatedAt = DateTime.UtcNow
            };

            await _supabaseClient.From<ChatMessage>().Insert(message);
        }
    }
}
