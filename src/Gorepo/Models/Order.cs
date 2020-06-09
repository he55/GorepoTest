namespace Gorepo.Models
{
    public class Order
    {
        public string OrderId { get; set; } = "";
        public decimal OrderAmount { get; set; }
        public string Code { get; set; } = "";
    }
}
