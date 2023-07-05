using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    // to name the table
    [Table("BasketItems")]
    public class BasketItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }

        // navigation properties
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // fully define the relationship 
        public int BasketId { get; set; }
        public Basket Basket { get; set; }
    }
}