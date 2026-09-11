using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using BibliotecaMVC.Services;

namespace BibliotecaMVC.Controllers
{
    public class AutoresController : Controller
    {
        private readonly IAutorService _autorService;

        public AutoresController(IAutorService autorService)
        {
            _autorService = autorService;
        }

        public IActionResult Index()
        {
            var autores = _autorService.ObtenerAutores();
            return View(autores.ToList());
        }

        public IActionResult Edit(int id)
        {
            var autor = _autorService.ObtenerAutorPorId(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        [HttpPost]
        public IActionResult Edit(int id, Autor autorEditado)
        {
            var autor = _autorService.ObtenerAutorPorId(id);
            if (autor == null)
            {
                return NotFound();
            }

            _autorService.ActualizarAutor(autorEditado);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var autor = _autorService.ObtenerAutorPorId(id);
            if (autor == null)
            {
                return NotFound();
            }
            return View(autor);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _autorService.EliminarAutor(id);
            return RedirectToAction("Index");
        }
    }
}
