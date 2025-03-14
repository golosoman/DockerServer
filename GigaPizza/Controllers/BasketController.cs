using Microsoft.AspNetCore.Mvc;
using GigaPizza.Models;
using System.Collections.Generic;
using System.Linq;

namespace GigaPizza.Controllers
{
    public class BasketController : Controller
    {
        // GET: /Basket
        public IActionResult Index()
        {
            // Здесь эмулируем данные корзины. В реальном приложении вы получите их из сессии или БД.
            var model = new BasketViewModel
            {
                CartItems = new List<BasketViewModel.CartItem>
                {
                    new BasketViewModel.CartItem { Id = 1, Name = "Пицца Ветчина и Грибы", Quantity = 1, Price = 549, ImageUrl = "/images/pizza/pizza_hum_and_mushrooms.avif" },
                    new BasketViewModel.CartItem { Id = 2, Name = "Пицца Маргарита", Quantity = 2, Price = 549, ImageUrl = "/images/pizza/pizza_margarita.avif" },
                    new BasketViewModel.CartItem { Id = 3, Name = "Пицца Пепперони", Quantity = 1, Price = 450, ImageUrl = "/images/pizza/pizza_pepperoni.avif" }
                }
            };

            model.TotalPrice = model.CartItems.Sum(item => item.TotalPrice);

            return View(model);
        }
    }
}
