using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services
{
    public interface ICategoriaService
    {
        /// <summary>
        /// Obtiene todas las categorías de la base de datos
        /// </summary>
        IEnumerable<Categoria> ObtenerCategorias();

        /// <summary>
        /// Obtiene una categoría por su ID
        /// </summary>
        Categoria? ObtenerCategoriaPorId(int id);

        /// <summary>
        /// Agrega una nueva categoría a la base de datos
        /// </summary>
        void AgregarCategoria(Categoria categoria);

        /// <summary>
        /// Actualiza una categoría existente en la base de datos
        /// </summary>
        void ActualizarCategoria(Categoria categoria);

        /// <summary>
        /// Elimina una categoría de la base de datos
        /// </summary>
        void EliminarCategoria(int id);
    }
}
