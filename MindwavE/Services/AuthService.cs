using Supabase.Gotrue;

namespace MindwavE.Services
{
    public class AuthService
    {
        private readonly Supabase.Client _supabaseClient;

        public AuthService(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        public async Task InitializeAsync()
        {
            await _supabaseClient.InitializeAsync();
        }

        public async Task<Session?> SignUp(string email, string password, string username)
        {
            var options = new SignUpOptions
            {
                Data = new Dictionary<string, object> { { "username", username } }
            };
            
            var session = await _supabaseClient.Auth.SignUp(email, password, options);
            return session;
        }

        public async Task<Session?> SignIn(string email, string password)
        {
            var session = await _supabaseClient.Auth.SignIn(email, password);
            return session;
        }

        public async Task SignOut()
        {
            await _supabaseClient.Auth.SignOut();
        }

        public async Task<Session?> RetrieveSessionAsync()
        {
            return await _supabaseClient.Auth.RetrieveSessionAsync();
        }

        public User? GetCurrentUser()
        {
            return _supabaseClient.Auth.CurrentUser;
        }
        
        public Session? GetCurrentSession()
        {
            return _supabaseClient.Auth.CurrentSession;
        }
    }
}
