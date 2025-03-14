namespace GigaPizza.Models
{
    public class Pizza
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Ingredients { get; set; }
        public required decimal Price { get; set; }
        public required string Image { get; set; }
        public required string Types { get; set; }
        public required string Description { get; set; }
        public required string RecommendedDrinks { get; set; }
    }
}
