namespace GigaPizza.Controllers
{
    using GigaPizza.Utils;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Threading.Tasks;

    [Route("api/pizza")]
    [ApiController]
    public class PizzaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PizzaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("filter")]
        public IActionResult FilterPizzas([FromBody] PizzaFilterRequest request)
        {
            if (request == null)
            {
                return BadRequest("Некорректные данные.");
            }

            var query = _context.Pizzas.AsQueryable();

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(p => p.Name.Contains(request.SearchTerm));
            }

            if (request.Types != null && request.Types.Any())
            {
                var filterTypes = request.Types.Select(t => t.ToLower()).ToList();

                query = query.Where(p => filterTypes.Any(type =>
        EF.Functions.Like(p.Types.ToLower(), "%" + type + "%")
    ));

            }

            if (request.MinPrice > 0)
            {
                query = query.Where(p => p.Price >= request.MinPrice);
            }

            if (request.MaxPrice > 0 && request.MaxPrice < decimal.MaxValue)
            {
                query = query.Where(p => p.Price <= request.MaxPrice);
            }

            var totalItems = query.Count();
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
            if (string.IsNullOrEmpty(name)) return BadRequest("Pizza name is required.");

            var pizza = await _context.Pizzas.FirstOrDefaultAsync(p => p.Name == name);

            if (pizza == null) return NotFound("Pizza not found.");

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
