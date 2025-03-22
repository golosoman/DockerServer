using GigaPizza.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GigaPizza.Controllers
{
    [Route("Catalog")]
    public class CatalogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CatalogController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var pizzas = await _context.Pizzas.ToListAsync();
            return View(pizzas);
        }
    }
}
