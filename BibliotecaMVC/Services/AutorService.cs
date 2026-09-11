using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services
{
    public class AutorService : IAutorService
    {
        private static readonly List<Autor> _autores = new List<Autor>
        {
            new Autor { Id = 1, Nombre = "Gabriel", Apellido = "García Márquez", Nacionalidad = "Colombiana", FechaNacimiento = new DateTime(1927, 3, 6), Activo = true },
            new Autor { Id = 2, Nombre = "Isabel", Apellido = "Allende", Nacionalidad = "Chilena", FechaNacimiento = new DateTime(1942, 8, 2), Activo = true },
            new Autor { Id = 3, Nombre = "Jorge Luis", Apellido = "Borges", Nacionalidad = "Argentina", FechaNacimiento = new DateTime(1899, 8, 24), Activo = false },
            new Autor { Id = 4, Nombre = "Mario", Apellido = "Vargas Llosa", Nacionalidad = "Peruana", FechaNacimiento = new DateTime(1936, 3, 28), Activo = true },
            new Autor { Id = 5, Nombre = "Julio", Apellido = "Cortázar", Nacionalidad = "Argentina", FechaNacimiento = new DateTime(1914, 8, 26), Activo = false }
        };

        public IEnumerable<Autor> ObtenerAutores()
        {
            return _autores;
        }

        public Autor? ObtenerAutorPorId(int id)
        {
            return _autores.FirstOrDefault(a => a.Id == id);
        }

        public void ActualizarAutor(Autor autorEditado)
        {
            var autor = _autores.FirstOrDefault(a => a.Id == autorEditado.Id);
            if (autor == null)
            {
                return;
            }

            autor.Nombre = autorEditado.Nombre;
            autor.Apellido = autorEditado.Apellido;
            autor.Nacionalidad = autorEditado.Nacionalidad;
            autor.FechaNacimiento = autorEditado.FechaNacimiento;
            autor.Activo = autorEditado.Activo;
        }

        public void EliminarAutor(int id)
        {
            var autor = _autores.FirstOrDefault(a => a.Id == id);
            if (autor != null)
            {
                _autores.Remove(autor);
            }
        }
    }
}
