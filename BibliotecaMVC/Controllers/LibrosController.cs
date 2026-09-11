using BibliotecaMVC.Data;
using BibliotecaMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _entorno;

        public LibrosController(ApplicationDbContext context, IWebHostEnvironment entorno)
        {
            _context = context;
            _entorno = entorno;
        }

        // GET: Libros  → Actividad 2: Mostrar (EF Core)
        public async Task<IActionResult> Index()
        {
            var libros = await _context.Libros.ToListAsync();
            return View(libros);
        }

        // GET: Libros/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var libro = await _context.Libros.FirstOrDefaultAsync(l => l.Id == id);
            if (libro == null) return NotFound();
            return View(libro);
        }

        // GET: Libros/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Libros/Create  → Actividad 3: Agregar (EF Core)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Libro libro, IFormFile? imagen)
        {
            if (ModelState.IsValid)
            {
                libro.ImagenUrl = await GuardarImagen(imagen);
                _context.Add(libro);                 // Agrega el libro al DbSet
                await _context.SaveChangesAsync();   // Guarda en SQL Server
                return RedirectToAction(nameof(Index));
            }
            return View(libro);
        }

        // GET: Libros/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null) return NotFound();
            return View(libro);
        }

        // POST: Libros/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Libro libroEditado, IFormFile? imagen)
        {
            if (id != libroEditado.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (imagen != null && imagen.Length > 0)
                {
                    libroEditado.ImagenUrl = await GuardarImagen(imagen);
                }

                _context.Update(libroEditado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(libroEditado);
        }

        // GET: Libros/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var libro = await _context.Libros.FirstOrDefaultAsync(l => l.Id == id);
            if (libro == null) return NotFound();
            return View(libro);
        }

        // POST: Libros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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

        // Guarda la imagen subida en wwwroot/images
        private async Task<string?> GuardarImagen(IFormFile? imagen)
        {
            if (imagen == null || imagen.Length == 0)
            {
                return "libro-default.png";
            }

            var carpetaImagenes = Path.Combine(_entorno.WebRootPath, "images");
            if (!Directory.Exists(carpetaImagenes))
            {
                Directory.CreateDirectory(carpetaImagenes);
            }

            var nombreArchivo = $"{Guid.NewGuid()}{Path.GetExtension(imagen.FileName)}";
            var ruta = Path.Combine(carpetaImagenes, nombreArchivo);

            using (var stream = new FileStream(ruta, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }

            return nombreArchivo;
        }
    }
}