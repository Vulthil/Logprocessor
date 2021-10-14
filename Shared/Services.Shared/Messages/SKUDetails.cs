namespace Services.Shared.Messages
{
    public interface SKUDetails
    {
        public string SKU { get; set; }
        public int Amount { get; set; }
        public double Price { get; set; }
        public int Qty { get; set; }
    }
}