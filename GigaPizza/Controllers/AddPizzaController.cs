using Microsoft.AspNetCore.Mvc;
using GigaPizza.Models;
using GigaPizza.Utils;
using System.IO;
using System.Linq;

namespace GigaPizza.Controllers
{
    [Route("AddPizza")]
    public class AddPizzaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AddPizzaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /AddPizza
        [HttpGet("")]
        public IActionResult Index()
        {
            var model = new AddPizzaViewModel();
            return View(model);
        }


        // POST: /AddPizza
        [HttpPost("")]
        public IActionResult Add(AddPizzaViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Дополнительная серверная валидация
                if (string.IsNullOrWhiteSpace(model.PizzaName) || model.PizzaName.Length < 8 || !model.PizzaName.StartsWith("Пицца "))
                {
                    ModelState.AddModelError("PizzaName", "Название пиццы должно начинаться с 'Пицца' и содержать минимум 8 символов.");
                }

                if (string.IsNullOrWhiteSpace(model.Ingredients) || model.Ingredients.Split(';').Any(ingredient => ingredient.Trim().Length < 3))
                {
                    ModelState.AddModelError("Ingredients", "Ингредиенты должны быть разделены точкой с запятой и не могут содержать элементы длиной менее 3 символов.");
                }

                if (model.Price <= 0)
                {
                    ModelState.AddModelError("Price", "Цена должна быть положительной.");
                }

                if (model.Photo == null || (model.Photo.ContentType != "image/jpeg" && model.Photo.ContentType != "image/png" && model.Photo.ContentType != "image/gif"))
                {
                    ModelState.AddModelError("Photo", "Допустимые форматы изображения: .jpg, .jpeg, .png, .gif.");
                }

                if (model.Categories == null || !model.Categories.Any())
                {
                    ModelState.AddModelError("Categories", "Необходимо выбрать хотя бы одну категорию.");
                }

                if (string.IsNullOrWhiteSpace(model.Description) || model.Description.Length < 20)
                {
                    ModelState.AddModelError("Description", "Описание должно содержать минимум 20 символов.");
                }

                if (string.IsNullOrWhiteSpace(model.RecommendedDrinks) || model.RecommendedDrinks.Split(';').Any(drink => drink.Trim().Length < 3))
                {
                    ModelState.AddModelError("RecommendedDrinks", "Рекомендуемые напитки должны быть разделены точкой с запятой и содержать минимум 3 символа.");
                }

                if (ModelState.IsValid)
                {
                    // Сохранение файла на сервере
                    var fileName = Path.GetFileName(model.Photo.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/pizza/", fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        model.Photo.CopyTo(stream);
                    }

                    var newPizza = new Pizza
                    {
                        Name = model.PizzaName,
                        Ingredients = model.Ingredients,
                        Price = model.Price,
                        Image = "/images/pizza/" + fileName,
                        // Объединяем выбранные категории через запятую
                        Types = string.Join(",", model.Categories),
                        Description = model.Description,
                        RecommendedDrinks = model.RecommendedDrinks
                    };

                    _context.Pizzas.Add(newPizza);
                    _context.SaveChanges();

                    return RedirectToAction("Index", "Catalog");
                }
            }

            TempData["ErrorMessages"] = string.Join("\n", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return View(model);
        }
    }
}
