using System.ComponentModel.DataAnnotations;

namespace BibliotecaMVC.Models
{
    public class Libro
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200, ErrorMessage = "El título no puede pasar de 200 caracteres")]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El autor es obligatorio")]
        [StringLength(150)]
        [Display(Name = "Autor")]
        public string Autor { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria")]
        [StringLength(100)]
        [Display(Name = "Categoría")]
        public string Categoria { get; set; } = string.Empty;

        [Range(1000, 2100, ErrorMessage = "El año debe estar entre 1000 y 2100")]
        [Display(Name = "Año de publicación")]
        public int AnioPublicacion { get; set; }

        [Display(Name = "Disponible")]
        public bool Disponible { get; set; } = true;

        [Display(Name = "Imagen")]
        public string? ImagenUrl { get; set; }  // opcional
    }
}