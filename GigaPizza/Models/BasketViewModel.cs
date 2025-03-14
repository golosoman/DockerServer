using System.Collections.Generic;
using System.Linq;

namespace GigaPizza.Models
{
    public class BasketViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public decimal TotalPrice { get; set; }

        public class CartItem
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Quantity { get; set; }
            public decimal Price { get; set; }
            public string ImageUrl { get; set; }
            public decimal TotalPrice => Quantity * Price;
        }
    }
}
