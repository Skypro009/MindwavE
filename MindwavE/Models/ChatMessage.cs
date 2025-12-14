using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace MindwavE.Models
{
    [Table("chat_messages")]
    public class ChatMessage : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

        [Column("content")]
        public string Content { get; set; }

        [Column("role")]
        public string Role { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
