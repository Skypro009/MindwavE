namespace MindwavE.Models
{
    public class ChatMessage
    {
        public string Text { get; set; }
        public bool IsUser { get; set; }
        public DateTime Timestamp { get; set; }

        public ChatMessage(string text, bool isUser)
        {
            Text = text;
            IsUser = isUser;
            Timestamp = DateTime.Now;
        }
    }
}
