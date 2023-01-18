namespace EShopCore3.ViewModels
{
    public class CartViewModel
    {
        public string? ItemId { get; set; }
        public decimal Quantity { get; set; }
        public string? ItemName { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public decimal PromoPrice { get; set; }
    }
}
