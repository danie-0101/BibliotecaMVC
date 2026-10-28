
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
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(
            ICategoriaService categoriaService,
            ILogger<CategoriasController> logger)
        {
            _categoriaService = categoriaService;
            _logger = logger;
        }

        // CONSULTAR CATEGORÍAS
        public IActionResult Index()
        {
            _logger.LogInformation(
                "Inicio de consulta de categorías.");

            try
            {
                var categorias = _categoriaService.ObtenerCategorias();
                var lista = categorias.ToList();

                _logger.LogInformation(
                    "Consulta de categorías completada. Registros encontrados: {Cantidad}",
                    lista.Count);

                return View(lista);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Error al consultar las categorías.");

                throw;
            }
        }

        // CREAR CATEGORÍA - GET
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            _logger.LogInformation(
                "Inicio del formulario de creación de categorías.");

            return View();
        }

        // CREAR CATEGORÍA - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create(Categoria categoria)
        {
            _logger.LogInformation(
                "Inicio de creación de una categoría.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "No se pudo crear la categoría porque los datos no son válidos.");

                return View(categoria);
            }

            try
            {
                _categoriaService.AgregarCategoria(categoria);

                _logger.LogInformation(
                    "Categoría creada correctamente con ID {CategoriaId}.",
                    categoria.Id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, "Error al crear una categoría.");

                ModelState.AddModelError(
                    "",
                    "No fue posible crear la categoría. Verifique los datos e inténtelo nuevamente.");

                return View(categoria);
            }
        }

        // EDITAR CATEGORÍA - GET
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id)
        {
            _logger.LogInformation(
                "Inicio de edición de la categoría con ID {CategoriaId}.",
                id);

            if (id <= 0)
            {
                _logger.LogWarning(
                    "Se solicitó editar una categoría con un ID no válido.");

                return BadRequest();
            }

            try
            {
                var categoria = _categoriaService.ObtenerCategoriaPorId(id);

                if (categoria == null)
                {
                    _logger.LogWarning(
                        "No se encontró la categoría con ID {CategoriaId}.",
                        id);

                    return NotFound();
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al consultar la categoría con ID {CategoriaId}.",
                    id);

                throw;
            }
        }

        // EDITAR CATEGORÍA - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id, Categoria categoriaEditada)
        {
            _logger.LogInformation(
                "Inicio de actualización de la categoría con ID {CategoriaId}.",
                id);

            if (id != categoriaEditada.Id)
            {
                _logger.LogWarning(
                    "El ID de la ruta no coincide con el ID de la categoría.");

                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "Los datos de la categoría con ID {CategoriaId} no son válidos.",
                    id);

                return View(categoriaEditada);
            }

            try
            {
                var categoriaExistente =
                    _categoriaService.ObtenerCategoriaPorId(id);

                if (categoriaExistente == null)
                {
                    _logger.LogWarning(
                        "No se puede actualizar: no existe la categoría con ID {CategoriaId}.",
                        id);

                    return NotFound();
                }

                _categoriaService.ActualizarCategoria(categoriaEditada);

                _logger.LogInformation(
                    "Categoría actualizada correctamente con ID {CategoriaId}.",
                    id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al actualizar la categoría con ID {CategoriaId}.",
                    id);

                ModelState.AddModelError(
                    "",
                    "No fue posible actualizar la categoría. Inténtelo nuevamente.");

                return View(categoriaEditada);
            }
        }

        // ELIMINAR CATEGORÍA - GET
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation(
                "Inicio de confirmación de eliminación de la categoría con ID {CategoriaId}.",
                id);

            if (id <= 0)
            {
                _logger.LogWarning(
                    "Se solicitó eliminar una categoría con un ID no válido.");

                return BadRequest();
            }

            try
            {
                var categoria = _categoriaService.ObtenerCategoriaPorId(id);

                if (categoria == null)
                {
                    _logger.LogWarning(
                        "No se encontró la categoría con ID {CategoriaId} para eliminar.",
                        id);

                    return NotFound();
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al consultar la categoría con ID {CategoriaId} para eliminar.",
                    id);

                throw;
            }
        }

        // ELIMINAR CATEGORÍA - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            _logger.LogInformation(
                "Inicio de eliminación de la categoría con ID {CategoriaId}.",
                id);

            if (id <= 0)
            {
                _logger.LogWarning(
                    "Se solicitó eliminar una categoría con un ID no válido.");

                return BadRequest();
            }

            try
            {
                var categoria = _categoriaService.ObtenerCategoriaPorId(id);

                if (categoria == null)
                {
                    _logger.LogWarning(
                        "No se puede eliminar: no existe la categoría con ID {CategoriaId}.",
                        id);

                    return NotFound();
                }

                _categoriaService.EliminarCategoria(id);

                _logger.LogInformation(
                    "Categoría eliminada correctamente con ID {CategoriaId}.",
                    id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al eliminar la categoría con ID {CategoriaId}.",
                    id);

                TempData["ErrorMessage"] =
                    "No fue posible eliminar la categoría. Inténtelo nuevamente.";

                return RedirectToAction(nameof(Index));
            }
        }
    }
}
