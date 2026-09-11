using Microsoft.AspNetCore.Mvc;
using BibliotecaMVC.Models;

namespace BibliotecaMVC.Controllers
{
    public class LibrosController : Controller
    {
        private readonly IWebHostEnvironment _entorno;

        public LibrosController(IWebHostEnvironment entorno)
        {
            _entorno = entorno;
        }

        private static List<Libro> libros = new List<Libro>
        {
            new Libro { Id = 1, Titulo = "Cien años de soledad", Autor = "Gabriel García Márquez", Categoria = "Novela", AnioPublicacion = 1967, Disponible = true, ImagenUrl = "libro-default.png" },
            new Libro { Id = 2, Titulo = "La casa de los espíritus", Autor = "Isabel Allende", Categoria = "Novela", AnioPublicacion = 1982, Disponible = true, ImagenUrl = "libro-default.png" },
            new Libro { Id = 3, Titulo = "Ficciones", Autor = "Jorge Luis Borges", Categoria = "Cuento", AnioPublicacion = 1944, Disponible = false, ImagenUrl = "libro-default.png" },
            new Libro { Id = 4, Titulo = "La ciudad y los perros", Autor = "Mario Vargas Llosa", Categoria = "Novela", AnioPublicacion = 1963, Disponible = true, ImagenUrl = "libro-default.png" },
            new Libro { Id = 5, Titulo = "Rayuela", Autor = "Julio Cortázar", Categoria = "Novela", AnioPublicacion = 1963, Disponible = false, ImagenUrl = "libro-default.png" }
        };

        // GET: Libros
        public IActionResult Index()
        {
            return View(libros);
        }

        // GET: Libros/Details/5
        public IActionResult Details(int id)
        {
            var libro = libros.FirstOrDefault(l => l.Id == id);
            if (libro == null)
            {
                return NotFound();
            }
            return View(libro);
        }

        // GET: Libros/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Libros/Create
        [HttpPost]
        public async Task<IActionResult> Create(Libro libro, IFormFile imagen)
        {
            libro.Id = libros.Any() ? libros.Max(l => l.Id) + 1 : 1;
            libro.ImagenUrl = await GuardarImagen(imagen);
            libros.Add(libro);
            return RedirectToAction("Index");
        }

        // GET: Libros/Edit/5
        public IActionResult Edit(int id)
        {
            var libro = libros.FirstOrDefault(l => l.Id == id);
            if (libro == null)
            {
                return NotFound();
            }
            return View(libro);
        }

        // POST: Libros/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Libro libroEditado, IFormFile imagen)
        {
            var libro = libros.FirstOrDefault(l => l.Id == id);
            if (libro == null)
            {
                return NotFound();
            }

            libro.Titulo = libroEditado.Titulo;
            libro.Autor = libroEditado.Autor;
            libro.Categoria = libroEditado.Categoria;
            libro.AnioPublicacion = libroEditado.AnioPublicacion;
            libro.Disponible = libroEditado.Disponible;

            if (imagen != null)
            {
                libro.ImagenUrl = await GuardarImagen(imagen);
            }

            return RedirectToAction("Index");
        }

        // GET: Libros/Delete/5
        public IActionResult Delete(int id)
        {
            var libro = libros.FirstOrDefault(l => l.Id == id);
            if (libro == null)
            {
                return NotFound();
            }
            return View(libro);
        }

        // POST: Libros/Delete/5
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var libro = libros.FirstOrDefault(l => l.Id == id);
            if (libro != null)
            {
                libros.Remove(libro);
            }
            return RedirectToAction("Index");
        }

        // Actividad 5 (Reto): guarda la imagen subida en wwwroot/images
        private async Task<string> GuardarImagen(IFormFile imagen)
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