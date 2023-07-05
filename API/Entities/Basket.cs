namespace API.Entities
{
    public class Basket
    {
        public int Id { get; set; }
        public string BuyerId { get; set; }
        // the entity framework will take care of relationship
        public List<BasketItem> Items { get; set; } = new();

        // --tracking basket in memory.

        // add basket
        public void AddItem(Product product, int quantity)
        {
            if (Items.All(item => item.ProductId != product.Id))
            {
                Items.Add(new BasketItem { Product = product, Quantity = quantity });
            }

            var existingItem = Items.FirstOrDefault(item => item.ProductId == product.Id);

            if (existingItem != null) existingItem.Quantity += quantity;
        }

        // remove basket
        public void RemoveItem(int productId, int quantity)
        {
            var item = Items.FirstOrDefault(item => item.ProductId == productId);

            if (item == null) return;
            // to reduce the item.
            item.Quantity -= quantity;
            if (item.Quantity == 0) Items.Remove(item);
        }
    }
}