using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class Libro
    {
        public int Id { get; set; }

        [Display(Name = "Título")]
        public string Titulo { get; set; }

        [Display(Name = "Autor")]
        public string Autor { get; set; }

        [Display(Name = "Categoría")]
        public string Categoria { get; set; }

        [Display(Name = "Año de publicación")]
        public int AnioPublicacion { get; set; }

        [Display(Name = "Disponible")]
        public bool Disponible { get; set; }

        // Nombre del archivo de imagen guardado en wwwroot/images
        public string ImagenUrl { get; set; }
    }
}