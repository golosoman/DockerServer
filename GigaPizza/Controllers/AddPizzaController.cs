using Microsoft.AspNetCore.Mvc;
using GigaPizza.Models;
using System.IO;
using System.Linq;
using GigaPizza.Domain;

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
        //[HttpGet("")]
        //public IActionResult Index()
        //{
        //    var model = new AddPizzaViewModel();
        //    return View(model);
        //}

        public IActionResult Index()
        {
            return View();
        }
    }
}
