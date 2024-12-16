using System.Diagnostics;
using FrituurHetHoekje.Data;
using FrituurHetHoekje.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FrituurHetHoekje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FrituurDB _context;

        public HomeController(ILogger<HomeController> logger, FrituurDB context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            foreach (var product in products)
            {
                if (string.IsNullOrEmpty(product.Name) || string.IsNullOrEmpty(product.Photo) || product.Price == null)
                {
                    _logger.LogError("Null or empty values found in product data");
                }
            }
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
