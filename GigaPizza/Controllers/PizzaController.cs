namespace GigaPizza.Controllers
{
    using GigaPizza.Domain;
    using GigaPizza.Models;
    using GigaPizza.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System.Linq;
    using System.Threading.Tasks;

    [Route("api/pizza")]
    [ApiController]
    public class PizzaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PizzaController> _logger;

        public PizzaController(ApplicationDbContext context, ILogger<PizzaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("filter")]
        public IActionResult FilterPizzas([FromBody] PizzaFilterRequest request)
        {
            if (request == null)
            {
                return BadRequest("Некорректные данные.");
            }

            // Логируем запрос
            _logger.LogInformation($"Filter request: SearchTerm: {request.SearchTerm}, Types: {string.Join(", ", request.Types)}, MinPrice: {request.MinPrice}, MaxPrice: {request.MaxPrice}, Page: {request.Page}, ItemsPerPage: {request.ItemsPerPage}");

            var query = _context.Pizzas.Include(p => p.Types).AsQueryable();

            // Фильтрация по названию
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                string searchTerm = $"%{request.SearchTerm}%";

                Console.WriteLine($"Фильтрация по названию: {searchTerm}");
                _logger.LogInformation($"Фильтрация по названию: {searchTerm}");

                query = query.Where(p => EF.Functions.Like(p.Name, searchTerm));
            }


            // Фильтрация по типам
            if (request.Types.Any())
            {
                query = query.Where(p => p.Types.Any(t => request.Types.Contains(t.Name)));
            }

            // Фильтрация по минимальной цене
            if (request.MinPrice > 0)
            {
                query = query.Where(p => p.Price >= request.MinPrice);
            }

            // Фильтрация по максимальной цене
            if (request.MaxPrice < decimal.MaxValue)
            {
                query = query.Where(p => p.Price <= request.MaxPrice);
            }

            // Подсчёт общего числа элементов
            var totalItems = query.Count();

            // Пагинация
            var pizzas = query.Skip((request.Page - 1) * request.ItemsPerPage)
                              .Take(request.ItemsPerPage)
                              .ToList();

            return Ok(new
            {
                Pizzas = pizzas,
                CurrentPage = request.Page,
                TotalPages = (int)Math.Ceiling(totalItems / (double)request.ItemsPerPage)
            });
        }


        [HttpGet("details")]
        public async Task<IActionResult> GetPizzaDetails([FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest("Pizza name is required.");

            // Подключаем Types для отображения всех данных пиццы
            var pizza = await _context.Pizzas
                .Include(p => p.Types)
                .FirstOrDefaultAsync(p => p.Name == name);

            if (pizza == null)
                return NotFound("Pizza not found.");

            return Ok(pizza);
        }

        [HttpGet("autocomplete")]
        public async Task<IActionResult> Autocomplete([FromQuery] string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return BadRequest("Поисковый запрос не может быть пустым.");
            }

            var suggestions = await _context.Pizzas
                .Where(p => p.Name.Contains(term))
                .Select(p => p.Name)
                .Distinct()
                .ToListAsync();

            return Ok(suggestions);
        }


        // POST:
        [HttpPost("")]
        public IActionResult Add([FromForm] AddPizzaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kv => kv.Value.Errors.Any())
                    .ToDictionary(
                        kv => kv.Key,
                        kv => kv.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                return BadRequest(new { success = false, errors });
            }

            // Дополнительная серверная валидация
            if (string.IsNullOrWhiteSpace(model.PizzaName) || model.PizzaName.Length < 8 || !model.PizzaName.StartsWith("Пицца "))
            {
                ModelState.AddModelError("PizzaName", "Название пиццы должно начинаться с 'Пицца' и содержать минимум 8 символов.");
            }

            if (string.IsNullOrWhiteSpace(model.Ingredients) || model.Ingredients.Split(';').Any(ingredient => ingredient.Trim().Length < 3))
            {
                ModelState.AddModelError("Ingredients", "Ингредиенты должны быть разделены точкой с запятой и не могут содержать элементы длиной менее 3 символов.");
            }

            if (model.Price <= 200)
            {
                ModelState.AddModelError("Price", "Цена должна быть положительной и больше 200");
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

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kv => kv.Value.Errors.Any())
                    .ToDictionary(
                        kv => kv.Key,
                        kv => kv.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                return BadRequest(new { success = false, errors });
            }

            // Если всё успешно
            PizzaService pizzaService = new PizzaService();
            _context.Pizzas.Add(pizzaService.ProcessPizzaInput(model));
            _context.SaveChanges();

            return Ok(new { success = true, message = "Пицца успешно добавлена!" });
        }

    }

    public class PizzaFilterRequest
    {
        public List<string> Types { get; set; } = new();
        public string SearchTerm { get; set; } = string.Empty;
        public decimal MinPrice { get; set; } = 0;
        public decimal MaxPrice { get; set; } = decimal.MaxValue;
        public int Page { get; set; } = 1;
        public int ItemsPerPage { get; set; } = 4;
    }

}
