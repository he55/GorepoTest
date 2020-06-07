namespace Gorepo.Models
{
    public class WeChatMessage
    {
        public int CreateTime { get; set; }
        public string ServerId { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
