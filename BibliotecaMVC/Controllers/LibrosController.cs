
using Microsoft.AspNetCore.Authorization;
using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Controllers
{
    [Authorize(Roles = "Admin,Bibliotecario,Usuario")]
    public class LibrosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _entorno;
        private readonly ILogger<LibrosController> _logger;

        public LibrosController(
            ApplicationDbContext context,
            IWebHostEnvironment entorno,
            ILogger<LibrosController> logger)
        {
            _context = context;
            _entorno = entorno;
            _logger = logger;
        }

        // CONSULTAR LIBROS
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation(
                "Inicio de consulta de libros.");

            var libros = await _context.Libros.ToListAsync();

            _logger.LogInformation(
                "Consulta de libros completada. Registros encontrados: {Cantidad}",
                libros.Count);

            return View(libros);
        }

        // VER DETALLES
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation(
                "Consulta de detalles del libro con ID {LibroId}.", id);

            var libro = await _context.Libros
                .FirstOrDefaultAsync(l => l.Id == id);

            if (libro == null)
            {
                _logger.LogWarning(
                    "No se encontró el libro con ID {LibroId}.", id);

                return NotFound();
            }

            return View(libro);
        }

        // CREAR LIBRO - GET
        [Authorize(Roles = "Admin,Bibliotecario")]
        public IActionResult Create()
        {
            _logger.LogInformation(
                "Inicio del formulario de creación de libros.");

            return View();
        }

        // CREAR LIBRO - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Bibliotecario")]
        public async Task<IActionResult> Create(
            Libro libro,
            IFormFile? imagen)
        {
            _logger.LogInformation(
                "Inicio de creación de un libro.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "No se pudo crear el libro porque los datos no son válidos.");

                return View(libro);
            }

            if (imagen != null && imagen.Length > 0)
            {
                libro.ImagenUrl = await GuardarImagen(imagen);
            }
            else
            {
                libro.ImagenUrl = "libro-default.png";
            }

            _context.Add(libro);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Libro creado correctamente con ID {LibroId}.",
                libro.Id);

            return RedirectToAction(nameof(Index));
        }

        // EDITAR LIBRO - GET
        [Authorize(Roles = "Admin,Bibliotecario")]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation(
                "Inicio de edición del libro con ID {LibroId}.", id);

            var libro = await _context.Libros.FindAsync(id);

            if (libro == null)
            {
                _logger.LogWarning(
                    "No se puede editar: no existe el libro con ID {LibroId}.",
                    id);

                return NotFound();
            }

            return View(libro);
        }

        // EDITAR LIBRO - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Bibliotecario")]
        public async Task<IActionResult> Edit(
            int id,
            Libro libroEditado,
            IFormFile? imagen)
        {
            _logger.LogInformation(
                "Inicio de actualización del libro con ID {LibroId}.", id);

            if (id != libroEditado.Id)
            {
                _logger.LogWarning(
                    "El ID de la ruta no coincide con el ID del libro enviado.");

                return NotFound();
            }

            var libro = await _context.Libros.FindAsync(id);

            if (libro == null)
            {
                _logger.LogWarning(
                    "No se puede actualizar: no existe el libro con ID {LibroId}.",
                    id);

                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "Los datos del libro con ID {LibroId} no son válidos.",
                    id);

                libroEditado.ImagenUrl = libro.ImagenUrl;
                return View(libroEditado);
            }

            libro.Titulo = libroEditado.Titulo;
            libro.Autor = libroEditado.Autor;
            libro.Categoria = libroEditado.Categoria;
            libro.AnioPublicacion = libroEditado.AnioPublicacion;
            libro.Disponible = libroEditado.Disponible;

            if (imagen != null && imagen.Length > 0)
            {
                libro.ImagenUrl = await GuardarImagen(imagen);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Libro actualizado correctamente con ID {LibroId}.", id);

            return RedirectToAction(nameof(Index));
        }

        // ELIMINAR LIBRO - GET
        [Authorize(Roles = "Admin,Bibliotecario")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation(
                "Inicio de confirmación de eliminación del libro con ID {LibroId}.",
                id);

            var libro = await _context.Libros
                .FirstOrDefaultAsync(l => l.Id == id);

            if (libro == null)
            {
                _logger.LogWarning(
                    "No se encontró el libro con ID {LibroId} para eliminar.",
                    id);

                return NotFound();
            }

            return View(libro);
        }

        // ELIMINAR LIBRO - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Bibliotecario")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation(
                "Inicio de eliminación del libro con ID {LibroId}.", id);

            var libro = await _context.Libros.FindAsync(id);

            if (libro == null)
            {
                _logger.LogWarning(
                    "No se puede eliminar: no existe el libro con ID {LibroId}.",
                    id);

                return NotFound();
            }

            _context.Libros.Remove(libro);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Libro eliminado correctamente con ID {LibroId}.", id);

            return RedirectToAction(nameof(Index));
        }

        // GUARDAR IMÁGENES
        private async Task<string?> GuardarImagen(IFormFile? imagen)
        {
            if (imagen == null || imagen.Length == 0)
            {
                return "libro-default.png";
            }

            var carpetaImagenes = Path.Combine(
                _entorno.WebRootPath ?? "wwwroot", "images");

            if (!Directory.Exists(carpetaImagenes))
            {
                Directory.CreateDirectory(carpetaImagenes);
            }

            var extension = Path.GetExtension(imagen.FileName);
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";

            var ruta = Path.Combine(
                carpetaImagenes, nombreArchivo);

            using (var stream = new FileStream(ruta, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }

            return nombreArchivo;
        }
    }
}
