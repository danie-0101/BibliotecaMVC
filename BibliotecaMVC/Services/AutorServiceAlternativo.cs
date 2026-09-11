using BibliotecaMVC.Models;

namespace BibliotecaMVC.Services
{
    public class AutorServiceAlternativo : IAutorService
    {
        private static readonly List<Autor> _autores = new List<Autor>
        {
            new Autor { Id = 1, Nombre = "Octavio", Apellido = "Paz", Nacionalidad = "Mexicana", FechaNacimiento = new DateTime(1914, 3, 31), Activo = true },
            new Autor { Id = 2, Nombre = "Pablo", Apellido = "Neruda", Nacionalidad = "Chilena", FechaNacimiento = new DateTime(1904, 7, 12), Activo = true },
            new Autor { Id = 3, Nombre = "Rómulo", Apellido = "Gallegos", Nacionalidad = "Venezolana", FechaNacimiento = new DateTime(1884, 8, 2), Activo = false }
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
