namespace Services.Shared.Messages
{
    public interface GetSKUDetails
    {
        public string SKU { get; set; }
        public int Amount { get; set; }
    }
}