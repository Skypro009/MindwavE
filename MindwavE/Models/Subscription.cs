using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MindwavE.Models
{
    [Table("subscriptions")]
    public class Subscription : BaseModel
    {
        [PrimaryKey("user_id", false)]
        [Column("user_id")]
        public string UserId { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("plan_id")]
        public string PlanId { get; set; }
        
        [Column("current_period_end")]
        public DateTime? CurrentPeriodEnd { get; set; }
    }
}
