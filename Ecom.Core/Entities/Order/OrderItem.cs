namespace Ecom.Core.Entities.Order
{
    public class OrderItem: BaseEntity<int>
    {
        public OrderItem()
        {
        }

        public OrderItem(int productItemId, decimal price, int quantity, string mainImage, string productName)
        {
            ProductItemId = productItemId;
            Price = price;
            Quantity = quantity;
            MainImage = mainImage;
            ProductName = productName;
        }

        public int ProductItemId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string MainImage { get; set; }
        public string ProductName { get; set; }
    }
}