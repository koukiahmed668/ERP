namespace ERP.api.sales.dtos
{
    public class SaleDto
    {
        public Guid Id { get; set; } 
        public string CustomerName { get; set; }
        public DateTime Date { get; set; }
        public List<SaleItemDto> ItemsSold { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
    }

    public class SaleItemDto
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
