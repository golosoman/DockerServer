
using GigaPizza.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GigaPizza.Controllers
{
    [Route("Contact")]
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
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
