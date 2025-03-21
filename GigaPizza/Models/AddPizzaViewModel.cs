﻿namespace GigaPizza.Models
{
    public class AddPizzaViewModel
    {
        public string PizzaName { get; set; }
        public string Ingredients { get; set; }
        public decimal Price { get; set; }
        public IFormFile Photo { get; set; }
        public string[] Categories { get; set; }
        public string Description { get; set; }
        public string RecommendedDrinks { get; set; }
    }
}
