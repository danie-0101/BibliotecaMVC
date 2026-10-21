
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

        public LibrosController(
            ApplicationDbContext context,
            IWebHostEnvironment entorno)
        {
            _context = context;
            _entorno = entorno;
        }

        // CONSULTAR LIBROS
        // Los tres roles pueden acceder
        public async Task<IActionResult> Index()
        {
            var libros = await _context.Libros.ToListAsync();
            return View(libros);
        }

        // VER DETALLES
        // Los tres roles pueden acceder
        public async Task<IActionResult> Details(int id)
        {
            var libro = await _context.Libros
                .FirstOrDefaultAsync(l => l.Id == id);

            if (libro == null)
                return NotFound();

            return View(libro);
        }

        // CREAR LIBRO - GET
        [Authorize(Roles = "Admin,Bibliotecario")]
        public IActionResult Create()
        {
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
            if (ModelState.IsValid)
            {
                libro.ImagenUrl = await GuardarImagen(imagen);

                _context.Add(libro);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(libro);
        }

        // EDITAR LIBRO - GET
        [Authorize(Roles = "Admin,Bibliotecario")]
        public async Task<IActionResult> Edit(int id)
        {
            var libro = await _context.Libros.FindAsync(id);

            if (libro == null)
                return NotFound();

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
            if (id != libroEditado.Id)
                return NotFound();

            var libro = await _context.Libros.FindAsync(id);

            if (libro == null)
                return NotFound();

            if (ModelState.IsValid)
            {
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

                return RedirectToAction(nameof(Index));
            }

            libroEditado.ImagenUrl = libro.ImagenUrl;
            return View(libroEditado);
        }

        // ELIMINAR LIBRO - GET
        [Authorize(Roles = "Admin,Bibliotecario")]
        public async Task<IActionResult> Delete(int id)
        {
            var libro = await _context.Libros
                .FirstOrDefaultAsync(l => l.Id == id);

            if (libro == null)
                return NotFound();

            return View(libro);
        }

        // ELIMINAR LIBRO - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Bibliotecario")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var libro = await _context.Libros.FindAsync(id);

            if (libro != null)
            {
                _context.Libros.Remove(libro);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GUARDAR IMÁGENES
        private async Task<string?> GuardarImagen(
            IFormFile? imagen)
        {
            if (imagen == null || imagen.Length == 0)
            {
                return "libro-default.png";
            }

            var carpetaImagenes = Path.Combine(
                _entorno.WebRootPath, "images");

            if (!Directory.Exists(carpetaImagenes))
            {
                Directory.CreateDirectory(carpetaImagenes);
            }

            var nombreArchivo =
                $"{Guid.NewGuid()}{Path.GetExtension(imagen.FileName)}";

            var ruta = Path.Combine(
                carpetaImagenes, nombreArchivo);

            using (var stream = new FileStream(
                ruta, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }

            return nombreArchivo;
        }
    }
}
