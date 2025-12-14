using MindwavE.Models;

namespace MindwavE.Services
{
    public class SubscriptionService
    {
        private readonly Supabase.Client _supabaseClient;

        public SubscriptionService(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        public async Task<Subscription?> GetSubscriptionAsync()
        {
            var user = _supabaseClient.Auth.CurrentUser;
            if (user == null) return null;

            try 
            {
                var response = await _supabaseClient.From<Subscription>()
                    .Where(x => x.UserId == user.Id)
                    .Limit(1)
                    .Get();
                
                return response.Models.FirstOrDefault();
            }
            catch
            {
                // Handle case where table might not exist or other error
                return null;
            }
        }

        public async Task SubscribeAsync(string planId)
        {
            var user = _supabaseClient.Auth.CurrentUser;
            if (user == null) return;

            var existing = await GetSubscriptionAsync();
            
            if (existing != null)
            {
                existing.Status = "active";
                existing.PlanId = planId;
                existing.CurrentPeriodEnd = DateTime.UtcNow.AddMonths(1);
                // Update requires PrimaryKey to be set on the object
                await _supabaseClient.From<Subscription>().Update(existing);
            }
            else
            {
                var sub = new Subscription
                {
                    UserId = user.Id,
                    Status = "active",
                    PlanId = planId,
                    CurrentPeriodEnd = DateTime.UtcNow.AddMonths(1)
                };
                await _supabaseClient.From<Subscription>().Insert(sub);
            }
        }
    }
}
