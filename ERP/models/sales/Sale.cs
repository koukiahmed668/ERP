namespace ERP.models.sales
{
    public class Sale
    {
         public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public DateTime Date { get; set; }
        public List<SaleItem> ItemsSold { get; set; } = new List<SaleItem>();
        public decimal TotalAmount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }
}
