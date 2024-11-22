using ERP.models.enums;

namespace ERP.models.inventory
{
    public class InventoryItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public decimal Quantity { get; set; }
        public InventoryStatus Status { get; set; }

    }
}
