using System.Text.Json.Serialization;

namespace GigaPizza.Models
{
    public class Pizza
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ingredients { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string RecommendedDrinks { get; set; }
        [JsonIgnore]
        public ICollection<PizzaType> Types { get; set; }
    }

    public class PizzaType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public ICollection<Pizza> Pizzas { get; set; }
    }

}
