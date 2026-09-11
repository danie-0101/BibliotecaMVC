using BibliotecaMVC.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace BibliotecaMVC.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly string _connectionString;

        public CategoriaService(IConfiguration configuration)
        {
            // Obtener la cadena de conexión desde appsettings.json
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Obtiene todas las categorías de la base de datos
        /// </summary>
        public IEnumerable<Categoria> ObtenerCategorias()
        {
            var categorias = new List<Categoria>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Consulta SQL parametrizada (aunque sin parámetros, es buena práctica)
                    string query = "SELECT Id, Nombre, Descripcion FROM Categorias ORDER BY Nombre";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.Text;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                categorias.Add(new Categoria
                                {
                                    Id = (int)reader["Id"],
                                    Nombre = (string)reader["Nombre"],
                                    Descripcion = reader["Descripcion"] != DBNull.Value ? (string)reader["Descripcion"] : null
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error al obtener categorías: {ex.Message}");
                // En producción, aquí podrías registrar el error en un log
                throw;
            }

            return categorias;
        }

        /// <summary>
        /// Obtiene una categoría específica por su ID
        /// </summary>
        public Categoria? ObtenerCategoriaPorId(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Consulta SQL parametrizada
                    string query = "SELECT Id, Nombre, Descripcion FROM Categorias WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.Text;

                        // Agregar parámetro SQL para evitar SQL injection
                        command.Parameters.AddWithValue("@Id", id);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Categoria
                                {
                                    Id = (int)reader["Id"],
                                    Nombre = (string)reader["Nombre"],
                                    Descripcion = reader["Descripcion"] != DBNull.Value ? (string)reader["Descripcion"] : null
                                };
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error al obtener categoría por ID: {ex.Message}");
                throw;
            }

            return null;
        }

        /// <summary>
        /// Agrega una nueva categoría a la base de datos
        /// </summary>
        public void AgregarCategoria(Categoria categoria)
        {
            if (categoria == null)
            {
                throw new ArgumentNullException(nameof(categoria));
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Consulta INSERT con SQL parametrizado
                    string query = "INSERT INTO Categorias (Nombre, Descripcion) VALUES (@Nombre, @Descripcion)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.Text;

                        // Agregar parámetros SQL para evitar SQL injection
                        command.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                        command.Parameters.AddWithValue("@Descripcion", categoria.Descripcion ?? (object)DBNull.Value);

                        // Ejecutar el comando
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error al agregar categoría: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Actualiza una categoría existente en la base de datos
        /// </summary>
        public void ActualizarCategoria(Categoria categoria)
        {
            if (categoria == null)
            {
                throw new ArgumentNullException(nameof(categoria));
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Consulta UPDATE con SQL parametrizado
                    string query = "UPDATE Categorias SET Nombre = @Nombre, Descripcion = @Descripcion WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.Text;

                        // Agregar parámetros SQL para evitar SQL injection
                        command.Parameters.AddWithValue("@Nombre", categoria.Nombre);
                        command.Parameters.AddWithValue("@Descripcion", categoria.Descripcion ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Id", categoria.Id);

                        // Ejecutar el comando
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new InvalidOperationException($"No se encontró la categoría con ID {categoria.Id}");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error al actualizar categoría: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Elimina una categoría de la base de datos
        /// </summary>
        public void EliminarCategoria(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Consulta DELETE con SQL parametrizado
                    string query = "DELETE FROM Categorias WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.Text;

                        // Agregar parámetro SQL para evitar SQL injection
                        command.Parameters.AddWithValue("@Id", id);

                        // Ejecutar el comando
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            throw new InvalidOperationException($"No se encontró la categoría con ID {id}");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error al eliminar categoría: {ex.Message}");
                throw;
            }
        }
    }
}
