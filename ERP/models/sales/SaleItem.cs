using ERP.models.inventory;

namespace ERP.models.sales
{
    public class SaleItem
    {
        public Guid ItemId { get; set; }  // Foreign key to InventoryItem
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // Navigation property to link to InventoryItem
        public InventoryItem InventoryItem { get; set; }

        public Sale Sale { get; set; }  

    }
}
