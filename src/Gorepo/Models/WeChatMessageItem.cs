namespace Gorepo.Models
{
    public class WeChatMessageItem
    {
        public int Timestamp { get; set; }
        public string MessageId { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
