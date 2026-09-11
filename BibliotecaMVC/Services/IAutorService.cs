using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services
{
    public interface IAutorService
    {
        IEnumerable<Autor> ObtenerAutores();

        Autor? ObtenerAutorPorId(int id);

        void ActualizarAutor(Autor autorEditado);

        void EliminarAutor(int id);
    }
}
