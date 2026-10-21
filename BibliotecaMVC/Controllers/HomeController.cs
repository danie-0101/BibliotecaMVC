
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // PÁGINA DE INICIO
        public IActionResult Index()
        {
            return View();
        }

        // PÁGINA ACERCA DE
        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        // PÁGINA DE PRIVACIDAD
        public IActionResult Privacy()
        {
            return View();
        }

        // PÁGINA DE ERROR
        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}
