using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;
using BibliotecaMVC.Services;

namespace BibliotecaMVC.Controllers
{
    [Authorize(Roles = "Administrador,Lector")]
    public class CategoriasController : Controller
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        /// <summary>
        /// Muestra el listado de todas las categorías
        /// </summary>
        public IActionResult Index()
        {
            try
            {
                var categorias = _categoriaService.ObtenerCategorias();
                return View(categorias.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al cargar las categorías: " + ex.Message;
                return View(new List<Categoria>());
            }
        }

        /// <summary>
        /// Muestra el formulario para crear una nueva categoría
        /// </summary>
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Procesa la creación de una nueva categoría
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            try
            {
                _categoriaService.AgregarCategoria(categoria);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear la categoría: " + ex.Message);
                return View(categoria);
            }
        }

        /// <summary>
        /// Muestra el formulario para editar una categoría existente
        /// </summary>
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            try
            {
                var categoria = _categoriaService.ObtenerCategoriaPorId(id);
                if (categoria == null)
                {
                    return NotFound();
                }
                return View(categoria);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al cargar la categoría: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Procesa la actualización de una categoría existente
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id, Categoria categoriaEditada)
        {
            if (id != categoriaEditada.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(categoriaEditada);
            }

            try
            {
                var categoriaExistente = _categoriaService.ObtenerCategoriaPorId(id);
                if (categoriaExistente == null)
                {
                    return NotFound();
                }

                _categoriaService.ActualizarCategoria(categoriaEditada);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar la categoría: " + ex.Message);
                return View(categoriaEditada);
            }
        }

        /// <summary>
        /// Muestra la página de confirmación de eliminación
        /// </summary>
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            try
            {
                var categoria = _categoriaService.ObtenerCategoriaPorId(id);
                if (categoria == null)
                {
                    return NotFound();
                }
                return View(categoria);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al cargar la categoría: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Procesa la confirmación de eliminación de una categoría
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            try
            {
                var categoria = _categoriaService.ObtenerCategoriaPorId(id);
                if (categoria == null)
                {
                    return NotFound();
                }

                _categoriaService.EliminarCategoria(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al eliminar la categoría: " + ex.Message);
                return RedirectToAction("Index");
            }
        }
    }
}
