namespace EShopCore3.Models
{
    public class Item
    {
        public Guid ItemId { get; set; }
        public string? Category { get; set; }
        public string? ItemName { get; set; }
        public string? Description { get; set; }
        public decimal ItemPrice { get; set; }
        public string? URL1 { get; set; }
        public string? URL2 { get; set; }
        public string? HOVERURL1 { get; set; }
        public string? HOVERURL2 { get; set; }
        public string? Gender { get; set; }
        public string? UsedBy { get; set; }
        public int Promotion { get; set; }
        public decimal PromoPrice { get; set; }
    }
}
