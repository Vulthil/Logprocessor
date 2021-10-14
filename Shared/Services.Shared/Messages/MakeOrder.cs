namespace Services.Shared.Messages
{
    public interface MakeOrder
    {
        public string SKU { get; set; }
        public int Amount { get; set; }
        public string Address { get; set; }
    }
}